using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;
using YamlDotNet.Serialization;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// The <c>configuration</c> class contains various functions accessing and modifying the configuration file and the configuration file template.
    /// </summary>
    class configuration
    {
        static string config_file_path = "config.ini";
        static string config_template_path = "resources\\config_template.ini";

        static string applicationRepoUrl = "https://api.github.com/repos/fmmmlee/GW2-Addon-Manager/releases/latest";


        //add convert-to-yaml method

        public static config getConfigAsYAML()
        {
            String updateFile = null;
            String yamlPath = "config.yaml";
            String yamlTemplatePath = "resources\\config_template.yaml";

            if (File.Exists(yamlPath))
                updateFile = File.ReadAllText(yamlPath);
            else
                updateFile = File.ReadAllText(yamlTemplatePath);

            Deserializer toDynamic = new Deserializer();
            config user_config = toDynamic.Deserialize<config>(updateFile);
            return user_config;
        }

        public static void setConfigAsYAML(config info)
        {
            String yamlPath = "config.yaml";
            File.WriteAllText(yamlPath, new Serializer().Serialize(info));
        }

        public static void setTemplateYAML(config info)
        {
            File.WriteAllText(config_template_path, new Serializer().Serialize(info));
        }

        /// <summary>
        /// <c>getConfigAsJObject</c> accesses a configuration file found at <c>config_file_path</c>, which should adhere to proper Json syntax.
        /// </summary>
        /// <returns> a <c>JObject</c> representing the configuration file as converted from Json using JObject.Parse() </returns>
        public static JObject getConfigAsJObject()
        {
            string config_file = File.ReadAllText(config_file_path);
            JObject config_obj = JObject.Parse(config_file);
            return config_obj;
        }

        /// <summary>
        /// <c>getTemplateConfig</c> accesses the configuration file template found at <c>config_template_path</c>, which should adhere to proper Json syntax.
        /// </summary>
        /// <returns> a dynamic object representing the default configuration file template as converted from Json using Json.NET </returns>
        public static dynamic getTemplateConfig()
        {
            string config_file = File.ReadAllText(config_template_path);
            dynamic config_obj = JsonConvert.DeserializeObject(config_file);
            return config_obj;
        }

        /// <summary>
        /// <c>getTemplateConfigAsJObject</c> accesses the configuration file template found at <c>config_template_path</c>, which should adhere to proper Json syntax.
        /// </summary>
        /// <returns> an instance of <c>JObject</c> representing the default configuration file template as converted from Json using Json.NET </returns>
        public static JObject getTemplateConfigAsJObject()
        {
            string config_file = File.ReadAllText(config_template_path);
            JObject config_obj = JObject.Parse(config_file);
            return config_obj;
        }

        /// <summary>
        /// Overwrites the configuration file template in the application's
        /// /resources/ folder with a string created by serializing <paramref name="config_obj"/>.
        /// </summary>
        /// <param name="config_obj"></param>
        public static void setConfigTemplate(dynamic config_obj)
        {
            string edited_config_file = JsonConvert.SerializeObject(config_obj, Formatting.Indented);
            File.WriteAllText(config_template_path, edited_config_file);
        }

        /// <summary>
        /// <c>ApplyDefaultConfig</c> reads the configuration file found at <c>config_file_path</c>
        /// and uses the information within to set properties across <paramref name="viewModel"/>.
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void ApplyDefaultConfig(OpeningViewModel viewModel)
        {
            config config_obj = getConfigAsYAML();

           

            DisplayAddonStatus(viewModel);
        }

        /// <summary>
        /// Displays the latest status of the plugins on the opening screen (disabled, enabled, version, installed).
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void DisplayAddonStatus(OpeningViewModel viewModel)
        {
            config config_obj = getConfigAsYAML();

            
        }

        /// <summary>
        /// <c>ChangeAddonConfig</c> writes the default add-ons section of the configuration file found at <c>config_file_path</c> using
        /// values found in <paramref name="viewModel"/>, which can be set by the user.
        /// </summary>
        /// <param name="viewModel">An instance of the <typeparamref>OpeningViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public static void ChangeAddonConfig(OpeningViewModel viewModel)
        {
            config config_obj = getConfigAsYAML();

            
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
        /// <c>SelfVersionStatus</c> either creates config.ini according to the template in /resources/, copies data from a previous installation to
        /// an updated configuration file template and then overwrites config.ini, or does nothing, depending on
        /// the user's installation.
        /// </summary>
        public static void ConfigFileStatus()
        {
            if (File.Exists(config_file_path))
            {
                JObject template_config = getTemplateConfigAsJObject();
                JObject current_config = getConfigAsJObject();

                /* if update flag set */
                if ((bool)template_config.GetValue("isupdate"))
                {
                    /* copy info from old config to template */
                    foreach (var property in current_config)
                    {
                        if (template_config.ContainsKey(property.Key) && property.Key != "application_version")
                        {
                            if (property.Value.HasValues)
                            {
                                JObject child = (JObject)property.Value;

                                foreach (var child_property in child)
                                {
                                    template_config[property.Key][child_property.Key] = child_property.Value;
                                }
                            }
                            else
                            {
                                template_config[property.Key] = property.Value;
                            }
                            
                        }


                    }
                    /* convert the updated JObject to a dynamic and overwrite config.ini */
                    setConfig(template_config.ToObject<dynamic>());

                    dynamic template_disable_update = getTemplateConfig();  //get the unedited config template
                    template_disable_update.isupdate = false;               //set isupdate to false, so program knows not to re-perform config file swapping
                    setConfigTemplate(template_disable_update);             //save to the config file template path
                }
            }
            else
            {
                File.Copy(config_template_path, config_file_path);

                dynamic template_disable_update = getTemplateConfig();  //get the unedited config template
                template_disable_update.isupdate = false;               //set isupdate to false, so program knows not to re-perform config file swapping
                setConfigTemplate(template_disable_update);             //save to the config file template path

                Application.Current.Properties["newinstall"] = true;
                return;
            }
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
