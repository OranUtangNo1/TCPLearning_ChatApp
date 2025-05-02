using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppClient.Model
{
    internal interface IChatModel
    {

        event Action<string> MessageRecieved;


        Task ConnectAsync(string ip, int port);

        Task DisConnectAsync();

        Task SendMessageAsync(string message);
    }
}
