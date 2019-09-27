using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            this.NavigationService.Navigate(new Uri("UI//UpdatingView.xaml", UriKind.Relative));
            List<AddonInfo> selectedAddons = new List<AddonInfo>();

            foreach (ListBoxItem addon in addons.Items)
            {
                if (addon.IsSelected)
                {
                    selectedAddons.Add(theViewModel.AddonList[addons.Items.IndexOf(addon)]);
                }
            }

            Application.Current.Properties["Selected"] = selectedAddons;

        }

        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /***** Checkbox checked handler *****/
        //a bit hacky but should work
        private void TheCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<int> selected = new ObservableCollection<int>();
            for (int i = 0; i < addons.Items.Count; i++)
            {
                CheckBox current = (CheckBox)addons.Items.GetItemAt(i);
                if ((bool)current.IsChecked)
                {
                    selected.Add(i);
                }
            }
            theViewModel.SelectedAddons = selected;
        }
    }
}
