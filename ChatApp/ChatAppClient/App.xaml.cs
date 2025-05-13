using ChatAppClient;
using ChatAppClient.Models;
using ChatAppClient.ViewModel;
using ChatAppCore;
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
            // SubSystems
            services.RegisterChatServices();

            // Models
            services.AddSingleton<IMessageManager, MessageManager>();
            services.AddSingleton<IClientChatModels, ClientChatModels>();

            // ViewModels
            services.AddSingleton<IVmlMainWindow, VmlMainWindow>();

            // Views
            services.AddSingleton<MainWindow>();
        }
    }
}
