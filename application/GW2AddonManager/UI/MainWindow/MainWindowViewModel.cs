using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace GW2AddonManager
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly ISelfManager _selfManager;
        private readonly ICoreManager _coreManager;
        private Visibility _updateLinkVisibility;
        private Visibility _updateProgressVisibility;
        private int _updateDownloadProgress;
        private string _updateAvailable;

        public Visibility UpdateLinkVisibility { get => _updateLinkVisibility; set => SetProperty(ref _updateLinkVisibility, value); }
        public Visibility UpdateProgressVisibility { get => _updateProgressVisibility; set => SetProperty(ref _updateProgressVisibility, value); }
        public int UpdateDownloadProgress { get => _updateDownloadProgress; set => SetProperty(ref _updateDownloadProgress, value); }
        public string UpdateAvailable { get => _updateAvailable; set => SetProperty(ref _updateAvailable, value); }

        public ICommand ChangeLanguage
        {
            get => new RelayCommand<string>(param => _coreManager.UpdateCulture(param));
        }

        public ICommand DownloadSelfUpdate
        {
            get => new AsyncRelayCommand<object>(async param =>
            {
                UpdateProgressVisibility = Visibility.Visible;
                UpdateLinkVisibility = Visibility.Hidden;
                await _selfManager.Update();
            });
        }

        public MainWindowViewModel(ISelfManager selfManager, ICoreManager coreManager)
        {
            _selfManager = selfManager;
            _coreManager = coreManager;

            UpdateLinkVisibility = Visibility.Hidden;
            UpdateProgressVisibility = Visibility.Hidden;

            _selfManager.MessageChanged += (obj, msg) => UpdateAvailable = msg;
            _selfManager.ProgressChanged += (obj, pct) => UpdateDownloadProgress = pct;
        }
    }
}