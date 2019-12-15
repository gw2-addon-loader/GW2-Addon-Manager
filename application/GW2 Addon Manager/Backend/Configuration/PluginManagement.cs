using System.Linq;
using System.Windows;

namespace GW2_Addon_Manager
{
    class PluginManagement
    {

        /// <summary>
        /// Deletes the currently selected addons.
        /// <seealso cref="OpeningViewModel.DeleteSelected"/>
        /// </summary>
        /// <param name="OpeningViewModel.GetInstance">The DataContext for the application UI.</param>
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
        /// <param name="OpeningViewModel.GetInstance">The DataContext for the application UI.</param>
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
        /// <param name="OpeningViewModel.GetInstance">The DataContext for the application UI.</param>
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
