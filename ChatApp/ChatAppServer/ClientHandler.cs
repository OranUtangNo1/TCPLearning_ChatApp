using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppServer
{
    internal class ClientHandler : IDisposable
    {
        private string _clientID;
        private string userName ;

        private TcpClient tcpClient;

        NetworkStream netWorkStream;
        StreamReader streamReader;
        StreamWriter streamWriter;

        // dispose filed
        private bool disposed = false;

        public ClientHandler(TcpClient client) 
        {
            this.tcpClient = client;

        }

        public void Start()
        {
            try 
            {
                using (tcpClient)
                using (this.netWorkStream = tcpClient.GetStream())
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = netWorkStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {

                            var jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            // メッセージ受信イベント発火

                        }
                        else
                        {
                            // クライアント切断検知イベント発火
                        }
                    }
                }
            }
            catch 
            {
                // dispose する

                // Error発生例外スロー

            }
        }

        public void SendMessage(string jsonMessage) 
        {
            try
            {
                this.streamWriter?.WriteLine(jsonMessage);
            }
            catch
            {
                //dispose
                this.Dispose(true);
                // Error発生例外スロー
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) 
        {
            // ガード節
            if (disposed)
            {
                return;
            }

            // マネージドリソース 明示的に呼び出された場合はマネージドリソースも破棄
            if (disposing) 
            {
                this.streamReader?.Dispose();
                this.streamWriter?.Dispose();
                this.netWorkStream?.Dispose();
                this.tcpClient?.Close();
            }

            // アンマネージドリソースの破棄

            this.disposed = true;
        }

        // デストラクタ
        ~ClientHandler() 
        {
            this.Dispose(false);
        }
    }
}
