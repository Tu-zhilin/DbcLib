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
                if (group.Signals.Contains(signal.Name))
                {
                    group.RemoveSignal(signal.Name);
                }
            }

            signals.Remove(signal);

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
            return signalGroup.First().AddSignal(signal.Name);
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
            return signalGroup.First().RemoveSignal(signal.Name);
        }
        //        public IEnumerable<Signal> GetSiganlsFromSignalGroup(string signalGroupName)
        //        {
        //            var signalGroup = signalGroups.Where(o => o.Name == signalGroupName);
        //            if (signalGroup.Count() == 0)
        //            {
        //#if DEBUG
        //                throw new Exception($"{this.Name} do not contains {signalGroupName} SignalGroup");
        //#else
        //                return false;
        //#endif
        //            }
        //            return signalGroup.First().Signals;
        //        }
    }
}