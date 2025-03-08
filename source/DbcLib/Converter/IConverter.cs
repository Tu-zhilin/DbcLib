using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal interface IConverter
    {
        void Deserialize(Dbc dbc, string line, ParserHelper parserHelper);
        string Serialize(Dbc dbc);
    }
}