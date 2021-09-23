using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using Localization;

namespace GW2_Addon_Manager
{
    public interface IConfigurationManager
    {
        string ApplicationVersion { get; }

        UserConfig UserConfig { get; set; }
    }

    public class ConfigurationManager : IConfigurationManager
    {
        private const string ConfigFileName = "config.xml";
        private const string PathToOldConfigFile = "config.yaml";

        public string ApplicationVersion
        {
            get
            {
                var currentAppVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{currentAppVersion.Major}.{currentAppVersion.Minor}.{currentAppVersion.Build}";
            }
        }

        private UserConfig _userConfig;
        public UserConfig UserConfig
        {
            get => _userConfig;
            set {
                _userConfig = value;
                Save();
            }
        }

        public ConfigurationManager()
        {
            UserConfig = Load();
        }

        private void Save()
        {
            var xmlDoc = new XmlDocument();
            var serializer = new XmlSerializer(typeof(UserConfig));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, UserConfig);
                stream.Position = 0;
                xmlDoc.Load(stream);
                xmlDoc.Save(ConfigFileName);
            }
        }

        private UserConfig Load()
        {
            if (File.Exists(PathToOldConfigFile))
                return Migrate();

            if (!File.Exists(ConfigFileName))
                return UserConfig.Default;

            var xmlDocu = new XmlDocument();
            xmlDocu.Load(ConfigFileName);
            var xmlString = xmlDocu.OuterXml;

            using (var read = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(UserConfig));
                using (var reader = new XmlTextReader(read))
                {
                    return serializer.Deserialize(reader) as UserConfig;
                }
            }
        }

        private UserConfig Migrate()
        {
            var yamlConfigAsString = File.ReadAllText(PathToOldConfigFile);
#pragma warning disable CS0612 // Type or member is obsolete
            var oldUserConfig = new Deserializer().Deserialize<OldConfig>(yamlConfigAsString);
#pragma warning restore CS0612 // Type or member is obsolete

            var newConfig = new UserConfig(
                oldUserConfig.loader_version,
                oldUserConfig.bin_folder,
                oldUserConfig.launch_game,
                oldUserConfig.exe_name,
                CultureConstants.English,
                oldUserConfig.game_path,
                null,
                oldUserConfig.installed.ToDictionary(
                    kv => kv.Key,
                    kv => new AddonState(kv.Value, oldUserConfig.version[kv.Key], true, oldUserConfig.disabled[kv.Key])));

            File.Delete(PathToOldConfigFile);

            return newConfig;
        }
    }
}