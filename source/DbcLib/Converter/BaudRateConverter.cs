using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal class BaudRateConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            // nothing to do...
        }
        public string Serialize(Dbc dbc)
        {
            return "BS_: ";
        }
    }
}