using DbcLib.Definitions.MessageDefinitions;
using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal class SignalGroupConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"SIG_GROUP_\s*(\d+)\s*(\w+)\s*(\d+)\s*:(.*);");
            var messageId = DbcHelper.GetMessageIDFromFile(match.Groups[1].Value);
            var name = match.Groups[2].Value;
            var id = int.Parse(match.Groups[3].Value);
            string remain = match.Groups[4].Value;
            MatchCollection matches = Regex.Matches(remain, @"(\w+)");
            var message = dbc.Messages.Where(o => o.Id == messageId).First();
            var signalGroup = message.CreateSignalGroup(name);
            signalGroup.SetId(id);
            foreach (var item in matches.Cast<Match>().ToList().Select(o => o.Groups[1].Value))
            {
                signalGroup.AddSignal(item);
            }
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var message in dbc.Messages)
            {
                foreach (var signalGroup in message.SignalGroups)
                {
                    if (signalGroup.Signals.Count == 0)
                    {
                        continue;
                    }
                    string content = null;
                    content = $"SIG_GROUP_ {DbcHelper.GetMessageIDForFile(message.Id)} {signalGroup.Name} {signalGroup.Id} : ";
                    for (int i = 0; i < signalGroup.Signals.Count; i++)
                    {
                        content += $"{signalGroup.Signals[i]}";
                        if (i < signalGroup.Signals.Count - 1)
                        {
                            content += " ";
                        }
                    }
                    sb.AppendLine(content + ";");
                }
            }
            return sb.ToString();
        }
    }
}