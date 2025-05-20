using System;

namespace ChatAppCore
{
    public class ChatMessage
    {
        /// <summary>Content</summary>
        public string Message { get; set; }
    }

    public class ConnectMessage
    {
        /// <summary>UserName</summary>
        public string PreferUserName { get; set; }
    }

    public class ConnectedMessage
    {
        /// <summary>割り当てられたClientID</summary>
        public string AssignedID { get; set; }
    }


}
