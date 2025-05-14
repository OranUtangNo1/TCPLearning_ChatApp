using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatAppCore
{
    /// <summary>
    /// TCP/IP接続管理クラス
    /// </summary>
    public class ConnectionManager : IConnectionManager
    {
        private readonly TcpClientSettings _settings;
        private TcpClient _client;
        private CancellationTokenSource _cts;

        /// <summary>
        /// 接続先のIPアドレス
        /// </summary>
        public string ConnectionIP { get; private set; }

        /// <summary>
        /// 接続先のポート番号
        /// </summary>
        public int ConnectionPort { get; private set; }

        /// <summary>
        /// アクティブな接続があるかを確認する
        /// </summary>
        public bool IsConnected => _client != null && _client.Connected;

        /// <summary>
        /// キャンセレーショントークンを取得する
        /// </summary>
        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        /// <summary>
        /// steram
        /// </summary>
        public NetworkStream NetworkStream { get; private set; }

        /// <summary>
        /// 接続状態が変更された時に発生するイベント
        /// </summary>
        public event Action<bool, string> ConnectionStatusChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">TCP/IPクライアントの設定</param>
        public ConnectionManager(TcpClientSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// 非同期に接続を試みる
        /// </summary>
        /// <param name="ip">接続先IPアドレス</param>
        /// <param name="port">接続先ポート番号</param>
        /// <returns>接続に成功した場合はtrue、それ以外はfalse</returns>
        public async Task<bool> ConnectAsync(string ip, int port)
        {
            try
            {
                // すでに接続中の場合は何もしない
                if (IsConnected)
                {
                    ConnectionStatusChanged?.Invoke(true, "Already connected");
                    return true;
                }

                // 接続の前に、既存のリソースをクリーンアップ
                DisposeTcpClient();

                // 新しいキャンセレーショントークンソースを作成
                _cts = new CancellationTokenSource();

                // 新しいTCPクライアントを作成
                _client = new TcpClient();

                // タイムアウト付きで接続を試みる
                var connectTask = _client.ConnectAsync(ip, port);
                var timeoutTask = Task.Delay(_settings.ConnectionTimeout);

                // いずれかのタスクが完了するまで待機
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                // タイムアウトした場合
                if (completedTask == timeoutTask)
                {
                    DisposeTcpClient();
                    ConnectionStatusChanged?.Invoke(false, "Connection timed out");
                    return false;
                }

                // 接続エラーが発生した場合
                if (_client == null || !_client.Connected)
                {
                    DisposeTcpClient();
                    ConnectionStatusChanged?.Invoke(false, "Connection failed");
                    return false;
                }

                // 接続に成功した場合、接続情報を更新
                var remoteEndPoint = _client.Client.RemoteEndPoint as IPEndPoint;
                if (remoteEndPoint != null)
                {
                    ConnectionIP = remoteEndPoint.Address.ToString();
                    ConnectionPort = remoteEndPoint.Port;
                }
                else
                {
                    ConnectionIP = ip;
                    ConnectionPort = port;
                }

                // Stream更新
                this.NetworkStream = _client.GetStream();

                // 接続成功イベントを発火
                ConnectionStatusChanged?.Invoke(true, "Connected successfully");
                return true;
            }
            catch (SocketException ex)
            {
                DisposeTcpClient();
                ConnectionStatusChanged?.Invoke(false, $"Socket error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                DisposeTcpClient();
                ConnectionStatusChanged?.Invoke(false, $"Unexpected error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 接続を切断する
        /// </summary>
        public void Disconnect()
        {
            try
            {
                DisposeTcpClient();
                ConnectionStatusChanged?.Invoke(false, "Disconnected");
            }
            catch (Exception ex)
            {
                ConnectionStatusChanged?.Invoke(false, $"Error during disconnect: {ex.Message}");
            }
        }

        /// <summary>
        /// TcpClient用のネットワークストリームを取得する
        /// </summary>
        /// <returns>ネットワークストリーム</returns>
        /// <exception cref="InvalidOperationException">クライアントが接続されていない場合</exception>
        public NetworkStream GetStream()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client is not connected");
            }
            return _client.GetStream();
        }

        /// <summary>
        /// TcpClientをDisposeする
        /// </summary>
        private void DisposeTcpClient()
        {
            try
            {
                // キャンセレーショントークンのキャンセル
                _cts?.Cancel();

                // クライアントのクローズとDispose
                if (_client != null)
                {
                    if (_client.Connected)
                    {
                        try
                        {
                            _client.Client.Shutdown(SocketShutdown.Both);
                        }
                        catch
                        {
                            // 既に接続が切れている場合など、例外を無視
                        }
                    }
                    _client.Close();
                    _client = null;
                }

                // streamを閉じる
                if (NetworkStream != null)
                {
                    NetworkStream.Close();
                    NetworkStream.Dispose();
                    NetworkStream = null;
                }

                // 接続情報のリセット
                ConnectionIP = null;
                ConnectionPort = -1;
            }
            catch (Exception)
            {
                // リソースの解放中のエラーは握りつぶす
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            _cts?.Dispose();
            _cts = null;
        }
    }
}

