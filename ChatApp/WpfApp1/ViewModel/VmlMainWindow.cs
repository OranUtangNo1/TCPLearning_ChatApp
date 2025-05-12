using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Media3D;
using ChatAppCore;
using ChatAppCore.ChatModel.Interface;
using ChatAppCore.FrameWork;

namespace ChatAppClient.ViewModel
{
    internal class VmlMainWindow : ViewModelBase,IVmlMainWindow
    {

        #region Const

        /// <summary>status</summary>
        private const string StatusConnect = "接続中";
        private const string StatusDisConnect = "切断中";

        #endregion

        #region  Private Filed

        /// <summary>ip</summary>
        private string _targetIp;

        /// <summary>port</summary>
        private string _targetPort;

        /// <summary>status</summary>
        private string _connectionStatus;

        /// <summary>sendText</summary>
        private string _inputText = String.Empty;

        /// <summary>listBox messages</summary>
        private ObservableCollection<string> _messages = new ObservableCollection<string>();

        /// <summary>chat Func</summary>
        private IChatModels chatModel = null;

        #endregion

        #region  View Propertys

        /// <summary>IPAddres TxtB</summary>
        public string TargetIP 
        {
            get => _targetIp;
            set => this.SetProperty(ref this._targetIp, value);
        }

        /// <summary>Post TxtB</summary>
        public string TargetPort
        {
            get => _targetPort;
            set => this.SetProperty(ref this._targetPort, value);
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

        /// <summary>Message List</summary>
        public ObservableCollection<string> Messages 
        {
            get => _messages;
            set => this.SetProperty(ref this._messages, value);
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

        public VmlMainWindow(IChatModels chatModel)
        {
            // ======= 初期値設定  =====================
            this.InitialSetting();


            // ======= コマンド登録  =====================
            this.ConnectCommand = new RelayCommand(Connect);
            this.DisConnectCommand = new RelayCommand(DisConnect);
            this.SendCommand = new RelayCommand(Send);


            // ======= インスタンス登録  =====================
            this.chatModel = chatModel;

            // ======= イベントハンドラ登録  =====================
            // メッセージ受信
            this.chatModel.MessageRecieved += msg => this.OnMessageRecieved(msg);

            // 接続状態変更
            this.chatModel.ConnectionStatusChanged += status => this.OnConnectionStatusChanged(status);

        }

        #endregion

        #region Method

        private void InitialSetting() 
        {
            // -----初期値設定------
            this.TargetIP = "127.0.0.1";
            this.TargetPort = "5000";
            this.ConnectionStatus = StatusDisConnect;
        }

        /// <summary>Connect</summary>
        private void Connect()
        {
            this.chatModel.ConnectAsync(this.TargetIP,int.Parse(this.TargetPort));
        }

        /// <summary>DisConnect</summary>
        private void DisConnect()
        {
            this.chatModel.DisConnectAsync();
        }

        /// <summary>Send</summary>
        private void Send()
        {
            this.chatModel.SendMessageAsync(this.InputText);
            this.InputText = string.Empty;
        }

        #endregion

        #region Event Handler

        private void OnMessageRecieved(Message msg) 
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var dispMessage = $"{msg.Timestamp}  --> 受 ( {msg.SenderIP} )   {msg.Content}";
                this.Messages.Add(dispMessage);
            });
        }

        private void OnConnectionStatusChanged(bool status) 
        {
            this.ConnectionStatus = status ? StatusConnect : StatusDisConnect;
        }

        #endregion

    }
}
