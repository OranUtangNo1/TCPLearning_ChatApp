using ChatAppCore;

namespace ChatAppClient.Models
{
    /// <summary>
    /// メッセージ管理クラス用インターフェース
    /// </summary>
    public interface IMessageManager
    {
        /// <summary>
        /// メッセージを管理下へ追加
        /// </summary>
        /// <param name="message">メッセージ</param>
        void AddMessage(ChatMessage message);
    }
}
