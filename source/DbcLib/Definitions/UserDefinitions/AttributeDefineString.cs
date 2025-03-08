using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.UserDefinitions
{
    public class AttributeDefineString : AttributeDefine
    {
        public AttributeDefineString(Dbc dbc, string name, ObjectType objectType) : base(dbc, name, objectType)
        {
        }
        internal override bool SetValueValid(object value)
        {
            if (value.GetType().Equals(typeof(string).FullName))
            {
#if DEBUG
                throw new ArgumentException($"{this.Name} value type must be string");
#else
                return false;
#endif
            }
            return true;
        }
    }
}