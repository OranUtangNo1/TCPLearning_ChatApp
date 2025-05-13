using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppCore
{
    /// <summary>
    /// メッセージパーサーインターフェース
    /// </summary>
    public interface IMessageParser
    {
        /// <summary>
        /// メッセージを受信した時に発生するイベント
        /// </summary>
        event Action<Message> MessageReceived;

        /// <summary>
        /// 受信したバイトデータをメッセージにパースする
        /// </summary>
        /// <param name="data">受信したバイトデータ</param>
        void ParseReceivedData(byte[] data);

        /// <summary>
        /// メッセージをバイトデータにエンコードする
        /// </summary>
        /// <param name="message">エンコードするメッセージ</param>
        /// <returns>エンコードされたバイトデータ</returns>
        byte[] EncodeMessage(string message);
    }
}
