using GalaSoft.MvvmLight.Command;
using IWshRuntimeLibrary;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GW2_Addon_Manager.App.Configuration;
using File = System.IO.File;
using Localization;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// <c>MainWindowViewModel</c> serves as the DataContext for MainWindow.xaml, which is the main layout for the application.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly Configuration _configuration;
        private string _updateAvailable;
        private int updateProgress;
        private Visibility _updateLinkVisibility;
        private Visibility _updateProgressVisibility;

        /// <summary>
        /// A string representing a visibility value for the Github releases link.
        /// </summary>
        public Visibility UpdateLinkVisibility
        {
            get => _updateLinkVisibility;
            set => SetProperty(ref _updateLinkVisibility, value);
        }
        /// <summary>
        /// A string representing a visibility value for the self-update download progress bar.
        /// </summary>
        public Visibility UpdateProgressVisibility
        {
            get => _updateProgressVisibility;
            set => SetProperty(ref _updateProgressVisibility, value);
        }

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
            get => new RelayCommand<object>(param => SelfUpdate.Update(), true);
        }

        /***** Misc *****/
        /// <summary>
        /// Binding for the value shown in the mini progress bar displayed when downloading a new version of the application.
        /// </summary>
        public int UpdateDownloadProgress
        {
            get => updateProgress;
            set => SetProperty(ref updateProgress, value);
        }

        /// <summary>
        /// A string that is assigned a value if there is an update available.
        /// </summary>
        public string UpdateAvailable
        {
            get => _updateAvailable;
            set => SetProperty(ref _updateAvailable, value);
        }

        /* Singleton Setup */
        private static MainWindowViewModel onlyInstance;
        /// <summary>
        /// This constructor initializes various default properties across the class and then
        /// applies any updated values to them using <c>ApplyDefaultConfig</c>.
        /// </summary>
        private MainWindowViewModel()
        {
            onlyInstance = this;

            _configurationManager = new ConfigurationManager();
            _configuration = new Configuration(_configurationManager);

            UpdateLinkVisibility = Visibility.Hidden;
            UpdateProgressVisibility = Visibility.Hidden;
        }
        /// <summary>
        /// Fetches the only instance of the OpeningViewModel and creates it if it has not been initialized yet.
        /// </summary>
        /// <returns>An instance of OpeningViewModel</returns>
        public static MainWindowViewModel GetInstance
        { get => (onlyInstance == null) ? new MainWindowViewModel() : onlyInstance; }


        /********** Class Structure/Other Methods **********/

        //credit to Freesnow on discord
        protected bool SetProperty<T>(ref T property, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, newValue) || propertyName == null) return false;

            property = newValue;
            propertyChanged(propertyName);
            return true;
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