using DbcLib.Definitions.NodeDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.MessageDefinitions
{
    public class Message : DbcObject
    {
        private Dbc parent;
        public Dbc Parent
        {
            get { return parent; }
        }
        public UInt32 Id { get; set; }
        public int Size { get; set; }
        private List<string> transmitters;
        public IReadOnlyList<string> Transmitters => new ReadOnlyCollection<string>(transmitters);
        private List<Signal> signals;
        public IReadOnlyList<Signal> Signals => new ReadOnlyCollection<Signal>(signals);
        private List<SignalGroup> signalGroups;
        public IReadOnlyList<SignalGroup> SignalGroups => new ReadOnlyCollection<SignalGroup>(signalGroups);
        public Message(Dbc parent, string name)
        {
            this.parent = parent;
            this.name = name;
            transmitters = new List<string>();
            signals = new List<Signal>();
            signalGroups = new List<SignalGroup>();
        }
        internal bool AddTransmitter(string node)
        {
            if (this.transmitters.Contains(node))
            {
#if DEBUG
                throw new Exception($"{this.Name}'transmitters contains {node}");
#else
                return false;
#endif
            }
            transmitters.Add(node);
            return true;
        }
        internal bool RemoveTransmitter(string node)
        {
            if (!this.transmitters.Contains(node))
            {
#if DEBUG
                throw new Exception($"{this.Name}'transmitters do not contains {node}");
#else
                return false;
#endif
            }
            transmitters.Remove(node);
            return true;
        }
        internal bool ClearTransmitter()
        {
            transmitters.Clear();
            return true;
        }

        private void _addSignal(Signal signal)
        {
            if (this.Name == "VECTOR__INDEPENDENT_SIG_MSG")
            {
                // 如果信号添加到"VECTOR__INDEPENDENT_SIG_MSG"下，则直接添加。
                signal.messages.Add("VECTOR__INDEPENDENT_SIG_MSG");
                return;
            }

            if (signal.messages.Contains("VECTOR__INDEPENDENT_SIG_MSG"))
            {
                // 如果添加到其他报文下，则从"VECTOR__INDEPENDENT_SIG_MSG"信号下移除。
                this.parent.Messages.Where(o => o.Name == "VECTOR__INDEPENDENT_SIG_MSG").First().RemoveSignal(signal);
            }
        }
        private void _removeSignal(Signal signal)
        {
            if (this.Name == "VECTOR__INDEPENDENT_SIG_MSG")
            {
                signal.messages.Remove(this.Name);
                return;
            }

            signal.messages.Remove(this.Name);
            
            if (signal.messages.Count() != 0)
            {
                return;
            }

            // 对于没有归属的信号，默认归属到"VECTOR__INDEPENDENT_SIG_MSG"报文下。
            var l = this.parent.Messages.Where(o => o.Name == "VECTOR__INDEPENDENT_SIG_MSG");

            if (l.Count() != 0)
            {
                l.First().AddSignal(signal);
                return;
            }

            // 如果反序列化的文件没有描述"VECTOR__INDEPENDENT_SIG_MSG"报文，则生成一个
            var message = this.parent.CreateMessage("VECTOR__INDEPENDENT_SIG_MSG");
            message.Id = 0xC0000000;
            message.Size = 0;
            message.Comment = "This is a message for not used signals, created by Vector CANdb++ DBC OLE DB Provider.";

            message.AddSignal(signal);
        }
        public bool AddSignal(Signal signal)
        {
            if (signals.Contains(signal))
            {
#if DEBUG
                throw new Exception($"{this.Name} contains {signal.Name} Signal");
#else
                return false;
#endif
            }
            signals.Add(signal);

            _addSignal(signal);

            return true;
        }
        public bool RemoveSignal(Signal signal)
        {
            if (!signals.Contains(signal))
            {
#if DEBUG
                throw new Exception($"{this.Name} contains {signal.Name} Signal");
#else
                return false;
#endif
            }

            foreach (var group in signalGroups)
            {
                if (group.Signals.Contains(signal))
                {
                    group.RemoveSignal(signal);
                }
            }

            signals.Remove(signal);

            _removeSignal(signal);

            return true;
        }
        public SignalGroup CreateSignalGroup(string name)
        {
            var signalGroup = new SignalGroup(1, name);
            signalGroups.Add(signalGroup);
            return signalGroup;
        }
        public bool AddSignalToSignalGroup(Signal signal, string signalGroupName)
        {
            var signalGroup = signalGroups.Where(o => o.Name == signalGroupName);
            if (signalGroup.Count() == 0)
            {
#if DEBUG
                throw new Exception($"{this.Name} do not contains {signalGroupName} SignalGroup");
#else
                return false;
#endif
            }
            if (!signals.Contains(signal))
            {
                AddSignal(signal);
            }
            return signalGroup.First().AddSignal(signal);
        }
        public bool RemoveSignalToSignalGroup(Signal signal, string signalGroupName)
        {
            var signalGroup = signalGroups.Where(o => o.Name == signalGroupName);
            if (signalGroup.Count() == 0)
            {
#if DEBUG
                throw new Exception($"{this.Name} do not contains {signalGroupName} SignalGroup");
#else
                return false;
#endif
            }
            return signalGroup.First().RemoveSignal(signal);
        }
    }
}