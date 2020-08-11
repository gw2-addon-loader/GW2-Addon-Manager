using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GW2_Addon_Manager
{
    class PluginManagement
    {
        /// <summary>
        /// Sets version fields of all installed and enabled addons to a dummy value so they are redownloaded, then starts update process.
        /// Intended for use if a user borks their install (probably by manually deleting something in the /addons/ folder).
        /// </summary>
        public static bool ForceRedownload()
        {
            string redownloadmsg = "This will forcibly redownload all installed addons regardless of their version. Do you wish to continue?";
            if (MessageBox.Show(redownloadmsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                UserConfig config = Configuration.getConfigAsYAML();
                config.version = config.version.ToDictionary(entry => entry.Key, entry => "dummy value");
                Configuration.setConfigAsYAML(config);
                return true;
            }
            return false; 
        }

        /// <summary>
        /// Deletes all addons and resets config to default state.
        /// <seealso cref="OpeningViewModel.CleanInstall"/>
        /// <seealso cref="Configuration.DeleteAllAddons"/>
        /// </summary>
        public static void DeleteAll()
        {
            string deletemsg = "This will delete ALL add-ons from Guild Wars 2 and all data associated with them! Are you sure you wish to continue?";
            string secondPrecautionaryMsg = "Are you absolutely sure you want to delete all addons? This action cannot be undone.";

            //precautionary "are you SURE" messages x2
            if (MessageBox.Show(deletemsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                if (MessageBox.Show(secondPrecautionaryMsg, "Absolutely Sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Configuration.DeleteAllAddons();
                    //post-delete info message
                    MessageBox.Show("All addons have been removed.", "Reverted to Clean Install", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                    
            Configuration.DisplayAddonStatus();
        }

        /// <summary>
        /// Deletes the currently selected addons.
        /// <seealso cref="OpeningViewModel.DeleteSelected"/>
        /// </summary>
        public static void DeleteSelected()
        {
            string deletemsg = "This will delete any add-ons that are selected and all data associated with them! Are you sure you wish to continue?";
            if (MessageBox.Show(deletemsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (AddonInfoFromYaml addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true))
                    GenericUpdater.delete(addon);
            }
            Configuration.DisplayAddonStatus();
        }

        /// <summary>
        /// Disables the currently selected addons.
        /// <seealso cref="OpeningViewModel.DisableSelected"/>
        /// </summary>
        public static void DisableSelected()
        {
            string disablemsg = "This will disable the selected add-ons until you choose to re-enable them. Do you wish to continue?";
            if (MessageBox.Show(disablemsg, "Disable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                foreach (AddonInfoFromYaml addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true))
                    GenericUpdater.Disable(addon);
            }
            Configuration.DisplayAddonStatus();
        }

        /// <summary>
        /// Enables the currently selected addons.
        /// <seealso cref="OpeningViewModel.EnableSelected"/>
        /// </summary>
        public static void EnableSelected()
        {
            string enablemsg = "This will enable any of the selected add-ons that are disabled. Do you wish to continue?";
            if (MessageBox.Show(enablemsg, "Enable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                foreach (AddonInfoFromYaml addon in OpeningViewModel.GetInstance.AddonList.Where(add => add.IsSelected == true))
                    GenericUpdater.enable(addon);
            }
            Configuration.DisplayAddonStatus();
        }
    }
}
