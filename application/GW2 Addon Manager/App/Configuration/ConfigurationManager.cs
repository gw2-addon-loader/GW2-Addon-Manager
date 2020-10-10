using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using GW2_Addon_Manager.App.Configuration.Model;
using YamlDotNet.Serialization;

namespace GW2_Addon_Manager.App.Configuration
{
    /// <inheritdoc />
    public class ConfigurationManager : IConfigurationManager
    {
        private const string ConfigFileName = "config.xml";

        private static readonly string ConfigFolder =
            Path.GetDirectoryName(
                Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        private static readonly string PathToConfgiFile = $"{ConfigFolder}\\{ConfigFileName}";
        private const string PathToOldConfigFile = "config.yaml";

        private static readonly UserConfig UserConfigInstance = CreateConfig();

        /// <inheritdoc />
        public string ApplicationVersion
        {
            get
            {
                var currentAppVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{currentAppVersion.Major}.{currentAppVersion.Minor}.{currentAppVersion.Build}";
            }
        }

        /// <inheritdoc />
        public UserConfig UserConfig => UserConfigInstance;

        /// <inheritdoc />
        public void SaveConfiguration()
        {
            var xmlDoc = new XmlDocument();
            var serializer = new XmlSerializer(typeof(UserConfig));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, UserConfig);
                stream.Position = 0;
                xmlDoc.Load(stream);
                xmlDoc.Save(PathToConfgiFile);
            }
        }

        private static UserConfig CreateConfig()
        {
            if (File.Exists(PathToOldConfigFile))
                return MigrateOldConfig();

            if (!File.Exists(PathToConfgiFile))
                return new UserConfig();

            var xmlDocu = new XmlDocument();
            xmlDocu.Load(PathToConfgiFile);
            var xmlString = xmlDocu.OuterXml;

            using (var read = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(UserConfig));
                using (var reader = new XmlTextReader(read))
                {
                    return (UserConfig)serializer.Deserialize(reader);
                }
            }
        }

        private static UserConfig MigrateOldConfig()
        {
            var yamlConfigAsString = File.ReadAllText(PathToOldConfigFile);
            var deserializer = new Deserializer();
            var oldUserConfig = deserializer.Deserialize<OldConfig>(yamlConfigAsString);

            var newConfig = new UserConfig
            {
                BinFolder = oldUserConfig.bin_folder,
                ExeName = oldUserConfig.exe_name,
                GamePath = oldUserConfig.game_path,
                LaunchGame = oldUserConfig.launch_game,
                LoaderVersion = oldUserConfig.loader_version,
                AddonsList = new AddonsList { Hash = oldUserConfig.current_addon_list }
            };
            foreach (var installedAddon in oldUserConfig.installed)
                newConfig.AddonsList.Add(new AddonData
                {
                    Name = installedAddon.Value,
                    Installed = true,
                    Disabled = oldUserConfig.disabled[installedAddon.Key],
                    Version = oldUserConfig.version[installedAddon.Key]
                });

            File.Delete(PathToOldConfigFile);

            return newConfig;
        }
    }
}