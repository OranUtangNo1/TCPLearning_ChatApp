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
        public string SenderID { get; set; }
        public string AddressID { get; set; }

        public MessageType MessageType { get; set; }
        public Object Data { get; set; }
    }
}
