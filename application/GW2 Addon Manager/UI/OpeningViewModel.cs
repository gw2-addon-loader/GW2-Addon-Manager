using GalaSoft.MvvmLight.Command;
using IWshRuntimeLibrary;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using File = System.IO.File;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// <c>OpeningViewModel</c> serves as the DataContext for OpeningView.xaml, which is the first screen encountered upon opening the application.
    /// </summary>
    public class OpeningViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// An event used to indicate that a property's value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A method used in notifying that a property's value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed value.</param>
        public void propertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private ObservableCollection<int> _selectedAddons;
        /// <summary>
        /// The indices of the checked boxes in the list of addons displayed on the UI.
        /// </summary>
        public ObservableCollection<int> SelectedAddons
        {
            get { return _selectedAddons; }
            set { _selectedAddons = value; propertyChanged("SelectedAddons"); }
        }

        private ObservableCollection<AddonInfo> _addonList;
        /// <summary>
        /// List of Addons
        /// </summary>
        public ObservableCollection<AddonInfo> AddonList
        {
            get { return _addonList;  }
            set { _addonList = value; propertyChanged("AddonList"); }
        }


        /// <summary>
        /// This constructor initializes various default properties across the class and then
        /// applies any updated values to them using <c>ApplyDefaultConfig</c>.
        /// </summary>
        public OpeningViewModel()
        {
            AddonList = ApprovedList.GenerateAddonList();

            DescriptionText = "Select an add-on to see more information about it.";
            DeveloperVisibility = Visibility.Hidden;

            UpdateLinkVisibility = Visibility.Hidden;
            UpdateProgressVisibility = Visibility.Hidden;

            GamePath = Configuration.getConfigAsYAML().game_path;
        }


        /***** Description Panel Text *****/
        private string _description;
        /// <summary>
        /// Text content for the description panel.
        /// </summary>
        public string DescriptionText
        {
            get { return _description; }
            set { _description = value; propertyChanged("DescriptionText"); }
        }

        /***** "Developer" label *****/
        private string _developer;
        /// <summary>
        /// The informational text showing the developer of the selected add-on.
        /// </summary>
        public string Developer
        {
            get { return _developer; }
            set { _developer = value; propertyChanged("Developer"); }
        }

        /*** Link to Addon Website ***/
        private string _addonwebsite;
        /// <summary>
        /// The website link in the description panel.
        /// </summary>
        public string AddonWebsite
        {
            get { return _addonwebsite; }
            set { _addonwebsite = value; propertyChanged("AddonWebsite"); }
        }

        private Visibility _developer_visibility;
        /// <summary>
        /// Visibility of the informational text showing the developer of the selected add-on.
        /// </summary>
        public Visibility DeveloperVisibility
        {
            get { return _developer_visibility; }
            set { _developer_visibility = value; propertyChanged("DeveloperVisibility"); }
        }



        /***** Application Version Status *****/

        string _updateAvailable;
        /// <summary>
        /// A string that is assigned a value if there is an update available.
        /// </summary>
        public string UpdateAvailable
        {
            get { return _updateAvailable; }
            set { _updateAvailable = value; propertyChanged("UpdateAvailable"); }
        }

        Visibility _updateLinkVisibility;
        /// <summary>
        /// A string representing a visibility value for the Github releases link.
        /// </summary>
        public Visibility UpdateLinkVisibility
        {
            get { return _updateLinkVisibility; }
            set { _updateLinkVisibility = value; propertyChanged("UpdateLinkVisibility"); }
        }

        Visibility _updateProgressVisibility;
        /// <summary>
        /// A string representing a visibility value for the self-update download progress bar.
        /// </summary>
        public Visibility UpdateProgressVisibility
        {
            get { return _updateProgressVisibility; }
            set { _updateProgressVisibility = value; propertyChanged("UpdateProgressVisibility"); }
        }

        int updateProgress;
        /// <summary>
        /// Binding for the value shown in the mini progress bar displayed when downloading a new version of the application.
        /// </summary>
        public int UpdateDownloadProgress
        {
            get { return updateProgress; }
            set { updateProgress = value; propertyChanged("UpdateDownloadProgress"); }
        }

        /// <summary>
        /// Handler for small button to download application update.
        /// </summary>
        public ICommand DownloadSelfUpdate
        {
            get { return new RelayCommand<object>(param => UpdateHelpers.UpdateSelf(this), true); }
        }

        /***** ************************** *****/

        /// <summary>
        /// Binding for the Content property of the text box displayed on the opening page.
        /// </summary>
        public string GamePath { get; set; }


        /***** Button Handlers *****/

        /// <summary>
        /// Handles button commands for the "set" button next to the game path text field in the opening screen.
        /// <see cref="Configuration.SetGamePath(string)"/>
        /// </summary>
        public ICommand SetGamePath
        {
            get { return new RelayCommand<object>(param => Configuration.SetGamePath(GamePath), true); }
        }

        /// <summary>
        /// Handles button commands for the button to make the current add-on selection the default.
        /// <see cref="Configuration.ChangeAddonConfig(OpeningViewModel)"/>
        /// </summary>
        public ICommand SetDefaultAddons
        {
            get { return new RelayCommand<object>(param => Configuration.ChangeAddonConfig(this), true); }
        }

        /// <summary>
        /// Handles the disable selected addons button.
        /// </summary>
        public ICommand DisableSelected
        {
            get { return new RelayCommand<object>(param =>PluginManagement.DisableSelected(this), true); }
        }

        /// <summary>
        /// Handles the enable selected addons button.
        /// </summary>
        public ICommand EnableSelected
        {
            get { return new RelayCommand<object>(param => PluginManagement.EnableSelected(this), true); }
        }

        /// <summary>
        /// Handles the delete selected addons button.
        /// </summary>
        public ICommand DeleteSelected
        {
            get { return new RelayCommand<object>(param => PluginManagement.DeleteSelected(this), true); }
        }
        

        /// <summary>
        /// Handles the create shortcut button under the options menu. <see cref="cs_logic"/>
        /// </summary>
        public ICommand CreateShortcut
        {
            get { return new RelayCommand<object>(param => cs_logic(), true); }
        }

        /// <summary>
        /// Creates a shortcut in the current user's start menu.
        /// </summary>
        //see the accepted answer at https://stackoverflow.com/questions/25024785/how-to-create-start-menu-shortcut
        //- I did some modifications (based on another SO question) to avoid admin access requirement
        private void cs_logic()
        {
            string appPath = Directory.GetCurrentDirectory();
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string appShortcutPath = Path.Combine(startMenuPath, @"GW2-UOAOM");
            string shortcutNamePath = Path.Combine(appShortcutPath, "GW2 Addon Manager" + ".lnk");

            if (!Directory.Exists(appShortcutPath))
                Directory.CreateDirectory(appShortcutPath);

            if (!File.Exists(shortcutNamePath))
            {
                WshShell quickShell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)quickShell.CreateShortcut(shortcutNamePath);
                shortcut.Description = "The Guild Wars 2 Unofficial Add-On Manager";
                shortcut.IconLocation = Path.Combine(appPath, "resources\\logo.ico");
                shortcut.WorkingDirectory = appPath;
                shortcut.TargetPath = Path.Combine(appPath, "GW2 Addon Manager" + ".exe");
                shortcut.Save();
            }
        }
    }
}
