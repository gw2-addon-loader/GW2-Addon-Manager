using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace GW2AddonManager
{
    public class OpeningViewModel : ObservableObject
    {
        private string _descriptionText;
        private string _developerText;
        private string _addonWebsiteLink;
        private Visibility _developerVisibility;
        private ObservableCollection<int> _selectedAddons = new ObservableCollection<int>();
        private ObservableCollection<AddonInfo> _addons = new ObservableCollection<AddonInfo>();

        public ObservableCollection<int> SelectedAddons { get => _selectedAddons; }

        public ObservableCollection<AddonInfo> Addons { get => _addons; }

        public string DescriptionText { get => _descriptionText; set => SetProperty(ref _descriptionText, value); }
        public string DeveloperText { get => _developerText; set => SetProperty(ref _developerText, value); }
        public string AddonWebsiteLink { get => _addonWebsiteLink; set => SetProperty(ref _addonWebsiteLink, value); }

        public Visibility DeveloperVisibility { get => _developerVisibility; set => SetProperty(ref _developerVisibility, value); }


        public bool AnyAddonSelected { get; set; } = false;
        public bool NoAddonSelected => !AnyAddonSelected;


        public ICommand DisableSelected
        {
            get => new RelayCommand<object>(param => _pluginManagement.DisableSelected(), true);
        }

        public ICommand EnableSelected
        {
            get => new RelayCommand<object>(param => _pluginManagement.EnableSelected(), true);
        }

        public ICommand DeleteSelected
        {
            get => new RelayCommand<object>(param => _pluginManagement.DeleteSelected(), true);
        }

        public ICommand CleanInstall
        {
            get => new RelayCommand<object>(param => _pluginManagement.DeleteAll(), true);
        }

        public string GamePath
        {
            get => _configurationManager.UserConfig.GamePath;
            set
            {
                if (value == _configurationManager.UserConfig.GamePath)
                    return;
                propertyChanged("GamePath");
                _configurationManager.UserConfig.GamePath = value;
                _configurationManager.Save();
            }
        }

        public OpeningViewModel(IConfigurationProvider configurationManager, AddonManager pluginManagement, AddonRepository approvedList)
        {
            _configurationManager = configurationManager;
            _pluginManagement = pluginManagement;

            AddonList = approvedList.GenerateAddonList();

            DescriptionText = StaticText.SelectAnAddonToSeeMoreInformationAboutIt;
            DeveloperVisibility = Visibility.Hidden;
        }
    }
}
