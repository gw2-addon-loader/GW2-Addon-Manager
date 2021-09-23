using System;
using System.IO;
using System.IO.Abstractions;
using System.Windows;
using GW2_Addon_Manager.App.Configuration;
using GW2_Addon_Manager.App.Configuration.Model;
using Localization;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// The <c>configuration</c> class contains various functions dealing with application configuration. 
    /// </summary>
    public class Configuration
    {
        static readonly string applicationRepoUrl = "https://api.github.com/repos/gw2-addon-loader/GW2-Addon-Manager/releases/latest";

        private readonly IConfigurationManager _configurationManager;

        public Configuration(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// <c>SetGamePath</c> both sets the game path for the current application session to <paramref name="path"/> and records it in the configuration file.
        /// </summary>
        /// <param name="path">The game path.</param>
        public void SetGamePath(string path)
        {
            try
            {
                Application.Current.Properties["game_path"] = path.Replace("\\", "\\\\");
            }
            catch (Exception)
            { }

            _configurationManager.UserConfig.GamePath = path;
            _configurationManager.SaveConfiguration();
            DetermineSystemType();
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
        /// Checks if there is a new version of the application available.
        /// </summary>
        public void CheckSelfUpdates()
        {
            var release_info = UpdateHelpers.GitReleaseInfo(applicationRepoUrl);
            var latestVersion = release_info.tag_name;

            if (latestVersion == _configurationManager.ApplicationVersion) return;

            MainWindowViewModel.GetInstance.UpdateAvailable = $"{latestVersion} {StaticText.Available.ToLower()}!";
            MainWindowViewModel.GetInstance.UpdateLinkVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Attempts to read the game folder and determine whether the game is running on a 64 or 32-bit system.
        /// Based on that, sets the 'bin_folder' property in the configuration file.
        /// </summary>
        public void DetermineSystemType()
        {
            if (!Directory.Exists(_configurationManager.UserConfig.GamePath)) return;
            
            if (Directory.Exists(_configurationManager.UserConfig.GamePath + "\\bin64"))
            {
                _configurationManager.UserConfig.BinFolder= "bin64";
                _configurationManager.UserConfig.ExeName = "Gw2-64.exe";
            }
            else if (Directory.Exists(_configurationManager.UserConfig.GamePath + "\\bin"))
            {
                _configurationManager.UserConfig.BinFolder = "bin";
                _configurationManager.UserConfig.ExeName = "Gw2.exe";
            }
            _configurationManager.SaveConfiguration();
        }

        /// <summary>
        /// Deletes all addons, addon loader, and configuration data related to addons.
        /// </summary>
        public void DeleteAllAddons()
        {
            //set installed, disabled, default, and version collections to the default installation setting
            _configurationManager.UserConfig.AddonsList = new AddonsList();

            //clear loader_version
            _configurationManager.UserConfig.LoaderVersion = null;

            //delete disabled plugins folder: ${install dir}/disabled plugins
            if(Directory.Exists("Disabled Plugins"))
                Directory.Delete("Disabled Plugins", true);
            //delete addons: {game folder}/addons
            if(Directory.Exists(Path.Combine(_configurationManager.UserConfig.GamePath, "addons")))
                Directory.Delete(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), true);
            //delete addon loader: {game folder}/{bin/64}/d3d9.dll
            File.Delete(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, _configurationManager.UserConfig.BinFolder), "d3d9.dll"));

            //write cleaned config file
            _configurationManager.SaveConfiguration();
        }
    }
}
