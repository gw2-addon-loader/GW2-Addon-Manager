using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using GW2AddonManager.App.Configuration;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using System.Threading;
using System.Globalization;
using System.Windows.Markup;

namespace GW2AddonManager
{
    public partial class OpeningView
    {
        public OpeningView()
        {
            _configurationManager = new ConfigurationProvider();
            var configuration = new Configuration(_configurationManager);
            configuration.CheckSelfUpdates();
            configuration.DetermineSystemType();
            _pluginManagement = new PluginManagement(_configurationManager);
            _pluginManagement.DisplayAddonStatus();

            DataContext = OpeningViewModel.GetInstance;

            InitializeComponent();
            //update notification
            if (File.Exists(UpdateNotificationFile))
            {
                Process.Start(releases_url);
                File.Delete(UpdateNotificationFile);
            }
        }

        /**** What Add-On Is Selected ****/
        public void addOnList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            AddonInfo selected = e.AddedItems.Count > 0 ? e.AddedItems[0] as AddonInfo : null;
            OpeningViewModel.GetInstance.DescriptionText = selected?.description;
            OpeningViewModel.GetInstance.DeveloperText = selected?.developer;
            OpeningViewModel.GetInstance.AddonWebsiteLink = selected?.website;

            OpeningViewModel.GetInstance.DeveloperVisibility = selected != null ? Visibility.Visible : Visibility.Hidden;
            OpeningViewModel.GetInstance.AnyAddonSelected = addons.SelectedItems.Count > 0;
        }

        private void SelectDirectoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var pathSelectionDialog = new CommonOpenFileDialog();
            pathSelectionDialog.IsFolderPicker = true;
            CommonFileDialogResult result = pathSelectionDialog.ShowDialog();
            if (result == (CommonFileDialogResult)1)
                OpeningViewModel.GetInstance.GamePath = pathSelectionDialog.FileName;

        }

        //just calls PluginManagement.ForceRedownload(); and then update_button_clicked
        private void RedownloadAddons(object sender, RoutedEventArgs e)
        {
            if (_pluginManagement.ForceRedownload())
                update_button_clicked(sender, e);
        }


        /***** UPDATE button *****/
        private void update_button_clicked(object sender, RoutedEventArgs e)
        {
            //If bin folder doesn't exist then LoaderSetup intialization will fail.
            if (_configurationManager.UserConfig.BinFolder == null) {
                MessageBox.Show("Unable to locate Guild Wars 2 /bin/ or /bin64/ folder." + Environment.NewLine + "Please verify Game Path is correct.",
                                "Unable to Update", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<AddonInfo> selectedAddons = new List<AddonInfo>();

            //the d3d9 wrapper is installed by default and hidden from the list displayed to the user, so it has to be added to this list manually
            AddonInfo wrapper = AddonYamlReader.GetAddonInInfo("d3d9_wrapper");
            wrapper.folder_name = "d3d9_wrapper";
            selectedAddons.Add(wrapper);

            foreach (AddonInfo addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true)) {
                selectedAddons.Add(addon);
            }

            Application.Current.Properties["Selected"] = selectedAddons;

            NavigationService.Navigate(new Uri("UI/UpdatingPage/UpdatingView.xaml", UriKind.Relative));
        }

        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void addons_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Escape) {
                addons.SelectedItems.Clear();
            }
        }
    }
}
