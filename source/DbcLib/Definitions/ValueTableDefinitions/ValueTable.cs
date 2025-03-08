using DbcLib.Definitions.MessageDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.ValueTableDefinitions
{
    public class ValueTable
    {
        private Signal parent;
        private Dictionary<int, string> descriptions = new Dictionary<int, string>();
        public IReadOnlyDictionary<int, string> Descriptions => new ReadOnlyDictionary<int, string>(descriptions);
        public ValueTable(Signal signal)
        {
            this.parent = signal;
            descriptions = new Dictionary<int, string>();
        }
        public bool Add(int value, string description)
        {
            if (descriptions.ContainsKey(value))
            {
#if DEBUG
                throw new Exception($"{parent.Name}'s value table contains {value}");
#else
                return false;
#endif
            }
            descriptions.Add(value, description);
            return true;
        }
        public bool Remove(int value)
        {
            if (!descriptions.ContainsKey(value))
            {
#if DEBUG
                throw new Exception($"{parent.Name}'s value table do not contains {value}");
#else
                return false;
#endif
            }
            descriptions.Remove(value);
            return true;
        }
        public bool Change(int value, string description)
        {
            if (!descriptions.ContainsKey(value))
            {
#if DEBUG
                throw new Exception($"{parent.Name}'s value table do not contains {value}");
#else
                return false;
#endif
            }
            descriptions[value] = description;
            return true;
        }
    }
}