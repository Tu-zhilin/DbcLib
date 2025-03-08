using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal class VersionConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Regex regex = new Regex(@"VERSION\s*""([^\""]*)""");
            Match match = regex.Match(line);
            dbc.Version = match.Groups[1].Value;
        }
        public string Serialize(Dbc dbc)
        {
            return $"VERSION \"{dbc.Version}\"";
        }
    }
}