using System.IO;
using YamlDotNet.Serialization;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Intended to read update.yaml files provided in addons that adhere to a specific set of specifications laid out for use by GW2-UOAOM and GW2-AddOn-Loader.
    /// </summary>
    class UpdateYamlReader
    {
        /// <summary>
        /// Gets info for an add-on from update.yaml provided by the author or packaged with the application (when the author hasn't written one yet).
        /// </summary>
        /// <param name="name">The name of the addon folder to read from.</param>
        /// <returns>An object with the information from update.yaml</returns>
        public static AddonInfo getBuiltInInfo(string name)
        {
            string yamlPath = "resources\\addons\\" + name + "\\update.yaml";
            string placeholderYamlPath = "resources\\addons\\" + name + "\\update-placeholder.yaml";
            string updateFile = null;
            if(File.Exists(yamlPath))
                updateFile = File.ReadAllText(yamlPath);
            else if(File.Exists(placeholderYamlPath))
                updateFile = File.ReadAllText(placeholderYamlPath);

            Deserializer toDynamic = new Deserializer();
            AddonInfo info = toDynamic.Deserialize<AddonInfo>(updateFile);
            return info;
        }

        /// <summary>
        /// Checks for a copy of Update.yaml in the directory given in <paramref name="search_folder"/> and if present copies it to an addon application resource folder specified in <paramref name="name"/>.
        /// </summary>
        public static void CheckForUpdateYaml(string name, string search_folder)
        {
            string yamlPath = Path.Combine(search_folder, "update.yaml");
            if (File.Exists(yamlPath))
                File.Copy(yamlPath, "resources\\addons\\" + name + "\\update.yaml", true);
        }
    }
}
