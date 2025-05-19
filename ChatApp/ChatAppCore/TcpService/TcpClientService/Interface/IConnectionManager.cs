using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatAppCore
{
    /// <summary>
    /// TCP/IP接続管理インターフェース
    /// </summary>
    public interface IConnectionManager : IDisposable
    {
        /// <summary>
        /// 接続先のIPアドレス
        /// </summary>
        string ConnectionIP { get; }

        /// <summary>
        /// 接続先のポート番号
        /// </summary>
        int ConnectionPort { get; }

        /// <summary>
        /// 接続状態が変更された時に発生するイベント
        /// </summary>
        event Action<bool, string> ConnectionStatusChanged;

        /// <summary>
        /// 接続失敗時に発生するイベント
        /// </summary>
        event Action<string> ConnectFailed;

        /// <summary>
        /// 非同期に接続を試みる
        /// </summary>
        /// <param name="ip">接続先IPアドレス</param>
        /// <param name="port">接続先ポート番号</param>
        /// <returns>接続に成功した場合はtrue、それ以外はfalse</returns>
        Task<bool> ConnectAsync(string ip, int port);

        /// <summary>
        /// 接続を切断する
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Stream読み出し
        /// </summary>
        NetworkStream GetStream();

        /// <summary>
        /// アクティブな接続があるかを確認する
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// キャンセレーショントークンを取得する
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// steram
        /// </summary>
        NetworkStream NetworkStream { get; }
    }
}