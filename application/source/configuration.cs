using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// The <c>configuration</c> class contains various functions accessing and modifying the configuration file.
    /// </summary>
    class configuration
    {
        static string config_file_path = "config.ini";

        /// <summary>
        /// <c>getConfig</c> accesses a configuration file found at <c>config_file_path</c>, which should adhere to proper Json syntax.
        /// </summary>
        /// <returns> a dynamic object representing the configuration file as converted from Json using Json.NET </returns>
        public static dynamic getConfig()
        {
            string config_file = File.ReadAllText(config_file_path);
            dynamic config_obj = JsonConvert.DeserializeObject(config_file);
            return config_obj;
        }

        /// <summary>
        /// <c>setConfig</c> overwrites the configuration file found at <c>config_file_path</c> with a Json string created by serializing<paramref name="config_obj"/>.
        /// </summary>
        /// <param name="config_obj"> the dynamic object to be serialized and written to the file </param>
        public static void setConfig(dynamic config_obj)
        {
            string edited_config_file = JsonConvert.SerializeObject(config_obj, Formatting.Indented);
            File.WriteAllText(config_file_path, edited_config_file);
        }


        /// <summary>
        /// <c>ApplyDefaultConfig</c> reads the configuration file found at <c>config_file_path</c> and uses the information
        /// within to set properties across <paramref name="viewModel"/>.
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void ApplyDefaultConfig(OpeningViewModel viewModel)
        {
            dynamic config_obj = getConfig();

            if ((bool)config_obj.default_configuration.arcdps)
                viewModel.ArcDPS_CheckBox = true;
            if ((bool)config_obj.default_configuration.gw2radial)
                viewModel.GW2Radial_CheckBox = true;
            if ((bool)config_obj.default_configuration.d912pxy)
                viewModel.d912pxy_CheckBox = true;

            if (config_obj.version.arcdps != null)
                viewModel.ArcDPS_Content = "ArcDPS (installed)";

            if (config_obj.version.gw2radial != null)
                viewModel.GW2Radial_Content = "GW2 Radial (" + config_obj.version.gw2radial + " installed)";

            if (config_obj.version.d912pxy != null)
                viewModel.d912pxy_Content = "d912pxy (" + config_obj.version.d912pxy + " installed)";
        }

        /// <summary>
        /// <c>ChangeAddonConfig</c> writes the default add-ons section of the configuration file found at <c>config_file_path</c> using
        /// values found in <paramref name="viewModel"/>, which can be set by the user.
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void ChangeAddonConfig(OpeningViewModel viewModel)
        {
            dynamic config_obj = getConfig();

            if (viewModel.ArcDPS_CheckBox)
                config_obj.default_configuration.arcdps = true;
            else
                config_obj.default_configuration.arcdps = false;

            if (viewModel.GW2Radial_CheckBox)
                config_obj.default_configuration.gw2radial = true;
            else
                config_obj.default_configuration.gw2radial = false;

            if (viewModel.d912pxy_CheckBox)
                config_obj.default_configuration.d912pxy = true;
            else
                config_obj.default_configuration.d912pxy = false;

            setConfig(config_obj);
        }


        /// <summary>
        /// <c>SetGamePath</c> both sets the game path for the current application session to <paramref name="path"/> and records it in the configuration file.
        /// </summary>
        /// <param name="path">The game path.</param>
        public static void SetGamePath(string path)
        {
            Application.Current.Properties["game_path"] = path.Replace("\\", "\\\\");
            dynamic config_obj = configuration.getConfig();
            config_obj.game_path = Application.Current.Properties["game_path"].ToString().Replace("\\\\", "\\");
            configuration.setConfig(config_obj);
        }
    }
}
