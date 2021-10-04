using GW2AddonManager.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GW2AddonManager
{
    public interface ICoreManager
    {
        void Uninstall();
        void UpdateCulture(string constant);
    }

    public class CoreManager : ICoreManager
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFileSystem _fileSystem;
        private readonly ILoaderManager _loaderManager;
        private readonly IAddonManager _addonManager;

        public CoreManager(IConfigurationProvider configurationProvider, IFileSystem fileSystem, ILoaderManager loaderManager, IAddonManager addonManager)
        {
            _configurationProvider = configurationProvider;
            _fileSystem = fileSystem;
            _loaderManager = loaderManager;
            _addonManager = addonManager;
        }

        public void Uninstall()
        {
            if(_configurationProvider.UserConfig.GamePath is null || !_fileSystem.Directory.Exists(_configurationProvider.UserConfig.GamePath))
            {
                _ = MessageBox.Show(StaticText.NoGamePath, StaticText.ResetToCleanInstall, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show(StaticText.ResetToCleanInstallWarning, StaticText.ResetToCleanInstall, MessageBoxButton.YesNo, MessageBoxImage.Hand, MessageBoxResult.No) != MessageBoxResult.Yes)
                return;

            _loaderManager.Uninstall();

            if (_fileSystem.Directory.Exists(_addonManager.AddonsFolder))
                _fileSystem.Directory.Delete(_addonManager.AddonsFolder, true);

            _configurationProvider.UserConfig = _configurationProvider.UserConfig with
            {
                AddonsState = new Dictionary<string, AddonState>()
            };

            if(_fileSystem.File.Exists(_configurationProvider.ConfigFileName))
                _fileSystem.File.Delete(_configurationProvider.ConfigFileName);

            _ = MessageBox.Show(StaticText.ResetToCleanInstallDone, StaticText.ResetToCleanInstall, MessageBoxButton.OK);

            Application.Current.Shutdown();
        }

        public void UpdateCulture(string constant)
        {
            bool needsChange = constant != _configurationProvider.UserConfig.Culture;
            if (needsChange)
            {
                _configurationProvider.UserConfig = _configurationProvider.UserConfig with
                {
                    Culture = constant
                };
            }

            CultureInfo culture = new CultureInfo(_configurationProvider.UserConfig.Culture);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (needsChange)
                App.Current.ReopenMainWindow();
        }
    }
}
