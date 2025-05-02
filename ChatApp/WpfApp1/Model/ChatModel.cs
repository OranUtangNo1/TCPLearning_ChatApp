using ChatAppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppClient.Model
{
    public class ChatModel: IChatModel
    {
        private IConnectionService connectionService;

        public event Action<string> MessageRecieved;

        public ChatModel(IConnectionService connectionService) 
        {
            this.connectionService = connectionService;
            this.connectionService.MessageRecived += msg => MessageRecieved?.Invoke(msg); 
        }

        public async Task ConnectAsync(string ip ,int port) 
        {
            await this.connectionService?.ConnectAsync(ip, port);
        }

        public async Task DisConnectAsync() 
        {
            this.connectionService.DisConnect();
            await Task.CompletedTask;
        }

        public async Task SendMessageAsync(string message)
        {
            await connectionService.SendAsync(message);
        }

    }
}
