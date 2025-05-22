using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace ChatAppServer
{
    public class ClientHandler : IDisposable
    {
        private TcpClient tcpClient;
        NetworkStream netWorkStream;
        StreamWriter streamWriter;

        // dispose filed
        private bool disposed = false;
        private bool isDisConnected = false;

        /// <summary>
        /// メッセージ受信イベント
        /// </summary>
        public event Action<string> MessageRecived;

        /// <summary>
        /// クライアント切断検知イベント
        /// </summary>
        public event Action<string> ClientDisConnected;

        public ClientHandler(TcpClient client)
        {
            this.tcpClient = client;

        }

        public void Start()
        {
            Task.Run(() => RecieveLoop());
        }

        private void RecieveLoop()
        {
            try
            {
                using (tcpClient)
                using (this.netWorkStream = tcpClient.GetStream())
                using (this.streamWriter = new StreamWriter(this.netWorkStream, Encoding.UTF8))
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = netWorkStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            var jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            // メッセージ受信イベント発火
                            this.MessageRecived?.Invoke(jsonString);
                        }
                        else
                        {
                            // クライアント切断検知イベント発火
                            this.OnClientDisConnected();
                            break;
                        }
                    }
                }
            }
            // IO関係の例外
            catch (IOException)
            {
                // Error発生例外スロー
                this.OnClientDisConnected();
            }
            //その他の例外
            catch (Exception ex)
            {
                // Error発生例外スロー
                this.OnClientDisConnected();
            }
            finally
            {
                this.Dispose();
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
                this.Dispose();
                // Error発生例外スロー
            }
        }

        private void OnClientDisConnected() 
        {
            if (isDisConnected) return;

            isDisConnected = true;
            this.ClientDisConnected?.Invoke("000");
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
