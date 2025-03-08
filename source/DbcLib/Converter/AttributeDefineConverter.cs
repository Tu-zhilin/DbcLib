using DbcLib.Definitions.MessageDefinitions;
using DbcLib.Definitions.UserDefinitions;
using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal interface IAttributeDefineConverter
    {
        void Deserialize(Dbc dbc, string name, ObjectType objectType, string line);
        string Serialize(AttributeDefine attributeDefine);
    }
    internal class AttributeDefineStringConverter : IAttributeDefineConverter
    {
        public void Deserialize(Dbc dbc, string name, ObjectType objectType, string line)
        {
            dbc.CreateAttributeDefine(name, objectType, AttributeValueType.STRING);
        }
        public string Serialize(AttributeDefine attributeDefine)
        {
            return $"BA_DEF_ {DbcHelper.ObjectTypeMapping(attributeDefine.ObjectType)} \"{attributeDefine.Name}\" STRING ;";
        }
    }
    internal class AttributeDefineIntConverter : IAttributeDefineConverter
    {
        public void Deserialize(Dbc dbc, string name, ObjectType objectType, string line)
        {
            var attributeDefine = (AttributeDefineInt)dbc.CreateAttributeDefine(name, objectType, AttributeValueType.INT);
            Match match = Regex.Match(line, @"(\d+)\s*(\d+)");
            attributeDefine.SetMaxValue(Int64.Parse(match.Groups[2].Value));
            attributeDefine.SetMinValue(Int64.Parse(match.Groups[1].Value));
        }
        public string Serialize(AttributeDefine attributeDefine)
        {
            return $"BA_DEF_ {DbcHelper.ObjectTypeMapping(attributeDefine.ObjectType)} \"{attributeDefine.Name}\" INT {((AttributeDefineInt)attributeDefine).Min} {((AttributeDefineInt)attributeDefine).Max};";
        }
    }
    internal class AttributeDefineHexConverter : IAttributeDefineConverter
    {
        public void Deserialize(Dbc dbc, string name, ObjectType objectType, string line)
        {
            var attributeDefine = (AttributeDefineHex)dbc.CreateAttributeDefine(name, objectType, AttributeValueType.HEX);
            Match match = Regex.Match(line, @"(\d+)\s*(\d+)");
            attributeDefine.SetMaxValue(Int64.Parse(match.Groups[2].Value));
            attributeDefine.SetMinValue(Int64.Parse(match.Groups[1].Value));
        }
        public string Serialize(AttributeDefine attributeDefine)
        {
            return $"BA_DEF_ {DbcHelper.ObjectTypeMapping(attributeDefine.ObjectType)} \"{attributeDefine.Name}\" HEX {((AttributeDefineHex)attributeDefine).Min} {((AttributeDefineHex)attributeDefine).Max};";
        }
    }
    internal class AttributeDefineEnumConverter : IAttributeDefineConverter
    {
        public void Deserialize(Dbc dbc, string name, ObjectType objectType, string line)
        {
            var attributeDefine = (AttributeDefineEnum)dbc.CreateAttributeDefine(name, objectType, AttributeValueType.ENUM);
            MatchCollection matchCollection = Regex.Matches(line, @"\""([^\""]+)\""");
            foreach (var item in matchCollection.Cast<Match>().ToList().Select(o => o.Groups[1].Value))
            {
                attributeDefine.AddEnumValue(item);
            }
        }
        public string Serialize(AttributeDefine attributeDefine)
        {
            string content = null;
            var _attribute = (AttributeDefineEnum)attributeDefine;
            content = $"BA_DEF_ {DbcHelper.ObjectTypeMapping(_attribute.ObjectType)} \"{_attribute.Name}\" ENUM ";
            for (int i = 0; i < _attribute.EnumValues.Count; i++)
            {
                content += $"\"{_attribute.EnumValues[i]}\"";
                if (i < _attribute.EnumValues.Count - 1)
                {
                    content += ",";
                }
            }
            content += ";";
            return content;
        }
    }
    internal class AttributeDefineFloatConverter : IAttributeDefineConverter
    {
        public void Deserialize(Dbc dbc, string name, ObjectType objectType, string line)
        {
            var attributeDefine = (AttributeDefineFloat)dbc.CreateAttributeDefine(name, objectType, AttributeValueType.FLOAT);
            Match match = Regex.Match(line, @"(-?\d+\.?\d*)\s*(-?\d+\.?\d*)");
            attributeDefine.SetMaxValue(double.Parse(match.Groups[2].Value));
            attributeDefine.SetMinValue(double.Parse(match.Groups[1].Value));
        }
        public string Serialize(AttributeDefine attributeDefine)
        {
            return $"BA_DEF_ {DbcHelper.ObjectTypeMapping(attributeDefine.ObjectType)} \"{attributeDefine.Name}\" FLOAT {((AttributeDefineHex)attributeDefine).Min} {((AttributeDefineHex)attributeDefine).Max};";
        }
    }
    internal class AttributeDefineConverter : IConverter
    {
        private Dictionary<string, IAttributeDefineConverter> converters = new Dictionary<string, IAttributeDefineConverter>()
        {
            ["INT"] = new AttributeDefineIntConverter(),
            ["STRING"] = new AttributeDefineStringConverter(),
            ["ENUM"] = new AttributeDefineEnumConverter(),
            ["HEX"] = new AttributeDefineHexConverter(),
            ["FLOAT"] = new AttributeDefineFloatConverter(),
        };
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"BA_DEF_\s*(\w+)");
            ObjectType objectType = ObjectType.NETWORK;
            if (match.Success == true)
            {
                switch (match.Groups[1].Value)
                {
                    case "SG_":
                        objectType = ObjectType.SIGNAL;
                        break;
                    case "BU_":
                        objectType = ObjectType.NODE;
                        break;
                    case "BO_":
                        objectType = ObjectType.MESSAGE;
                        break;
                    default:
                        parserHelper.Exception($"unrecognize type {match.Groups[1].Value}\r\n");
                        return;
                }
            }
            else if (match.Groups.Count == 1)
            {
                objectType = ObjectType.NETWORK;
            }
            match = Regex.Match(line, @"\s*""([^""]*)""\s*(\w+)\s*(.*);");
            converters[match.Groups[2].Value].Deserialize(dbc, match.Groups[1].Value, objectType, match.Groups[3].Value);
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in dbc.AttributeDefines)
            {
                if (item is AttributeDefineEnum)
                {
                    stringBuilder.AppendLine(converters["ENUM"].Serialize(item));
                }
                else if (item is AttributeDefineFloat)
                {
                    stringBuilder.AppendLine(converters["FLOAT"].Serialize(item));
                }
                else if (item is AttributeDefineInt)
                {
                    stringBuilder.AppendLine(converters["INT"].Serialize(item));
                }
                else if (item is AttributeDefineHex)
                {
                    stringBuilder.AppendLine(converters["HEX"].Serialize(item));
                }
                else // String
                {
                    stringBuilder.AppendLine(converters["STRING"].Serialize(item));
                }
            }
            return stringBuilder.ToString();
        }
    }
}