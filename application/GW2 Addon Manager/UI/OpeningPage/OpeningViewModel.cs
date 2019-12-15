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
        /***** private fields for ui bindings *****/
        private ObservableCollection<AddonInfoFromYaml> _addonList;
        private ObservableCollection<int> _selectedAddons;
        private string _developer;
        private string _description;
        private string _addonwebsite;
        private Visibility _developer_visibility;
        private string _updateAvailable;
        private int updateProgress;
        private Visibility _updateLinkVisibility;
        private Visibility _updateProgressVisibility;

        /********** UI BINDINGS **********/

        /***** Addon List Box *****/
        /// <summary>
        /// The indices of the checked boxes in the list of addons displayed on the UI.
        /// </summary>
        public ObservableCollection<int> SelectedAddons
        {
            get { return _selectedAddons; }
            set { _selectedAddons = value; propertyChanged($"{nameof(SelectedAddons)}"); }
        }
        /// <summary>
        /// List of Addons
        /// </summary>
        public ObservableCollection<AddonInfoFromYaml> AddonList
        {
            get { return _addonList;  }
            set { _addonList = value; propertyChanged($"{nameof(AddonList)}"); }
        }
        
        /***** Description Panel *****/
        /// <summary>
        /// Text content for the description panel.
        /// </summary>
        public string DescriptionText
        {
            get { return _description; }
            set { _description = value; propertyChanged($"{nameof(DescriptionText)}"); }
        }
        /// <summary>
        /// The informational text showing the developer of the selected add-on.
        /// </summary>
        public string DeveloperText
        {
            get { return _developer; }
            set { _developer = value; propertyChanged($"{nameof(DeveloperText)}"); }
        }
        /// <summary>
        /// The website link in the description panel.
        /// </summary>
        public string AddonWebsiteLink
        {
            get { return _addonwebsite; }
            set { _addonwebsite = value; propertyChanged($"{nameof(AddonWebsiteLink)}"); }
        }

        /***** Show/Hide Elements *****/
        /// <summary>
        /// Visibility of the informational text showing the developer of the selected add-on.
        /// </summary>
        public Visibility DeveloperVisibility
        {
            get { return _developer_visibility; }
            set { _developer_visibility = value; propertyChanged($"{nameof(DeveloperVisibility)}"); }
        }
        /// <summary>
        /// A string representing a visibility value for the Github releases link.
        /// </summary>
        public Visibility UpdateLinkVisibility
        {
            get { return _updateLinkVisibility; }
            set { _updateLinkVisibility = value; propertyChanged($"{nameof(UpdateLinkVisibility)}"); }
        }
        /// <summary>
        /// A string representing a visibility value for the self-update download progress bar.
        /// </summary>
        public Visibility UpdateProgressVisibility
        {
            get { return _updateProgressVisibility; }
            set { _updateProgressVisibility = value; propertyChanged($"{nameof(UpdateProgressVisibility)}"); }
        }
        
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
            get { return new RelayCommand<object>(param => Configuration.ChangeAddonConfig(), true); }
        }
        /// <summary>
        /// Handles the disable selected addons button.
        /// </summary>
        public ICommand DisableSelected
        {
            get { return new RelayCommand<object>(param =>PluginManagement.DisableSelected(), true); }
        }
        /// <summary>
        /// Handles the enable selected addons button.
        /// </summary>
        public ICommand EnableSelected
        {
            get { return new RelayCommand<object>(param => PluginManagement.EnableSelected(), true); }
        }
        /// <summary>
        /// Handles the delete selected addons button.
        /// </summary>
        public ICommand DeleteSelected
        {
            get { return new RelayCommand<object>(param => PluginManagement.DeleteSelected(), true); }
        }
        /// <summary>
        /// Handler for small button to download application update.
        /// </summary>
        public ICommand DownloadSelfUpdate
        {
            get { return new RelayCommand<object>(param => SelfUpdate.Update(), true); }
        }
        /// <summary>
        /// Handles the create shortcut button under the options menu. <see cref="cs_logic"/>
        /// </summary>
        public ICommand CreateShortcut
        {
            get { return new RelayCommand<object>(param => cs_logic(), true); }
        }

        /***** Misc *****/
        /// <summary>
        /// Binding for the value shown in the mini progress bar displayed when downloading a new version of the application.
        /// </summary>
        public int UpdateDownloadProgress
        {
            get { return updateProgress; }
            set { updateProgress = value; propertyChanged("UpdateDownloadProgress"); }
        }
        /// <summary>
        /// Content of the text box that contains the game path the program is set to look for the game in.
        /// </summary>
        public string GamePath { get; set; }
        /// <summary>
        /// A string that is assigned a value if there is an update available.
        /// </summary>
        public string UpdateAvailable
        {
            get { return _updateAvailable; }
            set { _updateAvailable = value; propertyChanged("UpdateAvailable"); }
        }


        /********** Class Structure/Other Methods **********/

        /* Singleton Setup */
        private static OpeningViewModel onlyInstance;
        /// <summary>
        /// This constructor initializes various default properties across the class and then
        /// applies any updated values to them using <c>ApplyDefaultConfig</c>.
        /// </summary>
        private OpeningViewModel()
        {
            onlyInstance = this;

            AddonList = ApprovedList.GenerateAddonList();

            DescriptionText = "Select an add-on to see more information about it.";
            DeveloperVisibility = Visibility.Hidden;

            UpdateLinkVisibility = Visibility.Hidden;
            UpdateProgressVisibility = Visibility.Hidden;

            GamePath = Configuration.getConfigAsYAML().game_path;
        }
        /// <summary>
        /// Fetches the only instance of the OpeningViewModel and creates it if it has not been initialized yet.
        /// </summary>
        /// <returns>An instance of OpeningViewModel</returns>
        public static OpeningViewModel GetInstance
        { get { return (onlyInstance == null) ? new OpeningViewModel() : onlyInstance; } }

        /*** Notify UI of Changed Binding Items ***/
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
        //TODO: Move this to a more appropriate class and just use a ICommand that invokes it
        /* Button Helper */
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
