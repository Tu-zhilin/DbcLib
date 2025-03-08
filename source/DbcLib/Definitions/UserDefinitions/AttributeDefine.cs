using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Definitions.UserDefinitions
{
    public enum AttributeValueType
    {
        ENUM,
        STRING,
        FLOAT,
        INT,
        HEX
    }
    public abstract class AttributeDefine
    {
        protected Dbc parent;
        private string name;
        public string Name
        {
            get { return name; }
        }
        private ObjectType objectType;
        public ObjectType ObjectType
        {
            get { return objectType; }
        }
        private object defaultValue;
        public object DefaultValue
        {
            get { return defaultValue; }
        }
        public AttributeDefine(Dbc dbc, string name, ObjectType objectType)
        {
            this.parent = dbc;
            this.name = name;
            this.objectType = objectType;
        }
        internal abstract bool SetValueValid(object value);
        /// <summary>
        /// 属性定义的名称变更
        /// </summary>
        /// <param name="name"></param>
        public void Rename(string name)
        {
            switch (this.objectType)
            {
                case ObjectType.NETWORK:
                    foreach (var item in this.parent.AttributeValues.Where(o => o.Name == this.Name))
                    {
                        item.Rename(name);
                    }
                    break;
                case ObjectType.NODE:
                    foreach (var node in this.parent.Nodes)
                    {
                        foreach (var item in node.AttributeValues.Where(o => o.Name == this.Name))
                        {
                            item.Rename(name);
                        }
                    }
                    break;
                case ObjectType.MESSAGE:
                    foreach (var message in this.parent.Messages)
                    {
                        foreach (var item in message.AttributeValues.Where(o => o.Name == this.Name))
                        {
                            item.Rename(name);
                        }
                    }
                    break;
                case ObjectType.SIGNAL:
                    foreach (var signal in this.parent.Signals)
                    {
                        foreach (var item in signal.AttributeValues.Where(o => o.Name == this.Name))
                        {
                            item.Rename(name);
                        }
                    }
                    break;
                case ObjectType.ENVIRONMENT:
#if DEBUG
                    throw new Exception($"object type ({this.objectType}) not supported yet");
#else
                    break;
#endif
                default:
#if DEBUG
                    throw new Exception($"unexpected Object Type ({this.objectType}) have occurred");
#else
                    break;
#endif
            }
            this.name = name;
        }
        /// <summary>
        /// 设置属性定义的所属对象类型
        /// </summary>
        /// <param name="objectType"></param>
        public void SetObjectType(ObjectType objectType)
        {
            // TDODO: 根据对象类型，变更所有关联项
        }
        /// <summary>
        /// 创建属性
        /// </summary>
        /// <returns></returns>
        public AttributeValue CreateAttributeValue()
        {
            return new AttributeValue(this);
        }
        /// <summary>
        /// 设置属性定义的默认值
        /// </summary>
        /// <param name="value"></param>
        public bool SetDefaultValue(object value)
        {
            if (!SetValueValid(value))
            {
                return false;
            }
            this.defaultValue = value;
            return true;
        }
        public static AttributeDefine CreateAttributeDefine(Dbc parent, string name, ObjectType objectType, AttributeValueType valueType)
        {
            switch (valueType)
            {
                case AttributeValueType.FLOAT:
                    return new AttributeDefineFloat(parent, name, objectType);
                case AttributeValueType.HEX:
                    return new AttributeDefineHex(parent, name, objectType);
                case AttributeValueType.STRING:
                    return new AttributeDefineString(parent, name, objectType);
                case AttributeValueType.INT:
                    return new AttributeDefineInt(parent, name, objectType);
                case AttributeValueType.ENUM:
                    return new AttributeDefineEnum(parent, name, objectType);
                default:
#if DEBUG
                    throw new Exception($"unexpected Attribute Value Type ({valueType}) have occurred");
#else
                    return null;
#endif
            }
        }
    }
}