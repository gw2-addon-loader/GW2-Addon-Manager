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
    public record LogEventArgs(string Message);
    public delegate void LogEventHandler(object sender, LogEventArgs eventArgs);

    public interface ICoreManager
    {
        void Uninstall();
        void UpdateCulture(string constant);

        event LogEventHandler Log;
        event EventHandler Uninstalling;
        void AddLog(string msg);
    }

    public class CoreManager : ICoreManager
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFileSystem _fileSystem;

        public event LogEventHandler Log;
        public event EventHandler Uninstalling;

        public CoreManager(IConfigurationProvider configurationProvider, IFileSystem fileSystem)
        {
            _configurationProvider = configurationProvider;
            _fileSystem = fileSystem;
        }

        public void Uninstall()
        {
            if(_configurationProvider.UserConfig.GamePath is null || !_fileSystem.Directory.Exists(_configurationProvider.UserConfig.GamePath))
            {
                _ = Popup.Show(StaticText.NoGamePath, StaticText.ResetToCleanInstall, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Popup.Show(StaticText.ResetToCleanInstallWarning, StaticText.ResetToCleanInstall, MessageBoxButton.YesNo, MessageBoxImage.Hand) != MessageBoxResult.Yes)
                return;

            Uninstalling?.Invoke(this, new EventArgs());

            _configurationProvider.UserConfig = _configurationProvider.UserConfig with
            {
                AddonsState = new Dictionary<string, AddonState>()
            };

            if(_fileSystem.File.Exists(_configurationProvider.ConfigFileName))
                _fileSystem.File.Delete(_configurationProvider.ConfigFileName);

            _ = Popup.Show(StaticText.ResetToCleanInstallDone, StaticText.ResetToCleanInstall, MessageBoxButton.OK);

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

        public void AddLog(string msg)
        {
            Log?.Invoke(this, new LogEventArgs(msg));
        }
    }
}
