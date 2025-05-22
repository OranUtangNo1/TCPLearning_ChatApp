using ChatAppServer;
using ChatAppCore;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ChatAppCore.Envelope;
using System.Text.Json;
using System.Security.Cryptography;

namespace ChatServer
{
    public class ChatServer 
    {
        // Const
        private string _serverID = "000";

        // serversetting
        private int _port;
        private string _ip;

        private bool isInitilaize = false;

        // インスタンス
        private ClientManager _clientManager;
        private MessageRouter _messageRouter;
        private readonly TcpListenerSetting _listerSetting;

        TcpListener _tcpListener;

        bool IsRunning { get; set; }

        public event Action<string> MessageReceived;

        public event Action<string> ClientConnected;
        public event Action<string> ClientDisConnected;
        

        public ChatServer(ClientManager clientManager, MessageRouter messageRouter, TcpListenerSetting listerSetting)
        {
            _clientManager = clientManager;
            _messageRouter = messageRouter;
            _listerSetting = listerSetting;

            // 初期設定
            this.InitialSetting();

            this.isInitilaize = true;
        }
        public async Task Open()
        {

            if (!isInitilaize) throw new Exception("Server cannot be started.");
            try
            {
                var listener = new TcpListener(IPAddress.Parse(this._ip), this._port);
                listener.Start();

                // 受信ループ
                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    // Client handler 作成
                    if (client != null)
                    {
                        var _clientHandler = new ClientHandler(client);
                        _clientHandler.Start();

                        // client登録
                        var _id = this.RegistClient(_clientHandler);

                        // 接続完了イベント
                        this.ClientConnected?.Invoke($"[Client connected.] ID:{_id}");

                        // clientにidを通知
                        var _envelope = EnvelopeCreator.ConnectedMessageEnvelopeCreate(_id, _serverID, _id);
                        string json = JsonSerializer.Serialize(_envelope);
                        this._clientManager.GetClient(_id).SendMessage(json);
                    }
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

        private void InitialSetting() 
        {
            this._ip = this._listerSetting.ServerIP;
            this._port = this._listerSetting.Port;

            this._tcpListener = new TcpListener(IPAddress.Parse(this._ip), this._port);
        }

        private string RegistClient(ClientHandler _connectedClient) 
        {
            var assignedID = this._clientManager.RegistClient(_connectedClient);
            // メッセージ受信イベント
            _connectedClient.MessageRecived += (jsonString) =>
            {
                // ルーティングに投げる
                this._messageRouter.MessageRouting(jsonString);
            };

            _connectedClient.ClientDisConnected += (dummy)=>
            {
                // 切断検知した際の処理
                // 接続完了イベント
                this.ClientDisConnected?.Invoke($"[Client disconnected] ID:{assignedID}");
            };

            return assignedID;
        }


    }
}