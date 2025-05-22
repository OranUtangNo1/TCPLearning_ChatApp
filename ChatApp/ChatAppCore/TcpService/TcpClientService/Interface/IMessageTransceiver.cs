using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppCore
{
    /// <summary>
    /// メッセージ送受信インターフェース
    /// </summary>
    public interface IMessageTransceiver : IDisposable
    {
        /// <summary>
        /// メッセージ受信イベント
        /// </summary>
        event Action<byte[]> DataReceived;

        /// <summary>
        /// メッセージを非同期に送信する
        /// </summary>
        /// <param name="data">送信するバイトデータ</param>
        Task SendAsync(byte[] data);

        /// <summary>
        /// 受信ループを開始する
        /// </summary>
        Task StartReceiveLoopAsync();
    }
}
