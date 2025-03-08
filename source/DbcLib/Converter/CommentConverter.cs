using DbcLib.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace DbcLib.Converter
{
    internal class CommentNetworkConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"\""([^\""]*)\""");
            dbc.Comment = match.Groups[1].Value;
        }
        public string Serialize(Dbc dbc)
        {
            if (!string.IsNullOrEmpty(dbc.Comment))
            {
                return $"CM_ \"{dbc.Comment}\";";
            }
            return null;
        }
    }
    internal class CommentNodeConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"CM_ BU_\s*(\w+)\s*\""?([^\""]+)\""?");
            var name = match.Groups[1].Value;
            var comment = match.Groups[2].Value;
            dbc.Nodes.Where(o => o.Name == name).First().Comment = comment;
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in dbc.Nodes)
            {
                if (!string.IsNullOrEmpty(item.Comment))
                {
                    stringBuilder.AppendLine($"CM_ BU_ {item.Name} \"{item.Comment}\";");
                }
            }
            return stringBuilder.ToString();
        }
    }
    internal class CommentMessageConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"CM_\s*(\w+)\s*(\d+)\s*\""?([^\""]+)\""?");
            var id = DbcHelper.GetMessageIDFromFile(match.Groups[2].Value);
            var comment = match.Groups[3].Value;
            dbc.Messages.Where(o => o.Id == id).First().Comment = comment;
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in dbc.Messages)
            {
                if (!string.IsNullOrEmpty(item.Comment))
                {
                    stringBuilder.AppendLine($"CM_ BO_ {DbcHelper.GetMessageIDForFile(item.Id)} \"{item.Comment}\";");
                }
            }
            return stringBuilder.ToString();
        }
    }
    internal class CommentSignalConverter : IConverter
    {
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match = Regex.Match(line, @"CM_\s*(\w+)\s*(\d+)\s*(\w+)\s*\W\""?([^\""]+)\""?");
            var messageId = DbcHelper.GetMessageIDFromFile(match.Groups[2].Value);
            var signalNmae = match.Groups[3].Value;
            var comment = match.Groups[4].Value;
            dbc.Messages.Where(o => o.Id == messageId).First().Signals.Where(o => o.Name == signalNmae).First().Comment = comment;
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var message in dbc.Messages)
            {
                foreach (var signal in message.Signals)
                {
                    if (!string.IsNullOrEmpty(signal.Comment))
                    {
                        stringBuilder.AppendLine($"CM_ SG_ {DbcHelper.GetMessageIDForFile(message.Id)} {signal.Name} \"{signal.Comment}\";");
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
    internal class CommentConverter : IConverter
    {
        private Dictionary<ObjectType, IConverter> converters = new Dictionary<ObjectType, IConverter>()
        {
            [ObjectType.NETWORK] = new CommentNetworkConverter(),
            [ObjectType.NODE] = new CommentNodeConverter(),
            [ObjectType.MESSAGE] = new CommentMessageConverter(),
            [ObjectType.SIGNAL] = new CommentSignalConverter(),
        };
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            ObjectType objectType = ObjectType.NETWORK;
            while (!line.EndsWith($"\";"))
            {
                line += (Environment.NewLine + parserHelper.GetLine());
            }
            Match match = Regex.Match(line, @"CM_\s*(\w+)");
            if (match.Success == false)
            {
                objectType = ObjectType.NETWORK;
            }
            else
            {
                switch (match.Groups[1].Value)
                {
                    case "SG_":
                        objectType = ObjectType.SIGNAL;
                        break;
                    case "BO_":
                        objectType = ObjectType.MESSAGE;
                        break;
                    case "BU_":
                        objectType = ObjectType.NODE;
                        break;
                    default:
                        parserHelper.Exception($" unrecognize object type {match.Groups[1].Value}\r\n");
                        return;
                }
            }
            converters[objectType].Deserialize(dbc, line, parserHelper);
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in converters)
            {
                var content = item.Value.Serialize(dbc);
                if (!string.IsNullOrEmpty(content))
                {
                    stringBuilder.Append(item.Value.Serialize(dbc));
                }
            }
            return stringBuilder.ToString();
        }
    }
}