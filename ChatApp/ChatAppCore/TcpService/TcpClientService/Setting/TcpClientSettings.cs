using System;
using System.Text;

namespace ChatAppCore
{
    /// <summary>
    /// TCP/IPクライアントの設定を管理するクラス
    /// </summary>
    public class TcpClientSettings
    {
        /// <summary>
        /// 接続タイムアウト (ミリ秒)
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5000;

        /// <summary>
        /// 受信バッファサイズ
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// メッセージのエンコーディング
        /// </summary> 
        public Encoding MessageEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// メッセージの終端文字列
        /// </summary>
        public string MessageDelimiter { get; set; } = "\r\n";
    }
}