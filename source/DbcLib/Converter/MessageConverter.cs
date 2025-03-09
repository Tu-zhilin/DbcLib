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
    internal class MessageConverter : IConverter
    {
        private SignalConverter signalConverter;
        public MessageConverter()
        {
            signalConverter = new SignalConverter();
        }
        public void Deserialize(Dbc dbc, string line, ParserHelper parserHelper)
        {
            Match match;
            Message message;
            match = new Regex(@"BO_\s*(\d*)\s*(\w*)\s*:\s*(\d*)\s*(\w*)").Match(line);
            message = dbc.CreateMessage(match.Groups[2].Value);
            message.Id = DbcHelper.GetMessageIDFromFile(match.Groups[1].Value);
            message.Size = int.Parse(match.Groups[3].Value);
            foreach (var item in match.Groups[4].Value.Split(','))
            {
                message.AddTransmitter(item);
            }
            string next = null;
            do
            {
                next = parserHelper.GetLine();
                if (string.IsNullOrEmpty(next))
                {
                    continue;
                }
                message.AddSignal(signalConverter.Deserialize(dbc, next));
            } while (!string.IsNullOrEmpty(next));
        }
        public string Serialize(Dbc dbc)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var message in dbc.Messages)
            {
                sb.AppendLine(_serializeMessage(message));
                foreach (var signal in message.Signals)
                {
                    sb.AppendLine(signalConverter.Serialize(signal));
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
        private string _serializeMessage(Message message)
        {
            if (message.Name == "VECTOR__INDEPENDENT_SIG_MSG" && message.Signals.Count == 0)
            {
                // 该信号Vector用来包含无归属的信号的，所以如果底下没有信号，则不需要生成
                return "";
            }
            if (message.Transmitters.Count == 0)
            {
                return $"BO_ {DbcHelper.GetMessageIDForFile(message.Id)} {message.Name}: {message.Size}" + " Vector__XXX";
            }
            return $"BO_ {DbcHelper.GetMessageIDForFile(message.Id)} {message.Name}: {message.Size}" + $" {message.Transmitters.First()}";
        }
    }
    internal class SignalConverter
    {
        public Signal Deserialize(Dbc dbc, string line)
        {
            int index = 1;
            Regex regex = new Regex(@"SG_\s*(\w+)\s*?(\w*?)\s*?\:\s*(\w*)\|(\d*)\@(\d*)\s*(\+|\-)\s*\((-?\d+\.?\d*),(-?\d+\.?\d*)\)\s*\[(-?\d+\.?\d*)\|([-+]?\d+\.\d*\d+[eE][-+]?\d+|-?\d+\.?\d*)\]\s*\""([^\""]*)\""\s*(.*)");
            Match match = regex.Match(line);
            Signal signal = dbc.CreateSignal(match.Groups[index++].Value);
            signal.Multiplexing = match.Groups[index++].Value;
            signal.StartBit = int.Parse(match.Groups[index++].Value);
            signal.Size = int.Parse(match.Groups[index++].Value);
            signal.SetByteOrder((ByteOrder)int.Parse(match.Groups[index++].Value));
            signal.DataType = match.Groups[index++].Value == "+" ? DataType.UNSIGNED : DataType.SIGNED;
            signal.Factor = double.Parse(match.Groups[index++].Value);
            signal.Offset = double.Parse(match.Groups[index++].Value);
            signal.Min = double.Parse(match.Groups[index++].Value);
            signal.Max = double.Parse(match.Groups[index++].Value);
            signal.Unit = match.Groups[index++].Value;
            foreach (var item in match.Groups[index++].Value.Split(','))
            {
                signal.AddReceiver(item);
            }
            return signal;
        }
        public string Serialize(Signal signal)
        {
            string content = $" SG_ {signal.Name} : {signal.StartBit}|{signal.Size}@{(signal.ByteOrder == ByteOrder.MSB ? 0 : 1)}{(signal.DataType == DataType.UNSIGNED ? "+" : "-")} ({signal.Factor},{signal.Offset}) [{signal.Min}|{signal.Max}] \"{signal.Unit}\"";
            
            if (signal.Receivers.Count == 0)
            {
                return content + " Vector__XXX";
            }
            for (int i = 0; i < signal.Receivers.Count; i++)
            {
                content += (i == 0 ? " " : "") + $"{signal.Receivers[i]}" + (i < signal.Receivers.Count - 1 ? "," : "");
            }
            return content;
        }
    }
}