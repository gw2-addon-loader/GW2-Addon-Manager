using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YamlDotNet.Serialization;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Interaction logic for Updating.xaml
    /// </summary>
    public partial class UpdatingView : Page
    {

        /// <summary>
        /// Sets the page's DataContext, initializes it, and begins the update process.
        /// </summary>
        public UpdatingView()
        {
            DataContext = UpdatingViewModel.GetInstance;
            InitializeComponent();

            LoaderSetup settingUp = new LoaderSetup();
            Task.Run(() => UpdateHelpers.UpdateAll());

            launchOnClose.IsChecked = Configuration.getConfigAsYAML().launch_game;
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
            SelfUpdate.startUpdater();

            if ((bool)launchOnClose.IsChecked)
            {
                string exeLocation = Path.Combine(Configuration.getConfigAsYAML().game_path, Configuration.getConfigAsYAML().exe_name);
                try
                {
                    Process.Start(exeLocation, "-autologin");
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show($"Unable to launch game as {Configuration.getConfigAsYAML().exe_name} is missing.",
                                     "Unable to Launch Game",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
            }

            UserConfig config = Configuration.getConfigAsYAML();
            if (config.launch_game != (bool)launchOnClose.IsChecked)
            {
                config.launch_game = (bool)launchOnClose.IsChecked;
                Configuration.setConfigAsYAML(config);
            }

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

