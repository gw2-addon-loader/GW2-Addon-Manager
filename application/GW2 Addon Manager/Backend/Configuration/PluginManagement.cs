using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GW2_Addon_Manager.App.Configuration;

namespace GW2_Addon_Manager
{
    public class PluginManagement
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly AddonYamlReader _yaml;
        private readonly GenericUpdaterFactory _updaterFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationManager"></param>
        /// <param name="yaml"></param>
        /// <param name="updaterFactory"></param>
        public PluginManagement(IConfigurationManager configurationManager, AddonYamlReader yaml, GenericUpdaterFactory updaterFactory)
        {
            _configurationManager = configurationManager;
            _yaml = yaml;
            _updaterFactory = updaterFactory;
        }

        /// <summary>
        /// Sets version fields of all installed and enabled addons to a dummy value so they are redownloaded, then starts update process.
        /// Intended for use if a user borks their install (probably by manually deleting something in the /addons/ folder).
        /// </summary>
        public bool ForceRedownload()
        {
            string redownloadmsg = "This will forcibly redownload all installed addons regardless of their version. Do you wish to continue?";
            if (MessageBox.Show(redownloadmsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _configurationManager.UserConfig.AddonsList.Where(a => a.Installed).ToList().ForEach(a => a.Version = "dummy value");
                _configurationManager.Save();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes all addons and resets config to default state.
        /// <seealso cref="OpeningViewModel.CleanInstall"/>
        /// <seealso cref="Configuration.DeleteAllAddons"/>
        /// </summary>
        public void DeleteAll()
        {
            string deletemsg = "This will delete ALL add-ons from Guild Wars 2 and all data associated with them! Are you sure you wish to continue?";
            string secondPrecautionaryMsg = "Are you absolutely sure you want to delete all addons? This action cannot be undone.";

            //precautionary "are you SURE" messages x2
            if (MessageBox.Show(deletemsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                if (MessageBox.Show(secondPrecautionaryMsg, "Absolutely Sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    new Configuration(_configurationManager).DeleteAllAddons();
                    //post-delete info message
                    MessageBox.Show("All addons have been removed.", "Reverted to Clean Install", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            DisplayAddonStatus();
        }

        /// <summary>
        /// Deletes the currently selected addons.
        /// <seealso cref="OpeningViewModel.DeleteSelected"/>
        /// </summary>
        public void DeleteSelected()
        {
            string deletemsg = "This will delete any add-ons that are selected and all data associated with them! Are you sure you wish to continue?";
            if (MessageBox.Show(deletemsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (AddonInfo addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true))
                    _updaterFactory.Create(addon).Delete();
            }
            DisplayAddonStatus();
        }

        /// <summary>
        /// Disables the currently selected addons.
        /// <seealso cref="OpeningViewModel.DisableSelected"/>
        /// </summary>
        public void DisableSelected()
        {
            string disablemsg = "This will disable the selected add-ons until you choose to re-enable them. Do you wish to continue?";
            if (MessageBox.Show(disablemsg, "Disable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                foreach (AddonInfo addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true))
                    _updaterFactory.Create(addon).Disable();
            }
            DisplayAddonStatus();
        }

        /// <summary>
        /// Enables the currently selected addons.
        /// <seealso cref="OpeningViewModel.EnableSelected"/>
        /// </summary>
        public void EnableSelected()
        {
            string enablemsg = "This will enable any of the selected add-ons that are disabled. Do you wish to continue?";
            if (MessageBox.Show(enablemsg, "Enable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                foreach (AddonInfo addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true))
                    _updaterFactory.Create(addon).Enable();
            }
            DisplayAddonStatus();
        }

        /// <summary>
        /// Displays the latest status of the plugins on the opening screen (disabled, enabled, version, installed).
        /// </summary>
        public void DisplayAddonStatus()
        {
            foreach (var addon in OpeningViewModel.GetInstance.AddonList)
            {
                var addonConfig =
                    _configurationManager.UserConfig.AddonsList[addon.FolderName];
                if (addonConfig == null) continue;

                addon.AddonName = _yaml.GetAddonInfo(addon.FolderName).AddonName;
                if (addonConfig.Installed)
                {
                    if (addon.FolderName == "arcdps" || addon.FolderName == "buildPad" || addonConfig.Version.Length > 10)
                        addon.AddonName += " (installed)";
                    else
                        addon.AddonName += " (" + addonConfig.Version + " installed)";
                }

                if (addonConfig.Disabled)
                    addon.AddonName += " (disabled)";
            }
        }
    }
}
