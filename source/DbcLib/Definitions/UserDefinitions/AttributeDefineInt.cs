using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.UserDefinitions
{
    public class AttributeDefineInt : AttributeDefine
    {
        private Int64 max;
        public Int64 Max
        {
            get { return max; }
        }
        private Int64 min;
        public Int64 Min
        {
            get { return min; }
        }
        public AttributeDefineInt(Dbc dbc, string name, ObjectType objectType, Int64 max = 0, Int64 min = 0) : base(dbc, name, objectType)
        {
            this.max = max;
            this.min = min;
        }
        internal override bool SetValueValid(object value)
        {
            if (!value.GetType().Equals(typeof(Int64)))
            {
#if DEBUG
                throw new ArgumentException($"{this.Name} value type must be Int64");
#else
                return false;
#endif
            }
            //            Int64 intValue = (Int64)value;
            //            if (intValue < min || intValue > max)
            //            {
            //#if DEBUG
            //                throw new ArgumentException($"{this.Name} value must be greater than {min} and less than {max}");
            //#else
            //                return false;
            //#endif
            //            }
            return true;
        }
        public bool SetMaxValue(Int64 value)
        {
            if (value < min)
            {
#if DEBUG
                throw new ArgumentException($"{this.Name} max value must greater than {min}");
#else
                return false;
#endif
            }
            this.max = value;
            return true;
        }
        public bool SetMinValue(Int64 value)
        {
            if (value > max)
            {
#if DEBUG
                throw new ArgumentException($"{this.Name} min value must less than {max}");
#else
                return false;
#endif
            }
            this.min = value;
            return true;
        }
    }
}