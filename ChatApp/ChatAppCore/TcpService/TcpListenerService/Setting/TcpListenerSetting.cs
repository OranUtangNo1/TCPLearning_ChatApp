using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppCore.TcpService.TcpListenerService.Setting
{
    internal class TcpListenerSetting
    {

        /// <summary>
        /// 接続サーバーのIPアドレス
        /// </summary>
        public string ServerIP { get; } = "127.0.0.1";

        /// <summary>
        /// 接続サーバーのポート番号
        /// </summary>
        public int Port { get;} = 4000;


    }
}
