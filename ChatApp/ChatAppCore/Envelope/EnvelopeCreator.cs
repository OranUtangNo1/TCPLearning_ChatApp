using ChatAppCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppCore.Envelope
{
    public static class EnvelopeCreator
    {
        public static MessageEnvelope ChatMessageEnvelopeCreate(string content,string senderID,string addressID) 
        {
            var data = EnvelopeCreate(MessageType.ChatMessage, content, senderID, addressID);
            var envelope = new MessageEnvelope()
            {
                MessageType = MessageType.ChatMessage,
                Data = data
            };
            return envelope;
        }

        public static MessageEnvelope ConnectMessageEnvelopeCreate(string preferUserName, string senderID, string addressID)
        {
            var data = EnvelopeCreate(MessageType.Connect, preferUserName, senderID, addressID);
            var envelope = new MessageEnvelope()
            {
                MessageType = MessageType.Connect,
                Data = data
            };
            return envelope;
        }


        private static Object EnvelopeCreate(MessageType messageType,params string[] strings) 
        {
            switch (messageType) 
            {
                case MessageType.ChatMessage:
                    if (strings.Length != 3) throw new ArgumentException("falid Number of parameters");
                    var chatMessage = new ChatMessage()
                    {
                        Content = strings[0],
                        To = strings[1],
                        From = strings[2],
                    };
                    return chatMessage;
                case MessageType.Connect:
                    if (strings.Length != 3) throw new ArgumentException("falid Number of parameters");
                    var connectMessage = new ConnectMessage()
                    {
                        PreferUserName = strings[0],
                        To = strings[1],
                        From = strings[2],
                    };
                    return connectMessage;

                default: throw new ArgumentException("falid MessageType");
            }

        }

    }
}
