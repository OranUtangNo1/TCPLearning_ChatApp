using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppCore.Data
{

    public enum MessageType
    {
        Connect,
        Connected,
        ChatMessage,
    }

    public class MessageEnvelope
    {
        public MessageType MessageType { get; set; }
        public Object Data { get; set; }
    }
}
