using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Manager
{
    class PluginManagement
    {
        /// <summary>
        /// Deletes the currently selected addons.
        /// <seealso cref="OpeningViewModel.DeleteSelected"/>
        /// <seealso cref="arcdps.delete(string)"/>
        /// <seealso cref="gw2radial.delete(string)"/>
        /// <seealso cref="d912pxy.delete(string)"/>
        /// </summary>
        /// <param name="viewModel">The DataContext for the application UI.</param>
        public static void DeleteSelected(OpeningViewModel viewModel)
        {
            string deletemsg = "This will delete any add-ons that are selected and all data associated with them! Are you sure you wish to continue?";
            if (MessageBox.Show(deletemsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                string gamePath = configuration.getConfig().game_path;
                if (viewModel.ArcDPS_CheckBox)
                    arcdps.delete(gamePath);
                if (viewModel.GW2Radial_CheckBox)
                    gw2radial.delete(gamePath);
                if (viewModel.d912pxy_CheckBox)
                    d912pxy.delete(gamePath);
            }
        }

        /// <summary>
        /// Disables the currently selected addons.
        /// <seealso cref="OpeningViewModel.DisableSelected"/>
        /// <seealso cref="arcdps.disable(string)"/>
        /// <seealso cref="gw2radial.disable(string)"/>
        /// <seealso cref="d912pxy.disable(string)"/>
        /// </summary>
        /// <param name="viewModel">The DataContext for the application UI.</param>
        public static void DisableSelected(OpeningViewModel viewModel)
        {
            string disablemsg = "This will disable the selected add-ons until you choose to re-enable them. Do you wish to continue?";
            if (MessageBox.Show(disablemsg, "Disable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string gamePath = configuration.getConfig().game_path;
                if (viewModel.ArcDPS_CheckBox)
                    arcdps.disable(gamePath);
                if (viewModel.GW2Radial_CheckBox)
                    gw2radial.disable(gamePath);
                if (viewModel.d912pxy_CheckBox)
                    d912pxy.disable(gamePath);
            }

            configuration.DisplayAddonStatus(viewModel);
        }

        /// <summary>
        /// Enables the currently selected addons.
        /// <seealso cref="OpeningViewModel.EnableSelected"/>
        /// <seealso cref="arcdps.enable(string)"/>
        /// <seealso cref="gw2radial.enable(string)"/>
        /// <seealso cref="d912pxy.enable(string)"/>
        /// </summary>
        /// <param name="viewModel">The DataContext for the application UI.</param>
        public static void EnableSelected(OpeningViewModel viewModel)
        {
            string enablemsg = "This will enable any of the selected add-ons that are disabled. Do you wish to continue?";
            if (MessageBox.Show(enablemsg, "Enable", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string gamePath = configuration.getConfig().game_path;
                if (viewModel.ArcDPS_CheckBox)
                    arcdps.enable(gamePath);
                if (viewModel.GW2Radial_CheckBox)
                    gw2radial.enable(gamePath);
                if (viewModel.d912pxy_CheckBox)
                    d912pxy.enable(gamePath);

                configuration.DisplayAddonStatus(viewModel);
            }

            configuration.DisplayAddonStatus(viewModel);
        }
    }
}
