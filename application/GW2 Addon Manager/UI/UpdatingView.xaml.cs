using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        UpdatingViewModel theViewModel;

        /// <summary>
        /// Sets the page's DataContext, initializes it, and begins the update process.
        /// </summary>
        public UpdatingView()
        {
            theViewModel = new UpdatingViewModel();
            DataContext = theViewModel;
            InitializeComponent();

            //Task.Run(() => UpdateHelpers.UpdateAll(theViewModel));
            UpdateHelpers.UpdateAll(theViewModel);
            //do this AFTER task is finished
            theViewModel.label = "Finished";
            theViewModel.showProgress = 100;
            theViewModel.closeButtonEnabled = true;
        }

        /***************************** Titlebar Window Drag *****************************/
        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        }

        /***************************** Button Controls *****************************/

        private void close_clicked(object sender, RoutedEventArgs e)
        {
            SelfUpdate.startUpdater();
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
    }
}

