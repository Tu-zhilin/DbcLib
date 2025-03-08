using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.UserDefinitions
{
    public class AttributeDefineFloat : AttributeDefine
    {
        private double max;
        public double Max
        {
            get { return max; }
        }
        private double min;
        public double Min
        {
            get { return min; }
        }
        public AttributeDefineFloat(Dbc dbc, string name, ObjectType objectType, double max = 0, double min = 0) : base(dbc, name, objectType)
        {
            this.max = max;
            this.min = min;
        }
        internal override bool SetValueValid(object value)
        {
            if (!value.GetType().Equals(typeof(double)))
            {
#if DEBUG
                throw new ArgumentException($"{this.Name} value type must be double");
#else
                return false;
#endif
            }
            //            if (!((double)value >= min && (double)value <= max))
            //            {
            //#if DEBUG
            //                throw new ArgumentException($"{this.Name} value must be greater than {min} and less than {max}");
            //#else
            //                return false;
            //#endif
            //            }
            return true;
        }
        public bool SetMaxValue(double value)
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
        public bool SetMinValue(double value)
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