using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    class UserConfig
    {
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
