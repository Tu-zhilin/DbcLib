using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Builder
{
    internal class MessageBuilder : IBuilder
    {
        public void Build(Dbc dbc)
        {
            // 构建默认Message
            var message = dbc.CreateMessage("VECTOR__INDEPENDENT_SIG_MSG");
            message.Id = 0xC0000000;
            message.Size = 0;
            message.Comment = "This is a message for not used signals, created by Vector CANdb++ DBC OLE DB Provider.";
        }
    }
}