using ChatAppCore.ChatModel.Interface;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ChatAppCore
{
    public class ChatModels : IChatModels
    {
        
        private ITcpClientService connectionService;
        private IMessageManager messageManager;

        public event Action<Message> MessageRecieved;
        public event Action<bool> ConnectionStatusChanged;

        public ChatModels(ITcpClientService clientService) 
        {
            this.connectionService = clientService;
            this.connectionService.MessageRecived += msg => this.OnMessageRecived(msg);
            this.connectionService.ConnectionStatusChanged += status => this.ConnectionStatusChanged.Invoke(status);

            this.messageManager = new MessageManager();
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

        public void OnMessageRecived(Message newMessage) 
        {
            this.messageManager.AddMessage(newMessage);
            this.MessageRecieved.Invoke(newMessage);
        }

    }
}
