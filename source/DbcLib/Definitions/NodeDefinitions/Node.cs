using DbcLib.Definitions.MessageDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.NodeDefinitions
{
    public class Node : DbcObject
    {
        private Dbc parent;
        public Dbc Parent
        {
            get { return parent; }
        }
        public Node(Dbc parent, string name)
        {
            this.parent = parent;
            this.name = name;
        }
        public IEnumerable<Message> GetTxMessages()
        {
            return this.parent.Messages.Where(o => o.Transmitters.Contains(this.Name));
        }
        public IEnumerable<Message> GetRxMessages()
        {
            return this.parent.Messages.Where(o => o.Signals.Where(s => s.Receivers.Contains(this.Name)).Count() != 0);
        }
        public IEnumerable<Signal> MappedRxSignals()
        {
            return this.parent.Signals.Where(o => o.Receivers.Contains(this.Name));
        }
        public IEnumerable<Signal> MappedTxSignals()
        {
            return GetTxMessages().SelectMany(item => item.Signals).ToList();
        }
        public bool AddTxMessage(Message message)
        {
            if (message.Transmitters.Contains(this.Name))
            {
#if DEBUG      
                throw new Exception($"{this.Name}'s Tx Message contains {message.Name}");
#else
                return false;
#endif
            }
            message.AddTransmitter(this.Name);
            return true;
        }
        public bool RemoveTxMessage(Message message)
        {
            if (!message.Transmitters.Contains(this.Name))
            {
#if DEBUG      
                throw new Exception($"{this.Name}'s Tx Message do not contains {message.Name}");
#else
                return false;
#endif
            }
            message.RemoveTransmitter(this.Name);
            return true;
        }
        public bool AddRxSignal(Signal signal)
        {
            if (signal.Receivers.Contains(this.Name))
            {
#if DEBUG
                throw new Exception($"{this.Name}'s Rx Signal contains {signal.Name}");
#else
                return false;
#endif
            }
            signal.AddReceiver(this.Name);
            return true;
        }
        public bool RemoveRxSignal(Signal signal)
        {
            if (!signal.Receivers.Contains(this.Name))
            {
#if DEBUG
                throw new Exception($"{this.Name}'s Rx Signal do not contains {signal.Name}");
#else
                return false;
#endif
            }
            signal.RemoveReceiver(this.Name);
            return true;
        }
        public override void Rename(string name)
        {
            // Node重命名后，相关的报文和信号都要发生变更
            foreach (var item in parent.Messages)
            {
                if (item.Transmitters.Contains(this.Name))
                {
                    item.RemoveTransmitter(this.Name);
                    item.AddTransmitter(name);
                }
            }
            foreach (var item in parent.Signals)
            {
                if (item.Receivers.Contains(this.Name))
                {
                    item.RemoveReceiver(this.Name);
                    item.AddReceiver(name);
                }
            }
            this.name = name;
        }
    }
}