using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppCore.ChatModel.Interface
{
    public interface IChatModels
    {

        event Action<Message> MessageRecieved;

        event Action<bool> ConnectionStatusChanged;


        Task ConnectAsync(string ip, int port);

        Task DisConnectAsync();

        Task SendMessageAsync(string message);
    }
}
