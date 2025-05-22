using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ChatAppClient.Models;
using ChatAppCore;
using ChatAppCore.FrameWork;

namespace ChatAppClient.ViewModel
{

    internal class ClientInfo 
    {
        public string UserName {  get; set; }
        public string UserID {  get; set; }
    }
    /// <summary>
    /// クライアント用チャット画面ViewModel
    /// </summary>
    internal class VmlMainWindow : ViewModelBase,IVmlMainWindow
    {

        #region Const

        /// <summary>status</summary>
        private const string StatusConnect = "接続中";
        private const string StatusDisConnect = "切断中";

        #endregion

        #region  Private Filed

        /// <summary>ip</summary>
        private string _targetUser;

        /// <summary>port</summary>
        private string _targetPort;

        /// <summary>status</summary>
        private string _connectionStatus;

        /// <summary>sendText</summary>
        private string _inputText = String.Empty;

        /// <summary>listBox messages</summary>
        private ObservableCollection<string> _messages = new ObservableCollection<string>();

        /// <summary>UserID</summary>
        private string _userID = String.Empty;

        /// <summary>UserName</summary>
        private string _userName = String.Empty;


        /// <summary></summary>
        private ObservableCollection<ClientInfo> _connectedClientList = new ObservableCollection<ClientInfo>();

        /// <summary></summary>
        private ClientInfo targetClient = new ClientInfo() { UserName = "テスト君", UserID = "999" };


        /// <summary>chat Func</summary>
        private IClientChatModels chatModel = null;

        #endregion

        #region  View Propertys



        /// <summary>TargetUser</summary>
        public string TargetUser 
        {
            get => _targetUser;
            set => this.SetProperty(ref this._targetUser, value);
        }

        /// <summary>Status Lbl</summary>
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => this.SetProperty(ref this._connectionStatus, value);
        }

        /// <summary>Send TextB</summary>
        public string InputText
        {
            get => _inputText;
            set => this.SetProperty(ref this._inputText, value);
        }

        /// <summary>ChatMessage List</summary>
        public ObservableCollection<string> Messages 
        {
            get => _messages;
            set => this.SetProperty(ref this._messages, value);
        }

        public string UserID 
        {
            get => _userID;
            set => this.SetProperty(ref _userID, value);
        }

        public string UserName
        {
            get => _userName;
            set => this.SetProperty(ref _userName, value);
        }

        #endregion

        #region Command

        /// <summary>ConnectBtn Command</summary>
        public ICommand ConnectCommand { get; }

        /// <summary>DisConnectBtn Command</summary>
        public ICommand DisConnectCommand { get; }

        /// <summary>SenBtn Command</summary>
        public ICommand SendCommand { get; }

        #endregion

        #region Constractor

        public VmlMainWindow(IClientChatModels chatModel)
        {
            // ======= 初期値設定  =====================
            this.InitialSetting();

            // ======= コマンド登録  =====================
            this.ConnectCommand = new RelayCommand(Connect, ConnectCanExecute);
            this.DisConnectCommand = new RelayCommand(DisConnect);
            this.SendCommand = new RelayCommand(Send);

            // ======= インスタンス登録  =====================
            this.chatModel = chatModel;

            // ======= イベントハンドラ登録  =====================
            // メッセージ受信
            this.chatModel.ChatMessageRecieved += msg => this.OnMessageRecieved(msg.Item1,msg.Item2);
            // UserID受信
            this.chatModel.ClientIDConfirmed += msg => UserID = msg.AssignedID;

            // 接続状態変更
            this.chatModel.ConnectionStatusChanged += status => this.OnConnectionStatusChanged(status);
            // 接続失敗
            this.chatModel.ConnectFailed += msg => this.OnConnectFailed(msg);

        }

        #endregion

        #region Comand Method

        /// <summary>Connect</summary>
        private bool ConnectCanExecute()
        {
            return !String.IsNullOrEmpty(this.UserName);
        }

        /// <summary>Connect</summary>
        private void Connect()
        {
            try 
            {
                if (this.ConnectionStatus == "接続中") return;

                // 接続
                this.chatModel.ConnectAsync();
                // UserName通知
                this.chatModel.SendPreferUserNameAsync(this.UserName);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>DisConnect</summary>
        private void DisConnect()
        {
            this.chatModel.DisConnectAsync();
        }

        /// <summary>Send</summary>
        private void Send()
        {
            try 
            {
                this.chatModel.SendChatMessageAsync(this.InputText, int.Parse(targetClient.UserID));
                this.AddMessageToList($"{DateTime.Now} : 送-->  {this.InputText}");

                this.InputText = string.Empty;
            }
            catch (ArgumentException ex) 
            {
            }
            catch(InvalidOperationException ex) 
            {
            }
            catch(Exception ex) 
            {
            }
        }
        #endregion

        #region Method

        /// <summary>
        /// 初期値設定
        /// </summary>
        private void InitialSetting()
        {
            // -----初期値設定------
            //this.TargetUser = "127.0.0.1";

            this.ConnectionStatus = StatusDisConnect;
        }

        /// <summary>
        /// メッセージ追加処理
        /// </summary>
        /// <param name="message">追加メッセージ</param>
        private void AddMessageToList(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                this.Messages.Add(message);
            });
        }
        #endregion

        #region Event Handler

        /// <summary>
        /// メッセージ受信時処理
        /// </summary>
        /// <param name="msg">受信メッセージ</param>
        private void OnMessageRecieved(string sender, ChatMessage msg) 
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var dispMessage = $"[{sender} --> {this.UserID}] : {msg.Message}";
                this.Messages.Add(dispMessage);
            });
        }

        /// <summary>
        /// 接続状態変更時処理
        /// </summary>
        /// <param name="status">変更後状態</param>
        private void OnConnectionStatusChanged(bool status) 
        {
            this.ConnectionStatus = status ? StatusConnect : StatusDisConnect;

            this.MessageAddOnStateChanged(status);
        }

        /// <summary>
        /// 接続状態変更時処理
        /// </summary>
        /// <param name="status">変更後状態</param>
        private void OnConnectFailed(string msg) 
        {
            this.AddMessageToList($"【Error】 {DateTime.Now} : {msg}");
        }

        /// <summary>
        /// 接続状態変更時のメッセージ追加処理
        /// </summary>
        /// <param name="newStatus">変更後状態</param>
        private void MessageAddOnStateChanged(bool newStatus)
        {
            if (newStatus)
            {
                this.AddMessageToList($"{DateTime.Now} : 接続完了");
            }
            else
            {
                this.AddMessageToList($"{DateTime.Now} : 切断されました");
            }
        }

        #endregion

    }
}
