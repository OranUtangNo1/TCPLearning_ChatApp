using System;
using System.Text;

namespace ChatAppCore
{
    /// <summary>
    /// メッセージパーサークラス
    /// </summary>
    public class MessageParser : IMessageParser
    {
        private readonly TcpClientSettings _settings;
        private readonly IConnectionManager _connectionManager;
        private readonly StringBuilder _buffer = new StringBuilder();

        /// <summary>
        /// メッセージを受信した時に発生するイベント
        /// </summary>
        public event Action<Message> MessageReceived;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">TCP/IPクライアントの設定</param>
        /// <param name="connectionManager">接続管理インスタンス</param>
        public MessageParser(TcpClientSettings settings, IConnectionManager connectionManager)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
        }

        /// <summary>
        /// 受信したバイトデータをメッセージにパースする
        /// </summary>
        /// <param name="data">受信したバイトデータ</param>
        public void ParseReceivedData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            // 受信データをデコード
            string receivedText = _settings.MessageEncoding.GetString(data);
            _buffer.Append(receivedText);

            // バッファの内容を処理
            ProcessBuffer();
        }

        /// <summary>
        /// バッファに蓄積されたデータを処理する
        /// </summary>
        private void ProcessBuffer()
        {
            string content = _buffer.ToString();
            int delimiterIndex;

            // デリミタでメッセージを分割
            while ((delimiterIndex = content.IndexOf(_settings.MessageDelimiter)) != -1)
            {
                string messageContent = content.Substring(0, delimiterIndex).TrimEnd('\r', '\n');
                _buffer.Remove(0, delimiterIndex + _settings.MessageDelimiter.Length);

                // メッセージオブジェクトを作成して通知
                var message = new Message(
                    messageContent,
                    _connectionManager.ConnectionIP,
                    _connectionManager.ConnectionPort,
                    DateTime.Now
                );

                MessageReceived?.Invoke(message);

                // 残りのコンテンツを取得
                content = _buffer.ToString();
            }
        }

        /// <summary>
        /// メッセージをバイトデータにエンコードする
        /// </summary>
        /// <param name="message">エンコードするメッセージ</param>
        /// <returns>エンコードされたバイトデータ</returns>
        public byte[] EncodeMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message cannot be null or empty", nameof(message));
            }

            // メッセージにデリミタを追加してエンコード
            return _settings.MessageEncoding.GetBytes(message + _settings.MessageDelimiter);
        }
    }
}
