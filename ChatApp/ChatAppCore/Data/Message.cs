using System;

namespace ChatAppCore
{
    public class Message
    {
        /// <summary>Content</summary>
        public string Content { get; set; }

        /// <summary></summary>
        public string SenderIP { get; set; }

        /// <summary></summary>
        public int SenderPort { get; set; }

        /// <summary></summary>
        public DateTime Timestamp { get; set; }

        /// <summary></summary>
        public Message(string content, string senderIP, int senderPort, DateTime timestamp)
        {
            Content = content;
            SenderIP = senderIP;
            SenderPort = senderPort;
            Timestamp = timestamp;
        }
    }
}
