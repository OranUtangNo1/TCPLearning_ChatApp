using Microsoft.Extensions.DependencyInjection;

namespace ChatAppCore
{
    /// <summary>
    /// アプリケーションで使用するサービスを登録するクラス
    /// </summary>
    public static class ServiceRegistration
    {
        /// <summary>
        /// サービスをDIコンテナに登録する
        /// </summary>
        /// <param name="services">DIコンテナ</param>
        /// <returns>DIコンテナ</returns>
        public static IServiceCollection RegisterChatServices(this IServiceCollection services)
        {
            // TCP/IP通信関連のサービスを登録
            services.AddSingleton<TcpClientSettings>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<IMessageTransceiver, MessageTransceiver>();
            services.AddSingleton<IMessageParser, MessageParser>();
            services.AddSingleton<ITcpClientService, TcpClientFacade>();

            return services;
        }
    }
}