using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using ChatAppClient.Model;
using ChatAppCore.FrameWork;

namespace ChatAppClient.ViewModel
{
    internal class VmlMainWindow : IVmlMainWindow,INotifyPropertyChanged
    {
        private const string StatusConnect = "接続中";
        private const string StatusDisConnect = "切断中";

        private string _targetIp;
        private string _targetPort;
        private string _connectionStatus;
        private string _inputText = String.Empty;

        private ObservableCollection<string> _messages = new ObservableCollection<string>();

        private IChatModel chatModel = null;

        public string TargetIP 
        {
            get => _targetIp;
            set => this.SetProperty(ref this._targetIp, value);
        }
        public string TargetPort
        {
            get => _targetPort;
            set => this.SetProperty(ref this._targetPort, value);
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => this.SetProperty(ref this._connectionStatus, value);
        }
        public string InputText
        {
            get => _inputText;
            set => this.SetProperty(ref this._inputText, value);
        }

        public ObservableCollection<string> Messages 
        {
            get => _messages;
            set => this.SetProperty(ref this._messages, value);
        }

        public ICommand ConnectCommand { get; }
        public ICommand DisConnectCommand { get; }
        public ICommand SendCommand { get; }


        public VmlMainWindow(IChatModel chatModel) 
        {
            this.TargetIP = "127.0.0.1";
            this.TargetPort = "5000";
            this.ConnectionStatus = StatusDisConnect;

            this.ConnectCommand = new RelayCommand(Connect);
            this.DisConnectCommand = new RelayCommand(DisConnect);
            this.SendCommand = new RelayCommand(Send);

            this.chatModel = chatModel;

            // イベントハンドラ登録
            this.chatModel.MessageRecieved += msg => this.Messages.Add(msg);
        }

        private void Connect()
        {
            this.chatModel.ConnectAsync(this.TargetIP,int.Parse(this.TargetPort));
        }

        private void DisConnect()
        {
            this.chatModel.DisConnectAsync();
        }

        private void Send()
        {
            MessageBox.Show("Send");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T val, [CallerMemberName] string propertyName = null) 
        {
            if (!Equals(field, val)) 
            {
                field = val;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
