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

        public ICommand DisableSelected => new RelayCommand(() => _addonManager.Disable(CheckedAddons));

        public ICommand EnableSelected => new RelayCommand(() => _addonManager.Enable(CheckedAddons));

        public ICommand DeleteSelected => new RelayCommand(() => _addonManager.Delete(CheckedAddons));

        public ICommand InstallSelected => new RelayCommand(() => _addonManager.Install(CheckedAddons));

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
