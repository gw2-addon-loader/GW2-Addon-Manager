using System.IO;
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
                string gamePath = configuration.getConfigAsYAML().game_path;


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
            config config = configuration.getConfigAsYAML();

            string bin64 = config.game_path + "\\bin64\\";

            if (config.installed["arcdps"] != null && !config.disabled["arcdps"])
            {
                arcdps_name = "d3d9.dll";
                if (config.installed["d912pxy"] != null && !config.disabled["d912pxy"])
                {
                    if (config.installed["gw2radial"] == null || config.disabled["gw2radial"])
                    { d912pxy_name = "d3d9_chainload.dll"; }
                    else
                    { d912pxy_name = "d912pxy.dll"; gw2radial_name = "d3d9_chainload.dll"; }
                }
                else if (config.installed["gw2radial"] != null && !config.disabled["gw2radial"])
                {
                    gw2radial_name = "d3d9_chainload.dll";
                }
            }
            else if (config.installed["gw2radial"] != null && !config.disabled["gw2radial"])
            {
                gw2radial_name = "d3d9.dll";
                if (config.installed["d912pxy"] != null && !config.disabled["d912pxy"])
                {
                    d912pxy_name = "d912pxy.dll";
                }
            }

            if (arcdps_name != null)
            {
                if (File.Exists(bin64 + arcdps_name) && (config.installed["arcdps"] != arcdps_name))
                    File.Delete(bin64 + arcdps_name);

                File.Move(bin64 + config.installed["arcdps"], bin64 + arcdps_name);
                config.installed["arcdps"] = arcdps_name;  //editing config file to match new plugin location
            }

            if (gw2radial_name != null)
            {
                if (File.Exists(bin64 + gw2radial_name) && (config.installed["gw2radial"] != gw2radial_name))
                    File.Delete(bin64 + gw2radial_name);

                File.Move(bin64 + config.installed["gw2radial"], bin64 + gw2radial_name);
                config.installed["gw2radial"] = gw2radial_name;    //editing config file to match new plugin location
            }

            if (d912pxy_name != null)
            {
                if (File.Exists(bin64 + d912pxy_name) && (config.installed["d912pxy"] != d912pxy_name))
                    File.Delete(bin64 + d912pxy_name);

                File.Move(bin64 + config.installed["d912pxy"], bin64 + d912pxy_name);
                config.installed["d912pxy"] = d912pxy_name;    //editing config file to match new plugin location
            }

            configuration.setConfigAsYAML(config);    //writing updated config file

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
                string gamePath = configuration.getConfigAsYAML().game_path;


                RenamePlugins();
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
                string gamePath = configuration.getConfigAsYAML().game_path;


                RenamePlugins();
                configuration.DisplayAddonStatus(viewModel);
            }
        }
    }
}
