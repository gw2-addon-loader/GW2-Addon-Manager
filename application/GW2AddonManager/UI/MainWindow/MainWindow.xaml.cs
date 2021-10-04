using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace GW2AddonManager
{
    public partial class MainWindow : Window, IHyperlinkHandler
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<MainWindowViewModel>();
        }

        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender && e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void close_clicked(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void minimize_clicked(object sender, RoutedEventArgs e) => (Parent as Window).WindowState = WindowState.Minimized;
    }
}
