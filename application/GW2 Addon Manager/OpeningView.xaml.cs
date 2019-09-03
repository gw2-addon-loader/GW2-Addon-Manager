using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Code-behind for OpeningView.xaml.
    /// </summary>
    public partial class AddOnSelector : Page
    {
        OpeningViewModel theViewModel;

        /// <summary>
        /// This constructor deals with creating or expanding the configuration file, setting the DataContext, and checking for application updates.
        /// </summary>
        public AddOnSelector()
        {
            configuration.ConfigFileStatus();          
            theViewModel = new OpeningViewModel();
            DataContext = theViewModel;
            configuration.CheckSelfUpdates(theViewModel);
            configuration.DetermineSystemType();
            InitializeComponent();
        }


        /**** What Add-On Is Selected ****/
        public void addOnList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (addOnList.SelectedIndex)
            {
                
                case 0:
                    AddonInfo arc = UpdateFileReader.getBuiltInInfo("arcdps");
                    theViewModel.DescriptionText = arc.description;
                    theViewModel.Developer = arc.developer;
                    theViewModel.DeveloperVisibility = Visibility.Visible;
                    break;
                case 1:
                    AddonInfo radial = UpdateFileReader.getBuiltInInfo("gw2radial");
                    theViewModel.DescriptionText = radial.description;
                    theViewModel.Developer = radial.developer;
                    theViewModel.DeveloperVisibility = Visibility.Visible;
                    break;
                case 2:
                    AddonInfo d912pxy = UpdateFileReader.getBuiltInInfo("d912pxy");
                    theViewModel.DescriptionText = d912pxy.description;
                    theViewModel.Developer = d912pxy.developer;
                    theViewModel.DeveloperVisibility = Visibility.Visible;
                    break;
                case 3:
                    AddonInfo arcdps_bhud = UpdateFileReader.getBuiltInInfo("arcdps_bhud");
                    theViewModel.DescriptionText = arcdps_bhud.description;
                    theViewModel.Developer = arcdps_bhud.developer;
                    theViewModel.DeveloperVisibility = Visibility.Visible;
                    break;
                case 4:
                    AddonInfo arcdps_mechanics = UpdateFileReader.getBuiltInInfo("arcdps_mechanics");
                    theViewModel.DescriptionText = arcdps_mechanics.description;
                    theViewModel.Developer = arcdps_mechanics.developer;
                    theViewModel.DeveloperVisibility = Visibility.Visible;
                    break;
            }
        }

        /***************************** NAV BAR *****************************/
        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        

        //race condition with processes
        private void close_clicked(object sender, RoutedEventArgs e)
        {
            SelfUpdate.startUpdater();
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

            if (theViewModel.arcdps_bhud_CheckBox)
                Application.Current.Properties["arcdps_bhud"] = true;
            else
                Application.Current.Properties["arcdps_bhud"] = false;

            if (theViewModel.arcdps_mechanics_CheckBox)
                Application.Current.Properties["arcdps_mechanics"] = true;
            else
                Application.Current.Properties["arcdps_mechanics"] = false;

            this.NavigationService.Navigate(new Uri("UpdatingView.xaml", UriKind.Relative));
        }

        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
