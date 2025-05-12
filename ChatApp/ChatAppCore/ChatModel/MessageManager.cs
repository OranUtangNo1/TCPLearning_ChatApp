using ChatAppCore.ChatModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppCore
{
    public class MessageManager : IMessageManager
    {
        public List<Message> Messages = new List<Message>();

        public void AddMessage(Message message)
        {
            this.Messages.Add(message);
        }

        public MessageManager() { }
    }
}
