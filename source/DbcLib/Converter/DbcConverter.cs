using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    public class DbcConvert
    {
        private static Dictionary<string, IConverter> Converters = new Dictionary<string, IConverter>()
        {
            ["VERSION"] = new VersionConverter(),
            ["NS_"] = new SymbolConverter(),
            ["BS_"] = new BaudRateConverter(),
            ["BU_"] = new NodeConverter(),
            ["BO_"] = new MessageConverter(),
            ["BO_TX_BU_"] = new MultipleTxMessageConverter(),
            ["CM_"] = new CommentConverter(),
            ["BA_DEF_"] = new AttributeDefineConverter(),
            ["BA_DEF_DEF_"] = new AttributeDefineDefConverter(),
            ["BA_"] = new AttributeValueConverter(),
            ["VAL_"] = new ValueTableConverter(),
            ["SIG_GROUP_"] = new SignalGroupConverter(),
        };
        public static Dbc Deserialize(string path, Encoding encoding)
        {
            string line = null;
            Dbc dbc = new Dbc();
            using (ParserHelper parserHelper = new ParserHelper(path, encoding))
            {
                Match match = null;
                do
                {
                    if (string.IsNullOrEmpty(line = parserHelper.GetLine()))
                    {
                        continue;
                    }
                    match = Regex.Match(line, @"(\w+)");
                    if (match.Success != true)
                    {
                        continue;
                    }
                    if (!Converters.ContainsKey(match.Groups[1].Value))
                    {
                        parserHelper.Exception($"unrecognized identifier {match.Groups[1].Value}");
                    }
                    Converters[match.Groups[1].Value].Deserialize(dbc, line, parserHelper);
                } while (line != null);
            }
            return dbc;
        }
        public static string Serialize(Dbc dbc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Converters)
            {
                sb.AppendLine(item.Value.Serialize(dbc) + Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}