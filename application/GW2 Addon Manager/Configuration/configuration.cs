using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using YamlDotNet.Serialization;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// The <c>configuration</c> class contains various functions accessing and modifying the configuration file and the configuration file template.
    /// </summary>
    class configuration
    {
        static string config_file_path = "config.yaml";
        static string config_template_path = "resources\\config_template.yaml";

        static string applicationRepoUrl = "https://api.github.com/repos/fmmmlee/GW2-Addon-Manager/releases/latest";

        public static config getConfigAsYAML()
        {
            String updateFile = null;

            if (File.Exists(config_file_path))
                updateFile = File.ReadAllText(config_file_path);
            else
                updateFile = File.ReadAllText(config_template_path);

            Deserializer toDynamic = new Deserializer();
            config user_config = toDynamic.Deserialize<config>(updateFile);
            return user_config;
        }

        public static void setConfigAsYAML(config info)
        {
            String config_file_path = "config.yaml";
            File.WriteAllText(config_file_path, new Serializer().Serialize(info));
        }

        public static void setTemplateYAML(config info)
        {
            File.WriteAllText(config_template_path, new Serializer().Serialize(info));
        }

        /// <summary>
        /// <c>getTemplateConfig</c> accesses the configuration file template found at <c>config_template_path</c>
        /// </summary>
        /// <returns> a config object representing the default configuration file template as serialized from YAML </returns>
        public static config getTemplateConfig()
        {
            String config_template_path = "resources\\config_template.yaml";

            string updateFile = File.ReadAllText(config_template_path);

            Deserializer toDynamic = new Deserializer();
            config user_config = toDynamic.Deserialize<config>(updateFile);
            return user_config;
        }

        /// <summary>
        /// Displays the latest status of the plugins on the opening screen (disabled, enabled, version, installed).
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void DisplayAddonStatus(OpeningViewModel viewModel)
        {
            config config_obj = getConfigAsYAML();

            foreach(AddonInfo addon in viewModel.AddonList)
            {
                if (config_obj.installed.ContainsKey(addon.folder_name) && config_obj.version.ContainsKey(addon.folder_name))
                {
                    if (addon.folder_name == "arcdps")
                        addon.addon_name += " (installed)";
                    else
                        addon.addon_name += " (" + config_obj.version[addon.folder_name] + " installed)";
                }
            }
            
        }

        /// <summary>
        /// <c>ChangeAddonConfig</c> writes the default add-ons section of the configuration file found at <c>config_file_path</c> using
        /// values found in <paramref name="viewModel"/>, which can be set by the user.
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void ChangeAddonConfig(OpeningViewModel viewModel)
        {
            config config_obj = getConfigAsYAML();
            foreach(AddonInfo addon in viewModel.AddonList)
            {
                if (addon.IsSelected)
                {
                    if (config_obj.default_configuration.ContainsKey(addon.folder_name))
                        config_obj.default_configuration[addon.folder_name] = true;
                    else
                        config_obj.default_configuration.Add(addon.folder_name, true);
                }
                else
                {
                    if (config_obj.default_configuration.ContainsKey(addon.folder_name))
                        config_obj.default_configuration[addon.folder_name] = false;
                }
                
            }
            setConfigAsYAML(config_obj);
        }


        /// <summary>
        /// <c>SetGamePath</c> both sets the game path for the current application session to <paramref name="path"/> and records it in the configuration file.
        /// </summary>
        /// <param name="path">The game path.</param>
        public static void SetGamePath(string path)
        {
            Application.Current.Properties["game_path"] = path.Replace("\\", "\\\\");
            config config_obj = configuration.getConfigAsYAML();
            config_obj.game_path = Application.Current.Properties["game_path"].ToString().Replace("\\\\", "\\");
            setConfigAsYAML(config_obj);
            DetermineSystemType();
        }

        /// <summary>
        /// Checks if there is a new version of the application available.
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void CheckSelfUpdates(OpeningViewModel viewModel)
        {
            string thisVersion = getConfigAsYAML().application_version;
            string latestVersion;

            dynamic release_info = UpdateHelpers.GitReleaseInfo(applicationRepoUrl);
            latestVersion = release_info.tag_name;

            if (latestVersion != thisVersion)
            {
                viewModel.UpdateAvailable = latestVersion + " available!";
                viewModel.UpdateLinkVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Attempts to read the game folder and determine whether the game is running on a 64 or 32-bit system.
        /// Based on that, sets the 'bin_folder' property in the configuration file.
        /// </summary>
        public static void DetermineSystemType()
        {
            config config = getConfigAsYAML();
            if (Directory.Exists(config.game_path.ToString()))
            {
                if (Directory.Exists(config.game_path.ToString() + "\\bin64"))
                {
                    config.bin_folder = "bin64";
                }
                else if (Directory.Exists(config.game_path.ToString() + "\\bin"))
                {
                    config.bin_folder = "bin";
                }
                setConfigAsYAML(config);
            }
        }
    }
}
