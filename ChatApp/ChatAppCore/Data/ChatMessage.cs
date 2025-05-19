using System;

namespace ChatAppCore
{
    //public interface IMessage 
    //{
    //    /// <summary></summary>
    //    string To { get;}

    //    /// <summary></summary>
    //    string From { get; }
    //}

    //public class ChatMessage: IMessage
    public class ChatMessage
    {
        /// <summary>Content</summary>
        public string Content { get; set; }

        /// <summary></summary>
        public string To { get; set; }

        /// <summary></summary>
        public string From { get; set; }

    }

    //public class ConnectMessage : IMessage
    public class ConnectMessage
    {
        /// <summary>UserName</summary>
        public string PreferUserName { get; set; }

        /// <summary></summary>
        public string To { get; set; }

        /// <summary></summary>
        public string From { get; set; }
    }

    //public class ConnectedMessage : IMessage
    public class ConnectedMessage
    {
        /// <summary>割り当てられたClientID</summary>
        public string ClientID { get; set; }

        /// <summary></summary>
        public string To { get; set; }

        /// <summary></summary>
        public string From { get; set; }
    }


}
