using DbcLib.Definitions.ValueTableDefinitions;
using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.MessageDefinitions
{
    public enum ByteOrder
    {
        MSB,
        LSB
    }
    public enum DataType
    {
        UNSIGNED,
        SIGNED
    }
    public class Signal : DbcObject
    {
        public string Multiplexing { get; set; }
        public int StartBit { get; set; }
        public int Size { get; set; }
        private ByteOrder byteOrder = ByteOrder.LSB;
        public ByteOrder ByteOrder
        {
            get { return byteOrder; }
        }
        public DataType DataType { get; set; }
        public double Factor { get; set; }
        public double Offset { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public string Unit { get; set; }
        private List<string> receivers;
        public IReadOnlyList<string> Receivers => new ReadOnlyCollection<string>(receivers);
        private ValueTable valueTable;
        public ValueTable ValueTable
        {
            get { return valueTable; }
        }
        internal List<string> messages;

        public Signal(string name)
        {
            this.name = name;
            receivers = new List<string>();
            messages = new List<string>();
        }
        public void CreateValueTable()
        {
            this.valueTable = new ValueTable(this);
        }
        internal bool AddReceiver(string name)
        {
            if (receivers.Contains(name))
            {
#if DEBUG
                throw new Exception($"{this.Name}'Receivers contains {name}");
#else
                return false;
#endif
            }
            receivers.Add(name);
            return true;
        }
        internal bool RemoveReceiver(string name)
        {
            if (!receivers.Contains(name))
            {
#if DEBUG
                throw new Exception($"{this.Name}'Receivers do not contains {name}");
#else
                return false;
#endif
            }
            receivers.Remove(name);
            return true;
        }
        internal void SetByteOrder(ByteOrder byteOrder)
        {
            this.byteOrder = byteOrder;
            //            if (byteOrder == this.ByteOrder)
            //            {
            //#if DEBUG
            //                throw new Exception($"Byte Order is equal");
            //#else
            //                return;
            //#endif
            //            }
            //            if (byteOrder == ByteOrder.MSB)
            //            {
            //                this.StartBit = DbcHelper.IntelToMotolora(this.StartBit, this.Size);
            //            }
            //            else
            //            {
            //                this.StartBit = DbcHelper.MotoloraToIntel(this.StartBit, this.Size);
            //            }
            //            this.byteOrder = byteOrder;
        }
    }
}