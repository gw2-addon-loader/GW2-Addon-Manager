using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Intended to read update.yaml files provided in addons that adhere to a specific set of specifications laid out for use by GW2-UOAOM and GW2-AddOn-Loader.
    /// </summary>
    class AddonYamlReader
    {
        private IDeserializer deserializer;

        public AddonYamlReader()
        {
            deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        /// <summary>
        /// Gets info for an add-on from update.yaml provided by the author or packaged with the application (when the author hasn't written one yet).
        /// </summary>
        /// <param name="name">The name of the addon folder to read from.</param>
        /// <returns>An object with the information from update.yaml</returns>
        public AddonInfo GetAddonInfo(string name)
        {
            string yamlPath = $"resources\\addons\\{name}\\update.yaml";
            string placeholderYamlPath = $"resources\\addons\\{name}\\update-placeholder.yaml";
            string updateFile = null;

            if(File.Exists(yamlPath))
                updateFile = File.ReadAllText(yamlPath);
            else if(File.Exists(placeholderYamlPath))
                updateFile = File.ReadAllText(placeholderYamlPath);

            return deserializer.Deserialize<AddonInfo>(updateFile);
        }

        /// <summary>
        /// Checks for a copy of Update.yaml in the directory given in <paramref name="searchFolder"/> and if present copies it to an addon application resource folder specified in <paramref name="name"/>.
        /// </summary>
        public void CheckForUpdateYaml(string name, string searchFolder)
        {
            string yamlPath = Path.Combine(searchFolder, "update.yaml");
            if (File.Exists(yamlPath))
                File.Copy(yamlPath, $"resources\\addons\\{name}\\update.yaml", true);
        }
    }
}
