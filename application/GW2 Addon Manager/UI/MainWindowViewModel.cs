using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GW2_Addon_Manager.App.Configuration;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// <c>MainWindowViewModel</c> serves as the DataContext for MainWindow.xaml, which is the main layout for the application.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IConfigurationProvider _configurationManager;
        private readonly Configuration _configuration;
        private readonly SelfUpdate _selfUpdate;

        /// <summary>
        /// A string representing a visibility value for the Github releases link.
        /// </summary>
        public Visibility UpdateLinkVisibility { get; set; }
        /// <summary>
        /// A string representing a visibility value for the self-update download progress bar.
        /// </summary>
        public Visibility UpdateProgressVisibility { get; set; }

        /// <summary>
        /// Handles the Change Language buttons.
        /// </summary>
        public ICommand ChangeLanguage
        {
            get => new RelayCommand<string>(param => _configuration.SetCulture(param), true);
        }

        /// <summary>
        /// Handler for small button to download application update.
        /// </summary>
        public ICommand DownloadSelfUpdate
        {
            get => new RelayCommand<object>(param => {
                UpdateProgressVisibility = Visibility.Visible;
                UpdateLinkVisibility = Visibility.Hidden;
                _selfUpdate.Update();
                }, true);
        }

        /***** Misc *****/
        /// <summary>
        /// Binding for the value shown in the mini progress bar displayed when downloading a new version of the application.
        /// </summary>
        public int UpdateDownloadProgress { get; set; }

        /// <summary>
        /// A string that is assigned a value if there is an update available.
        /// </summary>
        public string UpdateAvailable { get; set; }

        /// <summary>
        /// This constructor initializes various default properties across the class and then
        /// applies any updated values to them using <c>ApplyDefaultConfig</c>.
        /// </summary>
        public MainWindowViewModel(IConfigurationProvider configurationManager, SelfUpdate selfUpdate)
        {
            _configurationManager = configurationManager;
            _configuration = new Configuration(_configurationManager);
            _selfUpdate = selfUpdate;

            UpdateLinkVisibility = Visibility.Hidden;
            UpdateProgressVisibility = Visibility.Hidden;

            _selfUpdate.UpdateMessageChanged += (obj, msg) => UpdateAvailable = msg;
            _selfUpdate.UpdateProgressChanged += (obj, pct) => UpdateDownloadProgress = pct;
        }

        /// <summary>
        /// An event used to indicate that a property's value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}