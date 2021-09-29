using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GW2AddonManager.App.Configuration;
using Localization;

namespace GW2AddonManager
{
    /// <summary>
    /// <c>OpeningViewModel</c> serves as the DataContext for OpeningView.xaml, which is the first screen encountered upon opening the application.
    /// </summary>
    public class OpeningViewModel : INotifyPropertyChanged
    {
        private readonly IConfigurationProvider _configurationManager;
        private readonly AddonManager _pluginManagement;

        /********** UI BINDINGS **********/

        /***** Addon List Box *****/
        /// <summary>
        /// The indices of the checked boxes in the list of addons displayed on the UI.
        /// </summary>
        public ObservableCollection<int> SelectedAddons { get; set; }
        /// <summary>
        /// List of Addons
        /// </summary>
        public ObservableCollection<AddonInfo> AddonList { get; set; }

        /***** Description Panel *****/
        /// <summary>
        /// Text content for the description panel.
        /// </summary>
        public string DescriptionText { get; set; }
        /// <summary>
        /// The informational text showing the developer of the selected add-on.
        /// </summary>
        public string DeveloperText { get; set; }
        /// <summary>
        /// The website link in the description panel.
        /// </summary>
        public string AddonWebsiteLink { get; set; }

        /***** Show/Hide Elements *****/
        /// <summary>
        /// Visibility of the informational text showing the developer of the selected add-on.
        /// </summary>
        public Visibility DeveloperVisibility { get; set; }

        /***************************/
        /***** Button Handlers *****/
        /***************************/

        /* [Configuration Options] drop-down menu */
        
        /// <summary>
        /// Handles the disable selected addons button.
        /// </summary>
        public ICommand DisableSelected
        {
            get => new RelayCommand<object>(param => _pluginManagement.DisableSelected(), true);
        }
        /// <summary>
        /// Handles the enable selected addons button.
        /// </summary>
        public ICommand EnableSelected
        {
            get => new RelayCommand<object>(param => _pluginManagement.EnableSelected(), true);
        }
        /// <summary>
        /// Handles the delete selected addons button.
        /// </summary>
        public ICommand DeleteSelected
        {
            get => new RelayCommand<object>(param => _pluginManagement.DeleteSelected(), true);
        }
        /// <summary>
        /// Handles the Reset to Clean Install button.
        /// </summary>
        public ICommand CleanInstall
        {
            get => new RelayCommand<object>(param => _pluginManagement.DeleteAll(), true);
        }

        public bool AnyAddonSelected { get; set; } = false;

        public bool NoAddonSelected => !AnyAddonSelected;

        /******************************************/

        /// <summary>
        /// Content of the text box that contains the game path the program is set to look for the game in.
        /// </summary>
        public string GamePath
        {
            get => _configurationManager.UserConfig.GamePath;
            set
            {
                if (value == _configurationManager.UserConfig.GamePath) return;
                propertyChanged("GamePath");
                _configurationManager.UserConfig.GamePath = value;
                _configurationManager.Save();
            }
        }

        /********** Class Structure/Other Methods **********/

        /// <summary>
        /// This constructor initializes various default properties across the class and then
        /// applies any updated values to them using <c>ApplyDefaultConfig</c>.
        /// </summary>
        public OpeningViewModel(IConfigurationProvider configurationManager, AddonManager pluginManagement, AddonRepository approvedList)
        {
            _configurationManager = configurationManager;
            _pluginManagement = pluginManagement;

            AddonList = approvedList.GenerateAddonList();

            DescriptionText = StaticText.SelectAnAddonToSeeMoreInformationAboutIt;
            DeveloperVisibility = Visibility.Hidden;
        }

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
    }
}
