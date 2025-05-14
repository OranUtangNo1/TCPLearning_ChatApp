using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatAppCore
{
    /// <summary>
    /// メッセージ送受信クラス
    /// </summary>
    public class MessageTransceiver : IMessageTransceiver
    {
        private readonly IConnectionManager _connectionManager;
        private readonly TcpClientSettings _settings;
        private NetworkStream _stream => _connectionManager.NetworkStream;

        /// <summary>
        /// メッセージを受信した時に発生するイベント
        /// </summary>
        public event Action<byte[]> DataReceived;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionManager">接続管理インスタンス</param>
        /// <param name="settings">TCP/IPクライアントの設定</param>
        public MessageTransceiver(IConnectionManager connectionManager, TcpClientSettings settings)
        {
            _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// メッセージを非同期に送信する
        /// </summary>
        /// <param name="data">送信するバイトデータ</param>
        public async Task SendAsync(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data cannot be null or empty", nameof(data));
            }

            if (_stream == null || !_connectionManager.IsConnected)
            {
                throw new InvalidOperationException("Not connected to server");
            }

            try
            {
                await _stream.WriteAsync(data, 0, data.Length);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 受信ループを開始する
        /// </summary>
        public async Task StartReceiveLoopAsync()
        {
            if (_stream == null || !_connectionManager.IsConnected)
            {
                throw new InvalidOperationException("Not connected to server");
            }

            try
            {
                await Task.Run(async () =>
                {
                    await ReceiveLoopAsync(_connectionManager.CancellationToken);
                });
            }
            catch (OperationCanceledException)
            {
                // キャンセルは正常な終了
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Receive loop error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 受信ループの実装
        /// </summary>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[_settings.BufferSize];

            try
            {
                while (!cancellationToken.IsCancellationRequested && _connectionManager.IsConnected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    if (bytesRead == 0)
                    {
                        // 接続が切断された
                        _connectionManager.Disconnect();
                        break;
                    }

                    // 受信データをコピーして通知
                    var receivedData = new byte[bytesRead];
                    Array.Copy(buffer, receivedData, bytesRead);
                    DataReceived?.Invoke(receivedData);
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセルは正常な終了
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                // 非キャンセルエラーで接続を切断
                _connectionManager.Disconnect();
                throw new InvalidOperationException($"Receive error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
        }
    }
}
