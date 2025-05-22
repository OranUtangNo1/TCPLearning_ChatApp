using ChatAppCore;
using ChatAppServer;
using ChatServer;
using System.Runtime.CompilerServices;

class Program
{
    static async Task Main(string[] args)
    {
        // 必要な依存インスタンスの生成（ダミーやモックなど適宜）
        var clientManager = new ClientManager();
        var messageRouter = new MessageRouter(clientManager);
        var setting = new TcpListenerSetting();

        // サーバー起動
        var server = new ChatServer.ChatServer(clientManager, messageRouter, setting);
        server.ClientConnected += Console.WriteLine;
        server.ClientDisConnected += Console.WriteLine;

        Console.WriteLine("ChatServer is running. Press Enter to stop.");

        await server.Open();
    }
}
