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

        // 接続開始処理
        Task<bool> ConnectAsync();

        // 切断処理
        void DisConnect();

         // Message送信処理
        Task SendAsync(string message);
    }
}
