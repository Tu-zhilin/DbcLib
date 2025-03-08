using DbcLib.Definitions.ValueTableDefinitions;
using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DbcLib.Converter
{
    internal class ValueTableConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"VAL_\s*(\d+)\s*(\w+)");
            var messageId = DbcHelper.GetMessageIDFromFile(match.Groups[1].Value);
            var signalName = match.Groups[2].Value;
            match = Regex.Match(line, $@"{match.Groups[2].Value}\s*(.*);");
            string values = match.Groups[1].Value;
            MatchCollection matches_value = Regex.Matches(values, @"(\d+)\s+\""");
            MatchCollection matches_description = Regex.Matches(values, @"\""([^\""]*)\""");
            var signal = dbc.Signals.Where(o => o.Name == signalName).First();
            signal.CreateValueTable();
            if (matches_value.Count != matches_description.Count)
            {
                parserHelper.Exception("The number of values and descriptions does not match");
                return;
            }
            matches_value.Cast<Match>().
                Zip(matches_description.Cast<Match>(),
                (value, description) =>
                {
                    signal.ValueTable.Add(int.Parse(value.Groups[1].Value), description.Groups[1].Value);
                    return 0; // 无实际意义，仅为了满足 Zip 的返回值
                }).ToList();  // 触发 Zip 的执行
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<ValueTable> valueDescriptions = new List<ValueTable>();
            foreach (var message in dbc.Messages)
            {
                foreach (var signal in message.Signals)
                {
                    if (signal.ValueTable == null)
                    {
                        continue;
                    }
                    string content = null;
                    content = $"VAL_ {DbcHelper.GetMessageIDForFile(message.Id)} {signal.Name} ";
                    foreach (var item in signal.ValueTable.Descriptions)
                    {
                        content += $"{item.Key} \"{item.Value}\" ";
                    }
                    stringBuilder.AppendLine(content + ";");
                }
            }
            return stringBuilder.ToString();
        }
    }
}