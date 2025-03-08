using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal class MultipleTxMessageConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"BO_TX_BU_\s*(\w*)\s*:\s*(.*);");
            UInt32 id = DbcHelper.GetMessageIDFromFile(match.Groups[1].Value);
            var message = dbc.Messages.Where(o => o.Id == id).First();
            message.ClearTransmitter();
            var transmitter = match.Groups[2].Value.Split(',');
            transmitter.Reverse();
            foreach (var item in transmitter)
            {
                message.AddTransmitter(item);
            }
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var message in dbc.Messages.Where(o => o.Transmitters.Count > 1))
            {
                string content = $"BO_TX_BU_ {DbcHelper.GetMessageIDForFile(message.Id)} : ";
                for (int i = 0; i < message.Transmitters.Count; i++)
                {
                    content += message.Transmitters[i];
                    if (i == message.Transmitters.Count - 1)
                    {
                        content += ";";
                    }
                    else
                    {
                        content += ",";
                    }
                }
                stringBuilder.AppendLine(content);
            }
            return stringBuilder.ToString();
        }
    }
}