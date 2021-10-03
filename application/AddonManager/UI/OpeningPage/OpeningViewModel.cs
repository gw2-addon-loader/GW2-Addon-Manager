using GW2AddonManager.Localization;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

namespace GW2AddonManager
{
    public class OpeningViewModel : ObservableObject
    {
        private AddonInfo _selectedAddon = null;
        private ObservableCollection<int> _selectedAddons = new ObservableCollection<int>();
        private ObservableCollection<AddonInfo> _addons = new ObservableCollection<AddonInfo>();
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAddonManager _addonManager;
        private readonly IAddonRepository _addonRepository;
        private readonly ICoreManager _coreManager;

        public ObservableCollection<int> SelectedAddons { get => _selectedAddons; }

        public ObservableCollection<AddonInfo> Addons { get => _addons; }

        public string DescriptionText => _selectedAddon?.Description ?? StaticText.SelectAnAddonToSeeMoreInformationAboutIt;
        public string DeveloperText => _selectedAddon?.Developer ?? "";
        public string AddonWebsiteLink => _selectedAddon?.Website ?? "";

        public AddonInfo SelectedAddon { get => _selectedAddon; set => SetProperty(ref _selectedAddon, value); }

        public Visibility DeveloperVisibility => _selectedAddon?.Developer is not null ? Visibility.Visible : Visibility.Hidden;


        public bool AnyAddonSelected => _selectedAddon is not null;
        public bool NoAddonSelected => !AnyAddonSelected;

        public ICommand SelectDirectory => new RelayCommand(() =>
                                           {
                                               var pathSelectionDialog = new VistaFolderBrowserDialog();
                                               if (pathSelectionDialog.ShowDialog() ?? false)
                                                   _configurationProvider.UserConfig = _configurationProvider.UserConfig with
                                                   {
                                                       GamePath = pathSelectionDialog.SelectedPath
                                                   };
                                           });

        public ICommand DisableSelected => new RelayCommand<AddonInfo[]>(addons => _addonManager.Disable(addons));

        public ICommand EnableSelected => new RelayCommand<AddonInfo[]>(addons => _addonManager.Enable(addons));

        public ICommand DeleteSelected => new RelayCommand<AddonInfo[]>(addons => _addonManager.Delete(addons));

        public ICommand CleanInstall => new RelayCommand(() => _coreManager.Uninstall());

        public string GamePath => _configurationProvider.UserConfig.GamePath;

        public OpeningViewModel(IConfigurationProvider configurationProvider, IAddonManager addonManager, IAddonRepository addonRepository, ICoreManager coreManager)
        {
            _configurationProvider = configurationProvider;
            _addonManager = addonManager;
            _addonRepository = addonRepository;
            _coreManager = coreManager;
            _addons = new ObservableCollection<AddonInfo>(_addonRepository.Addons.Values);
        }
    }
}
