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
        private readonly StringBuilder _buffer = new StringBuilder();

        /// <summary>
        /// メッセージ受信イベント
        /// </summary>
        public event Action<string> MessageReceived;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">TCP/IPクライアントの設定</param>
        /// <param name="connectionManager">接続管理インスタンス</param>
        public MessageParser(TcpClientSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
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

                MessageReceived?.Invoke(messageContent);

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
