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
            var data = EnvelopeDataCreate(MessageType.ChatMessage, content);
            var envelope = new MessageEnvelope()
            {
                MessageType = MessageType.ChatMessage,
                SenderID = senderID,
                AddressID = addressID,
                Data = data
            };
            return envelope;
        }

        public static MessageEnvelope ConnectMessageEnvelopeCreate(string preferUserName, string senderID, string addressID)
        {
            var data = EnvelopeDataCreate(MessageType.Connect, preferUserName);
            var envelope = new MessageEnvelope()
            {
                MessageType = MessageType.Connect,
                SenderID = senderID,
                AddressID = addressID,
                Data = data
            };
            return envelope;
        }

        public static MessageEnvelope ConnectedMessageEnvelopeCreate(string assignedID, string senderID, string addressID)
        {
            var data = EnvelopeDataCreate(MessageType.Connected, assignedID);
            var envelope = new MessageEnvelope()
            {
                MessageType = MessageType.Connected,
                SenderID = senderID,
                AddressID = addressID,
                Data = data
            };
            return envelope;
        }

        private static Object EnvelopeDataCreate(MessageType messageType,params string[] strings) 
        {
            switch (messageType) 
            {
                case MessageType.ChatMessage:
                    if (strings.Length != 1) throw new ArgumentException("falid Number of parameters");
                    var chatMessage = new ChatMessage()
                    {
                        Message = strings[0],
                    };
                    return chatMessage;

                case MessageType.Connect:
                    if (strings.Length != 1) throw new ArgumentException("falid Number of parameters");
                    var connectMessage = new ConnectMessage()
                    {
                        PreferUserName = strings[0],
                    };
                    return connectMessage;

                case MessageType.Connected:
                    if (strings.Length != 1) throw new ArgumentException("falid Number of parameters");
                    var connectedMessage = new ConnectedMessage()
                    {
                        AssignedID = strings[0],
                    };
                    return connectedMessage;

                default: throw new ArgumentException("falid MessageType");
            }

        }

    }
}
