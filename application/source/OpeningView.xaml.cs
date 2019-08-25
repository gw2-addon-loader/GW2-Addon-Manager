using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddOnSelector : Page
    {
        OpeningViewModel theViewModel;

        /* page initialization */
        public AddOnSelector()
        {
            configuration.SelfVersionStatus();
            theViewModel = new OpeningViewModel();
            DataContext = theViewModel;
            InitializeComponent();
        }

        /***************************** NAV BAR *****************************/
        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        
        private void close_clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void minimize_clicked(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).WindowState = WindowState.Minimized;
        }

        /*****************************   ***   *****************************/





        /***** UPDATE button *****/
        private void update_button_clicked(object sender, RoutedEventArgs e)
        {
            if (theViewModel.ArcDPS_CheckBox)
                Application.Current.Properties["ArcDPS"] = true;
            else
                Application.Current.Properties["ArcDPS"] = false;

            if (theViewModel.GW2Radial_CheckBox)
                Application.Current.Properties["GW2Radial"] = true;
            else
                Application.Current.Properties["GW2Radial"] = false;

            if (theViewModel.d912pxy_CheckBox)
                Application.Current.Properties["d912pxy"] = true;
            else
                Application.Current.Properties["d912pxy"] = false;

            this.NavigationService.Navigate(new Uri("UpdatingView.xaml", UriKind.Relative));
        }
    }
}
