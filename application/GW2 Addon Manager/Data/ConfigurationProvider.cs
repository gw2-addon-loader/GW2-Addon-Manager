using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using Localization;
using Newtonsoft.Json;
using System.IO.Abstractions;

namespace GW2AddonManager
{
    public interface IConfigurationProvider
    {
        string ApplicationVersion { get; }

        Configuration UserConfig { get; set; }

        public string ConfigFileName { get; }
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        public string ConfigFileName => "config.json";

        public string ApplicationVersion
        {
            get
            {
                var currentAppVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{currentAppVersion.Major}.{currentAppVersion.Minor}.{currentAppVersion.Build}";
            }
        }

        private Configuration _userConfig;
        private readonly IFileSystem _fileSystem;

        public Configuration UserConfig
        {
            get => _userConfig;
            set {
                _userConfig = value;
                Save();
            }
        }

        public ConfigurationProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            UserConfig = Load();
        }

        private void Save()
        {
            var s = JsonConvert.SerializeObject((object)UserConfig);
            _fileSystem.File.WriteAllText(ConfigFileName, s);
        }

        private Configuration Load()
        {
            if (!_fileSystem.File.Exists(ConfigFileName))
                return Configuration.Default;

            var s = _fileSystem.File.ReadAllText(ConfigFileName);
            return JsonConvert.DeserializeObject<Configuration>(s);
        }
    }
}