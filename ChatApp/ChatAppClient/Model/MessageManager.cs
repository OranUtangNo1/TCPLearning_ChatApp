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
        private List<Message> messages = new List<Message>();
        #endregion

        #region Method
        /// <summary>
        /// メッセージを管理下へ追加
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void AddMessage(Message message)
        {
            this.messages.Add(message);
        }
        #endregion

        public MessageManager() { }
    }
}
