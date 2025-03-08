using DbcLib.Definitions.MessageDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.UserDefinitions
{
    public class AttributeDefineEnum : AttributeDefine
    {
        private List<string> enumValues;
        public List<string> EnumValues
        {
            get { return enumValues; }
        }
        public AttributeDefineEnum(Dbc dbc, string name, ObjectType objectType) : base(dbc, name, objectType)
        {
            this.enumValues = new List<string>();
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
            //            if (!EnumValues.Contains((string)value))
            //            {
            //#if DEBUG
            //                throw new Exception($"{this.Name} do not contains {value}");
            //#else
            //                return false;
            //#endif
            //            }
            return true;
        }
        public bool AddEnumValue(string value)
        {
            // 枚举属性的值是可以重名的
            this.enumValues.Add(value);
            return true;
        }
        public bool RemoveEnumValue(string value)
        {
            if (!EnumValues.Contains(value))
            {
#if DEBUG
                throw new Exception($"{this.Name} do not contains {value}");
#else
                return false;
#endif
            }
            if ((string)this.DefaultValue == value)
            {
                SetDefaultValue((object)enumValues.FirstOrDefault());
            }
            enumValues.Remove(value);
            switch (this.ObjectType)
            {
                case ObjectType.NETWORK:
                    foreach (var item in this.parent.AttributeValues.Where(o => (string)o.Value == value))
                    {
                        item.SetValue(this.DefaultValue);
                    }
                    break;
                case ObjectType.NODE:
                    foreach (var node in this.parent.Nodes)
                    {
                        foreach (var item in node.AttributeValues)
                        {
                            item.SetValue(this.DefaultValue);
                        }
                    }
                    break;
                case ObjectType.MESSAGE:
                    foreach (var message in this.parent.Messages)
                    {
                        foreach (var item in message.AttributeValues)
                        {
                            item.SetValue(this.DefaultValue);
                        }
                    }
                    break;
                case ObjectType.SIGNAL:
                    foreach (var signal in this.parent.Signals)
                    {
                        foreach (var item in signal.AttributeValues)
                        {
                            item.SetValue(this.DefaultValue);
                        }
                    }
                    break;
                case ObjectType.ENVIRONMENT:
#if DEBUG
                    throw new Exception($"object type ({this.ObjectType}) not supported yet");
#else
                    break;
#endif
                default:
#if DEBUG
                    throw new Exception($"unexpected Object Type ({this.ObjectType}) have occurred");
#else
                    break;
#endif
            }
            return true;
        }
    }
}