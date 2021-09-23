using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Intended to read update.yaml files provided in addons that adhere to a specific set of specifications laid out for use by GW2-UOAOM and GW2-AddOn-Loader.
    /// </summary>
    public class AddonYamlReader
    {
        private readonly IDeserializer _deserializer;

        public AddonYamlReader()
        {
            _deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public AddonInfo GetAddonInfo(string name)
        {
            string yamlPath = $"resources\\addons\\{name}\\update.yaml";
            string placeholderYamlPath = $"resources\\addons\\{name}\\update-placeholder.yaml";
            string updateFile = null;

            if(File.Exists(yamlPath))
                updateFile = File.ReadAllText(yamlPath);
            else if(File.Exists(placeholderYamlPath))
                updateFile = File.ReadAllText(placeholderYamlPath);

            if(updateFile is null)
                return null;

            var info = _deserializer.Deserialize<AddonInfo>(updateFile);
            return info with {
                FolderName = Path.GetRelativePath(Constants.AddonFolder, info.FolderName)
            };
        }

        public void CheckForUpdateYaml(string name, string searchFolder)
        {
            string yamlPath = Path.Combine(searchFolder, "update.yaml");
            if (File.Exists(yamlPath))
                File.Copy(yamlPath, $"resources\\addons\\{name}\\update.yaml", true);
        }
    }
}
