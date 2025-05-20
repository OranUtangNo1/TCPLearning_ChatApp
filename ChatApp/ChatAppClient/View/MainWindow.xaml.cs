using ChatAppClient.ViewModel;
using System.Windows;

namespace ChatAppClient
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IVmlMainWindow viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
