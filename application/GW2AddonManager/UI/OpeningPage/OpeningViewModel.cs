using GW2AddonManager.Localization;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;

namespace GW2AddonManager
{
    public class OpeningViewModel : DependentObservableObject
    {
        public class AddonInfoCheck : ObservableObject
        {
            private bool _checked;

            public AddonInfo Addon { get; init; }
            public bool AddonChecked
            {
                get => _checked;
                set => SetProperty(ref _checked, value);
            }

            public AddonInfoCheck(bool c, AddonInfo i)
            {
                _checked = c;
                Addon = i;
            }
        }

        private AddonInfoCheck _selectedAddon;
        private readonly IAddonManager _addonManager;
        private readonly IAddonRepository _addonRepository;
        private readonly ICoreManager _coreManager;
        private readonly IConfigurationProvider _configurationProvider;
              
        public ObservableCollection<AddonInfoCheck> Addons { get; } = new ObservableCollection<AddonInfoCheck>();
        public IEnumerable<AddonInfo> CheckedAddons { get => Addons.Where(x => x.AddonChecked).Select(x => x.Addon); }

        [DependsOn("SelectedAddon")]
        public string DescriptionText => _selectedAddon?.Addon.Description ?? StaticText.SelectAnAddonToSeeMoreInformationAboutIt;
        [DependsOn("SelectedAddon")]
        public string DeveloperText => _selectedAddon?.Addon.Developer ?? "";
        [DependsOn("SelectedAddon")]
        public string AddonWebsiteLink => _selectedAddon?.Addon.Website ?? "";
        [DependsOn("SelectedAddon")]
        public Visibility DeveloperVisibility => _selectedAddon?.Addon.Developer is not null ? Visibility.Visible : Visibility.Hidden;

        public AddonInfoCheck SelectedAddon {
            get => _selectedAddon;
            set => SetProperty(ref _selectedAddon, value);
        }

        [DependsOn("Addons")]
        public bool AnyAddonChecked => Addons.Any(x => x.AddonChecked);

        [DependsOn("Addons")]
        public bool NotAllAddonsChecked => Addons.Any(x => !x.AddonChecked);

        public ICommand DisableSelected => new RelayCommand(async () => { await _addonManager.Disable(CheckedAddons); OnPropertyChanged("Addons"); });

        public ICommand EnableSelected => new RelayCommand(async () => { await _addonManager.Enable(CheckedAddons); OnPropertyChanged("Addons"); });

        public ICommand DeleteSelected => new RelayCommand(async () => { await _addonManager.Delete(CheckedAddons); OnPropertyChanged("Addons"); });

        public ICommand InstallSelected => new RelayCommand(async () => { await _addonManager.Install(CheckedAddons); OnPropertyChanged("Addons"); });

        public ICommand CleanInstall => new RelayCommand(() => _coreManager.Uninstall());

        public ICommand CheckAllAddons => new RelayCommand(() =>
        {
            foreach (var a in Addons)
                a.AddonChecked = true;
            OnPropertyChanged("Addons");
        });

        public ICommand UncheckAllAddons => new RelayCommand(() =>
        {
            foreach (var a in Addons)
                a.AddonChecked = false;
            OnPropertyChanged("Addons");
        });

        public ICommand CheckAddon => new RelayCommand(() => OnPropertyChanged("Addons"));

        private string _log = "";
        public string Log
        {
            get => _log;
            set => SetProperty(ref _log, value);
        }

        [DependsOn("Log")]
        public Visibility LogVisibility => Log.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        [DependsOn("Addons")]
        public bool CanInstallAny => _configurationProvider.UserConfig.GamePath != null && CheckedAddons.Any(x => {
            bool exists = _configurationProvider.UserConfig.AddonsState.TryGetValue(x.Nickname, out AddonState state);
            return exists && !state.Installed || !exists;
        });
        [DependsOn("Addons")]
        public bool CanDeleteAny => _configurationProvider.UserConfig.GamePath != null && CheckedAddons.Any(x => {
            return _configurationProvider.UserConfig.AddonsState.TryGetValue(x.Nickname, out AddonState state) && state.Installed;
        });
        [DependsOn("Addons")]
        public bool CanEnableAny => _configurationProvider.UserConfig.GamePath != null && CheckedAddons.Any(x => {
            return _configurationProvider.UserConfig.AddonsState.TryGetValue(x.Nickname, out AddonState state) && state.Disabled;
        });
        [DependsOn("Addons")]
        public bool CanDisableAny => _configurationProvider.UserConfig.GamePath != null && CheckedAddons.Any(x => {
            return _configurationProvider.UserConfig.AddonsState.TryGetValue(x.Nickname, out AddonState state) && !state.Disabled;
        });

        public OpeningViewModel(IAddonManager addonManager, IAddonRepository addonRepository, ICoreManager coreManager, IConfigurationProvider configurationProvider)
        {
            _addonManager = addonManager;
            _addonRepository = addonRepository;
            _coreManager = coreManager;
            _configurationProvider = configurationProvider;
            Addons = new ObservableCollection<AddonInfoCheck>(_addonRepository.Addons.OrderBy(x => x.Value.AddonName).Select(
                x => {
                        bool check = false;
                        if(_configurationProvider.UserConfig.AddonsState.TryGetValue(x.Key, out AddonState state))
                            check = state.Installed && state.VersionId != x.Value.VersionId;

                        return new AddonInfoCheck(check, x.Value);
                    }));

            _coreManager.Log += _coreManager_Log;
        }

        private void _coreManager_Log(object sender, LogEventArgs eventArgs)
        {
            Log += eventArgs.Message + Environment.NewLine;
        }
    }
}
