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
    internal class SymbolConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            string symbol = null;
            do
            {
                symbol = parserHelper.GetLine();
                if (string.IsNullOrEmpty(symbol))
                {
                    break;
                }
                dbc.Symbol.Add(Regex.Match(symbol, @"(\w+)").Groups[1].Value);
            } while (symbol != null);
        }
        public string Serialize(Dbc dbc)
        {
            string content = null;
            content = "NS_ :" + Environment.NewLine;
            foreach (var item in dbc.Symbol)
            {
                content += $"\t{item}" + Environment.NewLine;
            }
            return content;
        }
    }
}