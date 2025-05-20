using ChatAppServer;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    public class ChatServer 
    {
        // Const
        private string _serverID = "999";

        // serversetting
        private int _port;
        private string _ip;

        // インスタンス
        private ClientManager _clientManager;
        private ServerMessageRouter _messageRouter;
        private readonly TcpListenerSetting _severeSetting;

        TcpListener _tcpListener;

        bool IsRunning { get; set; }
        

        public ChatServer(IClientManager clientManager, IServerMessageRouter messageRouter, TcpListenerSetting severeSetting)
        {
            _clientManager = clientManager;
            _messageRouter = messageRouter;
            _severeSetting = severeSetting;
        }

        private async Task Open() 
        {
            try 
            {
                var listener = new TcpListener(IPAddress.Parse(this._ip), this._port);
                listener.Start();

                // 受信ループ
                while (true) 
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    // 接続完了イベント
                    // Client handler 作成
                    if (client != null)
                    {
                        var _clientHandler = new ClientHandler(client);
                        _clientHandler.Start();
                        
                        // Client登録
                        var _id = this._clientManager.RegistClient(_clientHandler);

                        // id通知
                        EnvelopeCreator.
                        this._clientManager.GetClient(_id).SendMessage()
                    }
                    // ClietManagerに処理を移譲

                }
            }
            catch (Exception ex)
            {
                // エラー発生イベント
            }
            finally 
            {
                this._tcpListener?.Dispose();
            }
        }
    }
}