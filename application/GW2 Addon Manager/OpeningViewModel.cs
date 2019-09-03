using GalaSoft.MvvmLight.Command;
using IWshRuntimeLibrary;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
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
        protected void propertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// This constructor initializes various default properties across the class and then
        /// applies any updated values to them using <c>ApplyDefaultConfig</c>.
        /// </summary>
        public OpeningViewModel()
        {
            AddonInfo arcdps = UpdateFileReader.getBuiltInInfo("arcdps");
            ArcDPS_Tooltip = arcdps.tooltip;

            AddonInfo d912pxy = UpdateFileReader.getBuiltInInfo("d912pxy");
            d912pxy_Tooltip = d912pxy.tooltip;

            AddonInfo gw2radial = UpdateFileReader.getBuiltInInfo("gw2radial");
            GW2Radial_Tooltip = gw2radial.tooltip;

            AddonInfo arcdps_mechanics = UpdateFileReader.getBuiltInInfo("arcdps_mechanics");
            arcdps_mechanics_Tooltip = arcdps_mechanics.tooltip;

            AddonInfo arcdps_bhud = UpdateFileReader.getBuiltInInfo("arcdps_bhud");
            arcdps_bhud_Tooltip = arcdps_bhud.tooltip;

            DescriptionText = "Select an add-on to see more information about it.";
            DeveloperVisibility = Visibility.Hidden;

            UpdateLinkVisibility = Visibility.Hidden;
            UpdateProgressVisibility = Visibility.Hidden;

            GamePath = configuration.getConfig().game_path;  

            /* applying any values from config.ini */
            configuration.ApplyDefaultConfig(this);
        }


        /***** Description Panel Text *****/
        private string _description;
        public string DescriptionText
        {
            get { return _description; }
            set { _description = value; propertyChanged("DescriptionText"); }
        }

        /***** "Developer" label *****/
        private string _developer;
        public string Developer
        {
            get { return _developer; }
            set { _developer = value; propertyChanged("Developer"); }
        }

        private Visibility _developer_visibility;
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
            
            get
            {
                
                return new RelayCommand<object>(param => Updates.UpdateSelf(this), true);
            }
        }

        /***** ************************** *****/


        /************************** ArcDPS **************************/
        private bool _arc_box;
        private string _arc_content;
        private string _arc_tooltip;
        private string _arc_desc;
        /// <summary>
        /// Binding for the ArcDPS checkbox's IsChecked property.
        /// </summary>
        public bool ArcDPS_CheckBox
        {
            get { return _arc_box; }
            set { _arc_box = value; propertyChanged("ArcDPS_CheckBox"); }
        }

        /// <summary>
        /// Binding for the ArcDPS checkbox's Content property.
        /// </summary>
        public string ArcDPS_Content
        {
            get { return _arc_content; }
            set { _arc_content = value; propertyChanged("ArcDPS_Content"); }
        }

        /// <summary>
        /// Binding for the ArcDPS checkbox tooltip.
        /// </summary>
        public string ArcDPS_Tooltip
        {
            get { return _arc_tooltip;  }
            set { _arc_tooltip = value; propertyChanged("ArcDPS_Tooltip"); }
        }

        /// <summary>
        /// Binding for the ArcDPS description (unused).
        /// </summary>
        public string ArcDPS_Description
        {
            get { return _arc_desc; }
            set { _arc_desc = value; propertyChanged("ArcDPS_Description"); }
        }




        /************************** ArcDPS Mechanics Plugin **************************/
        private bool _arcdps_mechanics_box;
        private string _arcdps_mechanics_content;
        private string _arcdps_mechanics_tooltip;
        private string _arcdps_mechanics_desc;
        /// <summary>
        /// The data binding for the ArcDPS checkbox's IsChecked property.
        /// </summary>
        public bool arcdps_mechanics_CheckBox
        {
            get { return _arcdps_mechanics_box; }
            set { _arcdps_mechanics_box = value; propertyChanged("arcdps_mechanics_CheckBox"); }
        }

        /// <summary>
        /// The data binding for the ArcDPS checkbox's Content property.
        /// </summary>
        public string arcdps_mechanics_Content
        {
            get { return _arcdps_mechanics_content; }
            set { _arcdps_mechanics_content = value; propertyChanged("arcdps_mechanics_Content"); }
        }

        /// <summary>
        /// Binding for the ArcDPS mechanics checkbox tooltip.
        /// </summary>
        public string arcdps_mechanics_Tooltip
        {
            get { return _arcdps_mechanics_tooltip; }
            set { _arcdps_mechanics_tooltip = value; propertyChanged("arcdps_mechanics_Tooltip"); }
        }

        /// <summary>
        /// Binding for the ArcDPS mechanics description (unused).
        /// </summary>
        public string arcdps_mechanics_Description
        {
            get { return _arcdps_mechanics_desc; }
            set { _arcdps_mechanics_desc = value; propertyChanged("arcdps_mechanics_Description"); }
        }



        /************************** GW2 Radial **************************/
        private bool _radial_box;
        private string _radial_content;
        private string _radial_tooltip;
        private string _radial_desc;

        /// <summary>
        /// The data binding for the GW2 Radial checkbox's IsChecked property.
        /// </summary>
        public bool GW2Radial_CheckBox
        {
            get { return _radial_box; }
            set { _radial_box = value; propertyChanged("GW2Radial_CheckBox"); }
        }
        /// <summary>
        /// The data binding for the GW2 Radial checkbox's Content property.
        /// </summary>
        public string GW2Radial_Content
        {
            get { return _radial_content; }
            set { _radial_content = value; propertyChanged("GW2Radial_Content"); }
        }

        public string GW2Radial_Tooltip
        {
            get { return _radial_tooltip; }
            set { _radial_tooltip = value; propertyChanged("GW2Radial_Tooltip"); }
        }

        public string GW2Radial_Description
        {
            get { return _radial_desc; }
            set { _radial_desc = value; propertyChanged("GW2Radial_Description"); }
        }

        /************************** d912pxy **************************/
        private bool _d912pxy_box;
        private string _d912pxy_content;
        private string _d912pxy_tooltip;
        private string _d912pxy_desc;
        /// <summary>
        /// The data binding for the d912pxy checkbox's IsChecked property.
        /// </summary>
        public bool d912pxy_CheckBox
        {
            get { return _d912pxy_box; }
            set { _d912pxy_box = value; propertyChanged("d912pxy_CheckBox"); }
        }
        /// <summary>
        /// The data binding for the d912pxy checkbox's Content property.
        /// </summary>
        public string d912pxy_Content
        {
            get { return _d912pxy_content; }
            set { _d912pxy_content = value; propertyChanged("d912pxy_Content"); }
        }

        public string d912pxy_Tooltip
        {
            get { return _d912pxy_tooltip; }
            set { _d912pxy_tooltip = value; propertyChanged("d912pxy_Tooltip"); }
        }

        public string d912pxy_Description
        {
            get { return _d912pxy_desc; }
            set { _d912pxy_desc = value; propertyChanged("d912pxy_Description"); }
        }


        /************************** arcdps bhud integration **************************/
        private bool _arcdps_bhud_box;
        private string _arcdps_bhud_content;
        private string _arcdps_bhud_tooltip;
        private string _arcdps_bhud_desc;

        /// <summary>
        /// The data binding for the arcdps bhud checkbox's IsChecked property.
        /// </summary>
        public bool arcdps_bhud_CheckBox
        {
            get { return _arcdps_bhud_box; }
            set { _arcdps_bhud_box = value; propertyChanged("arcdps_bhud_CheckBox"); }
        }
        /// <summary>
        /// The data binding for the arcdps bhud checkbox's Content property.
        /// </summary>
        public string arcdps_bhud_Content
        {
            get { return _arcdps_bhud_content; }
            set { _arcdps_bhud_content = value; propertyChanged("arcdps_bhud_Content"); }
        }

        public string arcdps_bhud_Tooltip
        {
            get { return _arcdps_bhud_tooltip; }
            set { _arcdps_bhud_tooltip = value; propertyChanged("arcdps_bhud_Tooltip"); }
        }

        public string arcdps_bhud_Description
        {
            get { return _arcdps_bhud_desc; }
            set { _arcdps_bhud_desc = value; propertyChanged("arcdps_bhud_Description"); }
        }

        /// <summary>
        /// Binding for the Content property of the text box displayed on the opening page.
        /// </summary>
        public string GamePath { get; set; }


        


        /***** Button Handlers *****/

        /// <summary>
        /// Handles button commands for the "set" button next to the game path text field in the opening screen.
        /// <see cref="configuration.SetGamePath(string)"/>
        /// </summary>
        public ICommand SetGamePath
        {
            get { return new RelayCommand<object>(param => configuration.SetGamePath(GamePath), true); }
        }

        /// <summary>
        /// Handles button commands for the button to make the current add-on selection the default.
        /// <see cref="configuration.ChangeAddonConfig(OpeningViewModel)"/>
        /// </summary>
        public ICommand SetDefaultAddons
        {
            get { return new RelayCommand<object>(param => configuration.ChangeAddonConfig(this), true); }
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
