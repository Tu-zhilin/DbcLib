using DbcLib.Definitions.UserDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib
{
    public enum ObjectType
    {
        NETWORK,
        NODE,
        MESSAGE,
        SIGNAL,
        ENVIRONMENT,
    }
    public abstract class DbcObject
    {
        protected string name;
        public virtual string Name
        {
            get { return name; }
        }
        public string Comment { get; set; }
        private List<AttributeValue> attributeValues;
        public IReadOnlyList<AttributeValue> AttributeValues => new ReadOnlyCollection<AttributeValue>(attributeValues);
        public DbcObject()
        {
            attributeValues = new List<AttributeValue>();
        }
        public virtual void Rename(string name)
        {
            this.name = name;
        }
        internal bool AddAttributeValue(AttributeValue attributeValue)
        {
            if (this.attributeValues.Contains(attributeValue))
            {
#if DEBUG
                throw new Exception($"{this.Name}'s contains {attributeValue.Name} AttributeValue");
#else
                return false;
#endif
            }
            this.attributeValues.Add(attributeValue);
            return true;
        }
        internal bool RemoveAttributeValue(string name)
        {
            var attributeValue = this.attributeValues.Where(o => o.Name == name).ToArray();
            if (attributeValue.Count() == 0)
            {
#if DEBUG
                throw new Exception($"{this.Name}'s do not contains {name} AttributeValue");
#else
                return false;
#endif
            }
            for (int i = 0; i < attributeValue.Count(); i++)
            {
                this.attributeValues.Remove(attributeValue[i]);
            }
            return true;
        }
    }
}