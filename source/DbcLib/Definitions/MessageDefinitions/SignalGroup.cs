using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.MessageDefinitions
{
    public class SignalGroup
    {
        private int id = 1;
        public int Id
        {
            get { return id; }
        }
        public string Name { get; set; }
        private List<Signal> signals;
        public IReadOnlyList<Signal> Signals => new ReadOnlyCollection<Signal>(signals);
        public SignalGroup(int repetition, string name)
        {
            this.id = repetition;
            signals = new List<Signal>();
            this.Name = name;
        }
        internal void SetId(int id)
        {
            this.id = id;
        }
        internal bool AddSignal(Signal signal)
        {
            if (signals.Contains(signal))
            {
#if DEBUG
                throw new Exception($"{this.Name} contains {signal}");
#else
                return false;
#endif
            }
            signals.Add(signal);
            return true;
        }
        internal bool RemoveSignal(Signal signal)
        {
            if (!signals.Contains(signal))
            {
#if DEBUG
                throw new Exception($"{this.Name} do not contains {signal}");
#else
                return false;
#endif
            }
            signals.Remove(signal);
            return true;
        }
    }
}