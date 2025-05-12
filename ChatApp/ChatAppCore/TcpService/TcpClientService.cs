using ChatAppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatAppCore.TcpService
{
    public class TcpClientService : ITcpClientService
    {
        public const int BufferSize = 1024;

        public event Action<Message> MessageRecived;

        //　接続状態変更イベント
        public event Action<bool> ConnectionStatusChanged;


        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        private StringBuilder sb = new StringBuilder();

        public string ConnectionIP { get; set; }

        public int ConnectionPort { get; set; }

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            try
            {
                // すでに確立済み
                if(this._client != null && this._client.Connected)
                {
                    return false;
                }

                this._client = new TcpClient();
                await this._client.ConnectAsync(ip, port);

                if (this._client.Connected)
                {
                    var remoteEndPoint = this._client.Client.RemoteEndPoint as IPEndPoint;
                    if (remoteEndPoint != null)
                    {
                        this.ConnectionInfoConfirm(remoteEndPoint.Address.ToString(), remoteEndPoint.Port);
                    }

                    this._stream = this._client.GetStream();
                    this._cts = new CancellationTokenSource();
                    _ = Task.Run(() => this.ReceiveLoopAsync(this._cts.Token));

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException ex)
            {
                this.ClientDispose();
                return false;
            }
            catch (Exception ex)
            {
                this.ClientDispose();
                return false;
            }
        }

        public void DisConnect()
        {
            this._client.GetStream().Close();

            this.ClientDispose();
            this.ConnectionInfoReset();
        }

        public async Task SendAsync(string message)
        {
            try 
            {
                if (this._client != null && this._stream != null && this._client.Connected) 
                {
                    // バッファをエンコードして、受信文字列取得
                    var sentText = Encoding.GetEncoding("shift_jis").GetBytes($"{message}\r\n");

                    await this._stream.WriteAsync(sentText,0, message.Length);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task ReceiveLoopAsync(CancellationToken token) 
        {
            var buff = new byte[BufferSize];
            var sb = new StringBuilder();

            try 
            {
                while (!token.IsCancellationRequested)
                {
                    // tcpクライアントクラスのstream 
                    int bytes = await _stream.ReadAsync(buff, 0, buff.Length,token);

                    if(bytes == 0)
                    {
                        // 切断
                        this.ConnectionInfoReset();
                        this.ClientDispose();
                        break;
                    }

                    // バッファをエンコードして、受信文字列取得
                    var recieveText = Encoding.GetEncoding("shift_jis").GetString(buff, 0, bytes);

                    // 文字列結合
                    sb.Append(recieveText);
                    string content  = sb.ToString();
                    int newLineIndex;

                    // 受信文字列に改行が含まれている場合、メッセージ作成処理を行う
                    while ((newLineIndex = content.IndexOf("\r\n")) != -1) 
                    {
                        string message = content.Substring(0, newLineIndex).TrimEnd('\r', '\n');
                        sb.Remove(0, newLineIndex+2);

                        this.MessageRecived?.Invoke(new Message(message,this.ConnectionIP,this.ConnectionPort,DateTime.Now));

                        content = sb.ToString();
                    }
                }
            }
            catch (Exception ex) 
            {
                this.ConnectionInfoReset();
                this.ClientDispose();
            }
        }

        private void ClientDispose()
        {
            try
            {
                // 受信ループのキャンセル
                if (this._cts != null)
                {
                    this._cts.Cancel();
                    this._cts.Dispose();
                    this._cts = null;
                }

                // NetworkStream のクローズ
                if (this._stream != null)
                {
                    this._stream.Close();
                    this._stream.Dispose();
                    this._stream = null;
                }

                // TcpClient のクローズ
                if (this._client != null)
                {
                    this._client.Close();
                    this._client.Dispose();
                    this._client = null;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ConnectionInfoConfirm(string ip,int port)
        {
            this.ConnectionIP = ip;
            this.ConnectionPort = port;

            // 接続イベント発火
            this.ConnectionStatusChanged.Invoke(true);
        }

        private void ConnectionInfoReset() 
        {
            this.ConnectionIP = null;
            this.ConnectionPort = -1;

            // 接続イベント発火
            this.ConnectionStatusChanged.Invoke(false);
        }
    }
}
