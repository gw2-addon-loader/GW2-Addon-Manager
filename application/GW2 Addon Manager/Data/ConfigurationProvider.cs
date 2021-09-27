using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using Localization;
using Newtonsoft.Json;

namespace GW2_Addon_Manager
{
    public interface IConfigurationProvider
    {
        string ApplicationVersion { get; }

        Configuration UserConfig { get; set; }
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private const string ConfigFileName = "config.json";

        public string ApplicationVersion
        {
            get
            {
                var currentAppVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{currentAppVersion.Major}.{currentAppVersion.Minor}.{currentAppVersion.Build}";
            }
        }

        private Configuration _userConfig;
        public Configuration UserConfig
        {
            get => _userConfig;
            set {
                _userConfig = value;
                Save();
            }
        }

        public ConfigurationProvider()
        {
            UserConfig = Load();
        }

        private void Save()
        {
            var s = JsonConvert.SerializeObject((object)UserConfig);
            File.WriteAllText(ConfigFileName, s);
        }

        private Configuration Load()
        {
            if (!File.Exists(ConfigFileName))
                return Configuration.Default;

            var s = File.ReadAllText(ConfigFileName);
            return JsonConvert.DeserializeObject<Configuration>(s);
        }
    }
}