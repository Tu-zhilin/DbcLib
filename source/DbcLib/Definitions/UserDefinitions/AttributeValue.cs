using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.UserDefinitions
{
    public class AttributeValue
    {
        private AttributeDefine parent;
        public AttributeDefine Parent { get { return parent; } }
        private object value;
        public object Value
        {
            get { return value; }
        }
        private string name;
        public string Name
        {
            get { return name; }
        }
        public AttributeValue(AttributeDefine attributeDefine)
        {
            this.parent = attributeDefine;
            this.name = attributeDefine.Name;
            this.value = attributeDefine.DefaultValue;
        }
        internal void Rename(string name)
        {
            this.name = name;
        }
        public bool SetValue(object value)
        {
            if (!parent.SetValueValid(value))
            {
                return false;
            }
            this.value = value;
            return true;
        }
    }
}