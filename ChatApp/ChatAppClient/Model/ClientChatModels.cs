using ChatAppCore;
using System;
using System.Threading.Tasks;

namespace ChatAppClient.Models
{
    /// <summary>
    /// クライアント用チャットモデルクラス
    /// </summary>
    public class ClientChatModels : IClientChatModels, IDisposable
    {
        #region Filed

        /// <summary>
        /// backing fild
        /// </summary>
        private readonly ITcpClientService _connectionService;
        private readonly IMessageManager _messageManager;
        private bool _disposed = false;

        #endregion

        #region Event

        /// <summary>
        /// メッセージを受信した時に発生するイベント
        /// </summary>
        public event Action<Message> MessageRecieved;

        /// <summary>
        /// 接続状態が変更された時に発生するイベント
        /// </summary>
        public event Action<bool> ConnectionStatusChanged;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="clientService">TCP/IPクライアントサービス</param>
        /// <param name="messageManager">メッセージマネージャ</param>
        public ClientChatModels(ITcpClientService clientService, IMessageManager messageManager = null)
        {
            _connectionService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _messageManager = messageManager ?? new MessageManager();

            // イベントハンドラの登録
            _connectionService.MessageRecived += OnMessageRecived;
            _connectionService.ConnectionStatusChanged += OnConnectionStatusChanged;
        }


        #region Method

        /// <summary>
        /// サーバーに接続する
        /// </summary>
        /// <param name="ip">接続先IPアドレス</param>
        /// <param name="port">接続先ポート番号</param>
        /// <returns>接続処理のタスク</returns>
        /// <exception cref="InvalidOperationException">接続処理に失敗した場合</exception>
        public async Task ConnectAsync(string ip, int port)
        {
            ThrowIfDisposed();

            try
            {
                bool result = await _connectionService.ConnectAsync(ip, port);
                if (!result)
                {
                    throw new InvalidOperationException($"Failed to connect to {ip}:{port}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Connection error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// サーバーとの接続を切断する
        /// </summary>
        /// <returns>切断処理のタスク</returns>
        public async Task DisConnectAsync()
        {
            ThrowIfDisposed();

            try
            {
                _connectionService.DisConnect();
            }
            catch (Exception ex)
            {
                // 切断エラーはログに記録するが、ユーザーには伝えない
                System.Diagnostics.Debug.WriteLine($"Disconnect error: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// メッセージを送信する
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        /// <returns>送信処理のタスク</returns>
        /// <exception cref="ArgumentException">メッセージが空または無効な場合</exception>
        /// <exception cref="InvalidOperationException">送信に失敗した場合</exception>
        public async Task SendMessageAsync(string message)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message cannot be null or empty", nameof(message));
            }

            try
            {
                await _connectionService.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// メッセージ受信時の処理
        /// </summary>
        /// <param name="newMessage">受信したメッセージ</param>
        private void OnMessageRecived(Message newMessage)
        {
            if (newMessage == null)
            {
                return;
            }

            try
            {
                // メッセージをマネージャに追加
                _messageManager.AddMessage(newMessage);

                // イベントを発火
                MessageRecieved?.Invoke(newMessage);
            }
            catch (Exception ex)
            {
                // メッセージ処理エラーはログに記録
                System.Diagnostics.Debug.WriteLine($"Message processing error: {ex.Message}");
            }
        }

        /// <summary>
        /// 接続状態変更時の処理
        /// </summary>
        /// <param name="status">接続状態</param>
        private void OnConnectionStatusChanged(bool status)
        {
            try
            {
                ConnectionStatusChanged?.Invoke(status);
            }
            catch (Exception ex)
            {
                // イベントハンドラエラーはログに記録
                System.Diagnostics.Debug.WriteLine($"Connection status handler error: {ex.Message}");
            }
        }

        /// <summary>
        /// オブジェクトが破棄されている場合に例外をスローする
        /// </summary>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄されている場合</exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ClientChatModels));
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                // イベントハンドラの解除
                if (_connectionService != null)
                {
                    _connectionService.MessageRecived -= OnMessageRecived;
                    _connectionService.ConnectionStatusChanged -= OnConnectionStatusChanged;
                }

                // TcpClientServiceがIDisposableを実装している場合は破棄
                if (_connectionService is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}