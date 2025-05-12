using ChatAppClient;
using ChatAppClient.ViewModel;
using ChatAppCore;
using ChatAppCore.ChatModel.Interface;
using ChatAppCore.TcpService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ChatAppClient
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e) 
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();
            var mainWindow = provider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services) 
        {
            services.AddSingleton<ITcpClientService, TcpClientService>();
            services.AddSingleton<IChatModels, ChatModels>();
            services.AddSingleton<IVmlMainWindow, VmlMainWindow>();

            services.AddSingleton<MainWindow>();
        }
    }
}
