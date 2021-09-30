using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Localization;

namespace GW2AddonManager
{
    public partial class UpdatingView
    {
        private readonly IConfigurationProvider _configurationManager;

        public UpdatingView()
        {
            _configurationManager = new ConfigurationProvider();
            DataContext = UpdatingViewModel.GetInstance;
            InitializeComponent();

            LoaderManager settingUp = new LoaderSetup(new ConfigurationProvider());
            Task.Run(() => UpdateHelpers.UpdateAll());

            launchOnClose.IsChecked = _configurationManager.UserConfig.LaunchGame;
        }

        /***************************** Titlebar Window Drag *****************************/
        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        /***************************** Button Controls *****************************/

        private void close_clicked(object sender, RoutedEventArgs e)
        {
            SelfManager.startUpdater();

            if ((bool)launchOnClose.IsChecked)
            {
                string exeLocation = Path.Combine(_configurationManager.UserConfig.GamePath, _configurationManager.UserConfig.ExeName);
                try
                {
                    Process.Start(exeLocation, "-autologin");
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show($"Unable to launch game as {_configurationManager.UserConfig.ExeName} is missing.",
                                     "Unable to Launch Game",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
            }

            if (_configurationManager.UserConfig.LaunchGame != launchOnClose.IsChecked)
            {
                _configurationManager.UserConfig.LaunchGame = (bool)launchOnClose.IsChecked;
                _configurationManager.Save();
            }

            Application.Current.Shutdown();
        }

        private void xbutton_clicked(object sender, RoutedEventArgs e)
        {
            SelfManager.startUpdater();
            System.Windows.Application.Current.Shutdown();
        }

        private void minimize_clicked(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).WindowState = WindowState.Minimized;
        }


        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void back_clicked(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("UI/OpeningPage/OpeningView.xaml", UriKind.Relative));
        }

    }
}

