using ChatAppCore;
using System.Collections.Generic;

namespace ChatAppClient.Models
{
    /// <summary>
    /// メッセージ管理クラス
    /// </summary>
    public class MessageManager : IMessageManager
    {
        #region Filed
        /// <summary>
        /// メッセージリスト
        /// </summary>
        private List<ChatMessage> messages = new List<ChatMessage>();
        #endregion

        #region Method
        /// <summary>
        /// メッセージを管理下へ追加
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void AddMessage(ChatMessage message)
        {
            this.messages.Add(message);
        }
        #endregion

        public MessageManager() { }
    }
}
