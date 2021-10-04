using GW2AddonManager.Localization;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;
using System.Linq;
using System.Windows.Controls;

namespace GW2AddonManager
{
    public class OpeningViewModel : DependentObservableObject
    {
        private AddonInfo _selectedAddon = null;
        private ObservableCollection<AddonInfo> _checkedAddons = new ObservableCollection<AddonInfo>();
        private ObservableCollection<AddonInfo> _addons = new ObservableCollection<AddonInfo>();
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAddonManager _addonManager;
        private readonly IAddonRepository _addonRepository;
        private readonly ICoreManager _coreManager;

        public ObservableCollection<AddonInfo> CheckedAddons { get => _checkedAddons; }

        public ObservableCollection<AddonInfo> Addons { get => _addons; }

        [DependsOn("SelectedAddon")]
        public string DescriptionText => _selectedAddon?.Description ?? StaticText.SelectAnAddonToSeeMoreInformationAboutIt;
        [DependsOn("SelectedAddon")]
        public string DeveloperText => _selectedAddon?.Developer ?? "";
        [DependsOn("SelectedAddon")]
        public string AddonWebsiteLink => _selectedAddon?.Website ?? "";
        [DependsOn("SelectedAddon")]
        public Visibility DeveloperVisibility => _selectedAddon?.Developer is not null ? Visibility.Visible : Visibility.Hidden;

        public AddonInfo SelectedAddon { get => _selectedAddon; set => SetProperty(ref _selectedAddon, value); }


        [DependsOn("CheckedAddons")]
        public bool AnyAddonChecked => CheckedAddons.Count > 0;

        public ICommand SelectDirectory => new RelayCommand(() =>
                                           {
                                               var pathSelectionDialog = new VistaFolderBrowserDialog();
                                               if (pathSelectionDialog.ShowDialog() ?? false)
                                               {
                                                   _configurationProvider.UserConfig = _configurationProvider.UserConfig with
                                                   {
                                                       GamePath = pathSelectionDialog.SelectedPath
                                                   };
                                                   OnPropertyChanged("GamePath");
                                               }
                                           });

        public ICommand DisableSelected => new RelayCommand<AddonInfo[]>(addons => _addonManager.Disable(addons));

        public ICommand EnableSelected => new RelayCommand<AddonInfo[]>(addons => _addonManager.Enable(addons));

        public ICommand DeleteSelected => new RelayCommand<AddonInfo[]>(addons => _addonManager.Delete(addons));

        public ICommand CleanInstall => new RelayCommand(() => _coreManager.Uninstall());

        public ICommand CheckAddon => new RelayCommand<object[]>(x => {
            bool isChecked = (bool)x[1];
            AddonInfo addonInfo = (AddonInfo)x[0];

            if(isChecked)
                CheckedAddons.Add(addonInfo);
            else
                _ = CheckedAddons.Remove(addonInfo);
        });

        public string GamePath => _configurationProvider.UserConfig.GamePath;

        public OpeningViewModel(IConfigurationProvider configurationProvider, IAddonManager addonManager, IAddonRepository addonRepository, ICoreManager coreManager)
        {
            _configurationProvider = configurationProvider;
            _addonManager = addonManager;
            _addonRepository = addonRepository;
            _coreManager = coreManager;
            _addons = new ObservableCollection<AddonInfo>(_addonRepository.Addons.Values.OrderBy(x => x.AddonName));

            CheckedAddons.CollectionChanged += (_, _) => OnPropertyChanged("CheckedAddons");
        }
    }
}
