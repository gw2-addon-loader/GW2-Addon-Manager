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
using GW2_Addon_Manager.App.Configuration;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using System.Threading;
using System.Globalization;
using System.Windows.Markup;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Code-behind for OpeningView.xaml.
    /// </summary>
    public partial class OpeningView
    {
        static string releases_url = "https://github.com/fmmmlee/GW2-Addon-Manager/releases";
        static string UpdateNotificationFile = "updatenotification.txt";

        /// <summary>
        /// This constructor deals with creating or expanding the configuration file, setting the DataContext, and checking for application updates.
        /// </summary>
        public OpeningView()
        {
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

        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
