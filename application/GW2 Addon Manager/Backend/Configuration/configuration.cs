using System.Windows;
using GW2_Addon_Manager.App.Configuration;
using GW2_Addon_Manager.App.Configuration.Model;
using GW2_Addon_Manager.Dependencies.FileSystem;

namespace GW2_Addon_Manager
{
    /// <summary>
    ///     The <c>configuration</c> class contains various functions dealing with application configuration.
    /// </summary>
    public class Configuration
    {
        static readonly string ApplicationRepoUrl = "https://api.github.com/repos/fmmmlee/GW2-Addon-Manager/releases/latest";

        private readonly IConfigurationManager _configurationManager;
        private readonly UpdateHelper _updateHelper;
        private readonly IFileSystemManager _fileSystemManager;

        public Configuration(IConfigurationManager configurationManager, UpdateHelper updateHelper,
            IFileSystemManager fileSystemManager)
        {
            _configurationManager = configurationManager;
            _updateHelper = updateHelper;
            _fileSystemManager = fileSystemManager;
        }

        /// <summary>
        ///     Checks if there is a new version of the application available.
        /// </summary>
        public bool CheckIfNewVersionIsAvailable(out string latestVersion)
        {
            var releaseInfo = _updateHelper.GitReleaseInfo(ApplicationRepoUrl);
            if (releaseInfo == null)
            {
                latestVersion = _configurationManager.ApplicationVersion;
                return false;
            }

            latestVersion = releaseInfo.tag_name;
            return latestVersion != _configurationManager.ApplicationVersion;
        }

        /// <summary>
        /// <c>SetCulture</c> both sets the culture for the current application session to <paramref name="culture"/> and records it in the configuration file.
        /// </summary>
        /// <param name="culture"></param>
        public void SetCulture(string culture)
        {
            Application.Current.Properties["culture"] = culture;
            _configurationManager.UserConfig.Culture = culture;
            _configurationManager.SaveConfiguration();
            RestartApplication();
        }

        /// <summary>
        /// Restarts the application.
        /// </summary>
        private void RestartApplication()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Attempts to read the game folder and determine whether the game is running on a 64 or 32-bit system.
        /// Based on that, sets the 'bin_folder' property in the configuration file.
        /// </summary>
        public void DetermineSystemType()
        {
            if (!_fileSystemManager.DirectoryExists(_configurationManager.UserConfig.GamePath)) return;

            if (_fileSystemManager.DirectoryExists(_configurationManager.UserConfig.GamePath + "\\bin64"))
            {
                _configurationManager.UserConfig.BinFolder = "bin64";
                _configurationManager.UserConfig.ExeName = "Gw2-64.exe";
            }
            else if (_fileSystemManager.DirectoryExists(_configurationManager.UserConfig.GamePath + "\\bin"))
            {
                _configurationManager.UserConfig.BinFolder = "bin";
                _configurationManager.UserConfig.ExeName = "Gw2.exe";
            }

            _configurationManager.SaveConfiguration();
        }

        /// <summary>
        ///     Deletes all addons, addon loader, and configuration data related to addons.
        /// </summary>
        public void DeleteAllAddons()
        {
            //set installed, disabled, default, and version collections to the default installation setting
            _configurationManager.UserConfig.AddonsList = new AddonsList();

            //clear loader_version
            _configurationManager.UserConfig.LoaderVersion = null;

            //delete disabled plugins folder: ${install dir}/disabled plugins
            if (_fileSystemManager.DirectoryExists("Disabled Plugins"))
                _fileSystemManager.DirectoryDelete("Disabled Plugins", true);
            //delete addons: {game folder}/addons
            if (_fileSystemManager.DirectoryExists(
                _fileSystemManager.PathCombine(_configurationManager.UserConfig.GamePath, "addons")))
                _fileSystemManager.DirectoryDelete(
                    _fileSystemManager.PathCombine(_configurationManager.UserConfig.GamePath, "addons"), true);
            //delete addon loader: {game folder}/{bin/64}/d3d9.dll
            _fileSystemManager.FileDelete(_fileSystemManager.PathCombine(
                _fileSystemManager.PathCombine(_configurationManager.UserConfig.GamePath,
                    _configurationManager.UserConfig.BinFolder), "d3d9.dll"));

            //write cleaned config file
            _configurationManager.SaveConfiguration();
        }
    }
}