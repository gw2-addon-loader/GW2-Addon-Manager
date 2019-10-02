using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
            theViewModel = new OpeningViewModel();
            DataContext = theViewModel;
            configuration.CheckSelfUpdates(theViewModel);
            configuration.DetermineSystemType();
            configuration.DisplayAddonStatus(theViewModel);
            InitializeComponent();
        }


        /**** What Add-On Is Selected ****/
        /// <summary>
        /// Takes care of description page text updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void addOnList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            AddonInfo selected = theViewModel.AddonList[addons.SelectedIndex];
            theViewModel.DescriptionText = selected.description;
            theViewModel.Developer = selected.developer;

            theViewModel.DeveloperVisibility = Visibility.Visible;
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
            List<AddonInfo> selectedAddons = new List<AddonInfo>();

            AddonInfo wrapper = AddonYamlReader.getAddonInInfo("d3d9_wrapper");
            wrapper.folder_name = "d3d9_wrapper";
            selectedAddons.Add(wrapper);

            foreach (AddonInfo addon in theViewModel.AddonList.Where(add => add.IsSelected == true))
            {
                selectedAddons.Add(addon);
            }

            Application.Current.Properties["Selected"] = selectedAddons;

            this.NavigationService.Navigate(new Uri("UI/UpdatingView.xaml", UriKind.Relative));
        }

        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
