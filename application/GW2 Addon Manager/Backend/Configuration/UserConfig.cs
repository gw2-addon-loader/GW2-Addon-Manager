using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    public class UserConfig
    {

#pragma warning disable CS0649//disabled warning since these values are set during object construction by YAML deserializer and aren't modified after that
#pragma warning disable CS1591//purpose of each field should be largely self-explanatory
        public string application_version;
        public string loader_version;
        public string bin_folder;
        public bool isupdate;
        public string game_path;
        public Dictionary<string, bool> default_configuration;
        public Dictionary<string, string> version;
        public Dictionary<string, string> installed;
        public Dictionary<string, bool> disabled;
    }
}
