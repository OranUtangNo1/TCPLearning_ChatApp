using System;
using System.Threading.Tasks;

namespace ChatAppCore
{
    // TCP通信クラスインターフェース
    public interface ITcpClientService
    {
        // 受信イベント
        event Action<string> MessageRecived;

        //　接続状態変更イベント
        event Action<bool> ConnectionStatusChanged;

        /// <summary>
        /// 接続失敗時に発生するイベント
        /// </summary>
        event Action<string> ConnectFailed;

        // 接続開始処理
        Task<bool> ConnectAsync();

        // 切断処理
        void DisConnect();

         // Message送信処理
        Task SendAsync(string message);
    }
}
