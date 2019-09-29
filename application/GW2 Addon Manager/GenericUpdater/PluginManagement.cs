using System.IO;
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
        /// <param name="viewModel">The DataContext for the application UI.</param>
        public static void DeleteSelected(OpeningViewModel viewModel)
        {
            string deletemsg = "This will delete any add-ons that are selected and all data associated with them! Are you sure you wish to continue?";
            if (MessageBox.Show(deletemsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (AddonInfo addon in viewModel.AddonList.Where(add => add.IsSelected == true))
                    GenericUpdater.delete(addon);
            }

            configuration.DisplayAddonStatus(viewModel);
        }

        

        /// <summary>
        /// Disables the currently selected addons.
        /// <seealso cref="OpeningViewModel.DisableSelected"/>
        /// </summary>
        /// <param name="viewModel">The DataContext for the application UI.</param>
        public static void DisableSelected(OpeningViewModel viewModel)
        {
            string disablemsg = "This will disable the selected add-ons until you choose to re-enable them. Do you wish to continue?";
            if (MessageBox.Show(disablemsg, "Disable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                foreach (int index in viewModel.SelectedAddons)
                    GenericUpdater.disable(viewModel.AddonList[index]);
            }

            configuration.DisplayAddonStatus(viewModel);
        }

        /// <summary>
        /// Enables the currently selected addons.
        /// <seealso cref="OpeningViewModel.EnableSelected"/>
        /// </summary>
        /// <param name="viewModel">The DataContext for the application UI.</param>
        public static void EnableSelected(OpeningViewModel viewModel)
        {
            string enablemsg = "This will enable any of the selected add-ons that are disabled. Do you wish to continue?";
            if (MessageBox.Show(enablemsg, "Enable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                foreach (int index in viewModel.SelectedAddons)
                    GenericUpdater.enable(viewModel.AddonList[index]);
            }

            configuration.DisplayAddonStatus(viewModel);
        }
    }
}
