using System;
using System.Threading.Tasks;

namespace ChatAppCore
{
    /// <summary>
    /// TCP/IPクライアントのファサードクラス
    /// </summary>
    public class TcpClientFacade : ITcpClientService
    {
        private readonly TcpClientSettings _settings;
        private readonly IConnectionManager _connectionManager;
        private readonly IMessageTransceiver _messageTransceiver;
        private readonly IMessageParser _messageParser;

        /// <summary>
        /// メッセージを受信した時に発生するイベント
        /// </summary>
        public event Action<string> MessageRecived;

        /// <summary>
        /// 接続状態が変更された時に発生するイベント
        /// </summary>
        public event Action<bool> ConnectionStatusChanged;

        /// <summary>
        /// 接続失敗時に発生するイベント
        /// </summary>
        public event Action<string> ConnectFailed;

        /// <summary>
        /// 接続先のIPアドレス
        /// </summary>
        public string ConnectionIP => _connectionManager.ConnectionIP;

        /// <summary>
        /// 接続先のポート番号
        /// </summary>
        public int ConnectionPort => _connectionManager.ConnectionPort;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">TCP/IPクライアントの設定</param>
        public TcpClientFacade(TcpClientSettings settings = null)
        {
            _settings = settings == null ?  new TcpClientSettings() : settings;

            _connectionManager = new ConnectionManager(settings);
            _messageTransceiver = new MessageTransceiver(_connectionManager, settings);
            _messageParser = new MessageParser(settings);

            // イベントの接続
            _connectionManager.ConnectionStatusChanged += OnConnectionStatusChanged;
            // イベントの接続
            _connectionManager.ConnectFailed += OnConnectFailed;


            _messageTransceiver.DataReceived += OnDataReceived;
            _messageParser.MessageReceived += OnMessageReceived;
        }

        /// <summary>
        /// 非同期に接続を試みる
        /// </summary>
        /// <param name="ip">接続先IPアドレス</param>
        /// <param name="port">接続先ポート番号</param>
        /// <returns>接続に成功した場合はtrue、それ以外はfalse</returns>
        public async Task<bool> ConnectAsync()
        {
            bool result = await _connectionManager.ConnectAsync(_settings.ServerIP, _settings.ServerPort);
            if (result)
            {
                try
                {
                    // 受信ループを開始
                    _ = _messageTransceiver.StartReceiveLoopAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await Task.CompletedTask;
                    return false;
                }
            }
            return result;
        }

        /// <summary>
        /// 接続を切断する
        /// </summary>
        public void DisConnect()
        {
            _connectionManager.Disconnect();
        }

        /// <summary>
        /// メッセージを非同期に送信する
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        public async Task SendAsync(string message)
        {
            try
            {
                byte[] data = _messageParser.EncodeMessage(message);
                await _messageTransceiver.SendAsync(data);
            }
            catch (Exception)
            {
                // 送信エラーを無視（既存の実装と同様）
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// データ受信時の処理
        /// </summary>
        /// <param name="data">受信したバイトデータ</param>
        private void OnDataReceived(byte[] data)
        {
            _messageParser.ParseReceivedData(data);
        }

        /// <summary>
        /// メッセージ受信時の処理
        /// </summary>
        /// <param name="message">受信したメッセージ</param>
        private void OnMessageReceived(string rowMessage)
        {
            MessageRecived?.Invoke(rowMessage);
        }

        /// <summary>
        /// 接続状態変更時の処理
        /// </summary>
        /// <param name="isConnected">接続状態</param>
        /// <param name="message">ステータスメッセージ</param>
        private void OnConnectionStatusChanged(bool isConnected, string message)
        {
            ConnectionStatusChanged?.Invoke(isConnected);
        }

        /// <summary>
        /// 接続状態変更時の処理
        /// </summary>
        /// <param name="isConnected">接続状態</param>
        /// <param name="message">ステータスメッセージ</param>
        private void OnConnectFailed(string message)
        {
            ConnectFailed?.Invoke(message);
        }

    }
}
