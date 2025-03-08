using DbcLib.Definitions.NodeDefinitions;
using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal class NodeConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            MatchCollection matches = Regex.Matches(line, @"(\w+)");
            foreach (var item in matches.Cast<Match>().ToList().Skip(1).Select(o => o.Groups[1].Value))
            {
                dbc.CreateNode(item);
            }
        }
        public string Serialize(Dbc dbc)
        {
            string content = null;
            content = "BU_:";
            foreach (var item in dbc.Nodes)
            {
                content += $" {item.Name}";
            }
            return content;
        }
    }
}