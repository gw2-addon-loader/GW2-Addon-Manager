
using Newtonsoft.Json;
using System.IO;

namespace GW2_Addon_Manager
{
    class configuration
    {
        static string config_file_path = "config.ini";

        public static dynamic getConfig()
        {
            string config_file = File.ReadAllText(config_file_path);
            dynamic config_obj = JsonConvert.DeserializeObject(config_file);
            return config_obj;
        }

        public static void setConfig(dynamic config_obj)
        {
            string edited_config_file = JsonConvert.SerializeObject(config_obj, Formatting.Indented);
            File.WriteAllText(config_file_path, edited_config_file);
        }
    }
}
