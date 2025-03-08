using DbcLib.Definitions.MessageDefinitions;
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
    internal class AttributeDefineDefConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"BA_DEF_DEF_\s*""([^""]*)""\s*(.*);");
            AttributeDefine item = dbc.AttributeDefines.Where(o => o.Name == match.Groups[1].Value).First();
            if (item is AttributeDefineEnum)
            {
                item.SetDefaultValue(match.Groups[2].Value.Split(new char[2] { '\"', '\"' })[1]);
            }
            else if (item is AttributeDefineFloat)
            {
                item.SetDefaultValue(double.Parse(match.Groups[2].Value));
            }
            else if (item is AttributeDefineInt)
            {
                item.SetDefaultValue(Int64.Parse(match.Groups[2].Value));
            }
            else if (item is AttributeDefineHex)
            {
                item.SetDefaultValue(Int64.Parse(match.Groups[2].Value));
            }
            else // String
            {
                item.SetDefaultValue(match.Groups[2].Value.Split(new char[2] { '\"', '\"' })[1]);
            }
            DbcHelper.UpdateAttributeValueDefault(dbc, item);
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dbc.AttributeDefines)
            {
                if (item is AttributeDefineEnum || item is AttributeDefineString)
                {
                    sb.AppendLine($"BA_DEF_DEF_ \"{item.Name}\" \"{item.DefaultValue}\";");
                }
                else
                {
                    sb.AppendLine($"BA_DEF_DEF_ \"{item.Name}\" {item.DefaultValue};");
                }
            }
            return sb.ToString();
        }
    }
}