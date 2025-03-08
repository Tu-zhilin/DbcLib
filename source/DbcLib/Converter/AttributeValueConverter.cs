using DbcLib.Definitions.UserDefinitions;
using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal interface IAttributeValueConverter
    {
        void Deserialize(Dbc dbc, string name, string line);
        string Serialize(Dbc dbc);
    }
    internal abstract class AttributeValueBaseConverter : IAttributeValueConverter
    {
        public abstract void Deserialize(Dbc dbc, string name, string line);
        public abstract string Serialize(Dbc dbc);
        internal string GetAttributeValue(Dbc dbc, AttributeValue attributeValue)
        {
            var attributeDefine = attributeValue.Parent;
            if (attributeDefine is AttributeDefineEnum)
            {
                return $"{(attributeDefine as AttributeDefineEnum).EnumValues.IndexOf((string)attributeValue.Value)};";
            }
            else if (attributeDefine is AttributeDefineFloat)
            {
                return $"{attributeValue.Value};";
            }
            else if (attributeDefine is AttributeDefineInt)
            {
                return $"{attributeValue.Value};";
            }
            else if (attributeDefine is AttributeDefineHex)
            {
                return $"{attributeValue.Value};";
            }
            else // String
            {
                return $"\"{attributeValue.Value}\";";
            }
        }
        internal void SetAttributeValue(Dbc dbc, AttributeValue attributeValue, string value)
        {
            var attributeDefine = attributeValue.Parent;
            if (attributeDefine is AttributeDefineEnum)
            {
                attributeValue.SetValue((attributeDefine as AttributeDefineEnum).EnumValues[int.Parse(value)]);
            }
            else if (attributeDefine is AttributeDefineFloat)
            {
                attributeValue.SetValue(double.Parse(value));
            }
            else if (attributeDefine is AttributeDefineInt)
            {
                if (value.Contains("."))
                {
                    attributeValue.SetValue((int)(float.Parse(value)));
                }
                else
                {
                    attributeValue.SetValue(Int64.Parse(value));
                }
            }
            else if (attributeDefine is AttributeDefineHex)
            {
                attributeValue.SetValue(Int64.Parse(value));
            }
            else // String
            {
                attributeValue.SetValue(value);
            }
        }
    }
    internal class AttributeValueNetworkConverter : AttributeValueBaseConverter
    {
        public override void Deserialize(Dbc dbc, string name, string line)
        {
            Match match = Regex.Match(line, $@"BA_\s*\""{name}\""\s*\""?([^\""]*)\""?;");
            var attributeValue = dbc.AttributeValues.Where(o => o.Name == name).First();
            SetAttributeValue(dbc, attributeValue, match.Groups[1].Value);
        }
        public override string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var attribute in dbc.AttributeValues)
            {
                if (attribute.Value == attribute.Parent.DefaultValue)
                {
                    continue;
                }
                stringBuilder.AppendLine($"BA_ \"{attribute.Name}\" {DbcHelper.ObjectTypeMapping(ObjectType.NETWORK)} " + GetAttributeValue(dbc, attribute));
            }
            return stringBuilder.ToString();
        }
    }
    internal class AttributeValueNodeConverter : AttributeValueBaseConverter
    {
        public override void Deserialize(Dbc dbc, string name, string line)
        {
            Match match = Regex.Match(line, @"BU_\s*(\w+)\s*\""?([^\""""]+)\""?;");
            var attributeValue = dbc.Nodes.
                Where(o => o.Name == match.Groups[1].Value).First().AttributeValues.
                Where(o => o.Name == name).First();
            SetAttributeValue(dbc, attributeValue, match.Groups[2].Value);
        }
        public override string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var node in dbc.Nodes)
            {
                foreach (var attribute in node.AttributeValues)
                {
                    if (attribute.Value == attribute.Parent.DefaultValue)
                    {
                        continue;
                    }
                    stringBuilder.AppendLine($"BA_ \"{attribute.Name}\" {DbcHelper.ObjectTypeMapping(ObjectType.NODE)} {node.Name} " + GetAttributeValue(dbc, attribute));
                }
            }
            return stringBuilder.ToString();
        }
    }
    internal class AttributeValueMessageConverter : AttributeValueBaseConverter
    {
        public override void Deserialize(Dbc dbc, string name, string line)
        {
            Match match = Regex.Match(line, @"BO_\s*(\d+)\s*\""?([^\""""]+)\""?;");
            var attributeValue = dbc.Messages.
                Where(o => o.Id == DbcHelper.GetMessageIDFromFile(match.Groups[1].Value)).First().AttributeValues.
                Where(o => o.Name == name).First();
            SetAttributeValue(dbc, attributeValue, match.Groups[2].Value);
        }
        public override string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var message in dbc.Messages)
            {
                foreach (var attribute in message.AttributeValues)
                {
                    if (attribute.Value == attribute.Parent.DefaultValue)
                    {
                        continue;
                    }
                    stringBuilder.AppendLine($"BA_ \"{attribute.Name}\" {DbcHelper.ObjectTypeMapping(ObjectType.MESSAGE)} {DbcHelper.GetMessageIDForFile(message.Id)} " + GetAttributeValue(dbc, attribute));
                }
            }
            return stringBuilder.ToString();
        }
    }
    internal class AttributeValueSignalConverter : AttributeValueBaseConverter
    {
        public override void Deserialize(Dbc dbc, string name, string line)
        {
            Match match = Regex.Match(line, @"SG_\s*(\d+)\s*(\w+)\s*""?([^\""]*)""?;");
            var attributeValue = dbc.Messages.
                Where(o => o.Id == DbcHelper.GetMessageIDFromFile(match.Groups[1].Value)).First().Signals.
                Where(o => o.Name == match.Groups[2].Value).First().AttributeValues.
                Where(o => o.Name == name).First();
            SetAttributeValue(dbc, attributeValue, match.Groups[3].Value);
        }
        public override string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var message in dbc.Messages)
            {
                foreach (var signal in message.Signals)
                {
                    foreach (var attribute in signal.AttributeValues)
                    {
                        if (attribute.Value == attribute.Parent.DefaultValue)
                        {
                            continue;
                        }
                        stringBuilder.AppendLine($"BA_ \"{attribute.Name}\" {DbcHelper.ObjectTypeMapping(ObjectType.SIGNAL)} {DbcHelper.GetMessageIDForFile(message.Id)} {signal.Name} " + GetAttributeValue(dbc, attribute));
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
    internal class AttributeValueConverter : IConverter
    {
        private Dictionary<ObjectType, IAttributeValueConverter> converters = new Dictionary<ObjectType, IAttributeValueConverter>()
        {
            [ObjectType.NETWORK] = new AttributeValueNetworkConverter(),
            [ObjectType.NODE] = new AttributeValueNodeConverter(),
            [ObjectType.MESSAGE] = new AttributeValueMessageConverter(),
            [ObjectType.SIGNAL] = new AttributeValueSignalConverter(),
        };
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            string next = null;
            while (!line.EndsWith(";"))
            {
                line += (next = parserHelper.GetLine());
                if (string.IsNullOrEmpty(next))
                {
                    parserHelper.Exception("did not read \";\" ending");
                    return;
                }
            }
            Match match = Regex.Match(line, @"BA_\s*""([^\""""]+)""\s*(\w+)?");
            string attributeName = match.Groups[1].Value;
            ObjectType objectType = DbcHelper.ObjectTypeMapping(match.Groups[2].Value);
            converters[objectType].Deserialize(dbc, attributeName, line);
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in converters)
            {
                stringBuilder.Append(item.Value.Serialize(dbc));
            }
            return stringBuilder.ToString();
        }
    }
}