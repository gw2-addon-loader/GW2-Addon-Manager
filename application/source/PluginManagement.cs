using System.IO;
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

                RenamePlugins();
            }
            configuration.DisplayAddonStatus(viewModel);
        }

        /// <summary>
        /// Names and moves plugins based on the current installation configuration.
        /// </summary>
        public static void RenamePlugins()
        {
            string d912pxy_name = null;
            string arcdps_name = null;
            string gw2radial_name = null;
            dynamic config = configuration.getConfig();

            string bin64 = config.game_path + "\\bin64\\";

            if (config.installed.arcdps != null && !(bool)config.disabled.arcdps)
            {
                arcdps_name = "d3d9.dll";
                if (config.installed.d912pxy != null && !(bool)config.disabled.d912pxy)
                {
                    if (config.installed.gw2radial == null || (bool)config.disabled.gw2radial)
                    { d912pxy_name = "d3d9_chainload.dll"; }
                    else
                    { d912pxy_name = "d912pxy.dll"; gw2radial_name = "d3d9_chainload.dll"; }
                }
                else if (config.installed.gw2radial != null && !(bool)config.disabled.gw2radial)
                {
                    gw2radial_name = "d3d9_chainload.dll";
                }
            }
            else if (config.installed.gw2radial != null && !(bool)config.disabled.gw2radial)
            {
                gw2radial_name = "d3d9.dll";
                if (config.installed.d912pxy != null && !(bool)config.disabled.d912pxy)
                {
                    d912pxy_name = "d912pxy.dll";
                }
            }

            if (arcdps_name != null)
            {
                if (File.Exists(bin64 + arcdps_name) && (config.installed.arcdps != arcdps_name))
                    File.Delete(bin64 + arcdps_name);

                File.Move(bin64 + config.installed.arcdps, bin64 + arcdps_name);
                config.installed.arcdps = arcdps_name;  //editing config file to match new plugin location
            }

            if (gw2radial_name != null)
            {
                if (File.Exists(bin64 + gw2radial_name) && (config.installed.gw2radial != gw2radial_name))
                    File.Delete(bin64 + gw2radial_name);

                File.Move(bin64 + config.installed.gw2radial, bin64 + gw2radial_name);
                config.installed.gw2radial = gw2radial_name;    //editing config file to match new plugin location
            }

            if (d912pxy_name != null)
            {
                if (File.Exists(bin64 + d912pxy_name) && (config.installed.d912pxy != d912pxy_name))
                    File.Delete(bin64 + d912pxy_name);

                File.Move(bin64 + config.installed.d912pxy, bin64 + d912pxy_name);
                config.installed.d912pxy = d912pxy_name;    //editing config file to match new plugin location
            }

            configuration.setConfig(config);    //writing updated config file

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

                RenamePlugins();
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

                RenamePlugins();
                configuration.DisplayAddonStatus(viewModel);
            }
        }

        //TODO: Write a function that just returns the right plugin names for arc, gw2radial, and d912pxy given the current configuration
    }
}
