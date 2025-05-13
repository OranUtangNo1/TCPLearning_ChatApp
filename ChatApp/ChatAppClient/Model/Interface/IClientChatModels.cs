using ChatAppCore;
using System;
using System.Threading.Tasks;

namespace ChatAppClient.Models
{
    /// <summary>
    /// チャットモデルクラスインターフェース
    /// </summary>
    public interface IClientChatModels
    {
        #region Event

        /// <summary>
        /// メッセージを受信した時に発生するイベント
        /// </summary>
        event Action<Message> MessageRecieved;

        /// <summary>
        /// 接続状態が変更された時に発生するイベント
        /// </summary>
        event Action<bool> ConnectionStatusChanged;

        #endregion

        #region Method

        /// <summary>
        /// サーバーに接続する
        /// </summary>
        /// <param name="ip">接続先IPアドレス</param>
        /// <param name="port">接続先ポート番号</param>
        /// <returns>接続処理のタスク</returns>
        /// <exception cref="InvalidOperationException">接続処理に失敗した場合</exception>
        Task ConnectAsync(string ip, int port);

        /// <summary>
        /// サーバーとの接続を切断する
        /// </summary>
        /// <returns>切断処理のタスク</returns>
        Task DisConnectAsync();

        /// <summary>
        /// メッセージを送信する
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        /// <returns>送信処理のタスク</returns>
        /// <exception cref="ArgumentException">メッセージが空または無効な場合</exception>
        /// <exception cref="InvalidOperationException">送信に失敗した場合</exception>
        Task SendMessageAsync(string message);

        #endregion
    }
}
