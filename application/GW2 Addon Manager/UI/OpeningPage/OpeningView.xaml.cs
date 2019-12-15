using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Code-behind for OpeningView.xaml.
    /// </summary>
    public partial class OpeningView : Page
    {
        static string releases_url = "https://github.com/fmmmlee/GW2-Addon-Manager/releases";
        static string UpdateNotificationFile = "updatenotification.txt";

        /// <summary>
        /// This constructor deals with creating or expanding the configuration file, setting the DataContext, and checking for application updates.
        /// </summary>
        public OpeningView()
        {
            DataContext = OpeningViewModel.GetInstance;
            Configuration.CheckSelfUpdates();
            Configuration.DetermineSystemType();
            Configuration.DisplayAddonStatus();
            InitializeComponent();
            //update notification
            if (File.Exists(UpdateNotificationFile))
            {
                Process.Start(releases_url);
                File.Delete(UpdateNotificationFile);
            }
        }


        /**** What Add-On Is Selected ****/
        /// <summary>
        /// Takes care of description page text updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void addOnList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            AddonInfoFromYaml selected = OpeningViewModel.GetInstance.AddonList[addons.SelectedIndex];
            OpeningViewModel.GetInstance.DescriptionText = selected.description;
            OpeningViewModel.GetInstance.DeveloperText = selected.developer;
            OpeningViewModel.GetInstance.AddonWebsiteLink = selected.website;

            OpeningViewModel.GetInstance.DeveloperVisibility = Visibility.Visible;
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
            List<AddonInfoFromYaml> selectedAddons = new List<AddonInfoFromYaml>();

            AddonInfoFromYaml wrapper = AddonYamlReader.getAddonInInfo("d3d9_wrapper");
            wrapper.folder_name = "d3d9_wrapper";
            selectedAddons.Add(wrapper);

            foreach (AddonInfoFromYaml addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true))
            {
                selectedAddons.Add(addon);
            }

            Application.Current.Properties["Selected"] = selectedAddons;

            this.NavigationService.Navigate(new Uri("UI/UpdatingPage/UpdatingView.xaml", UriKind.Relative));
        }

        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
