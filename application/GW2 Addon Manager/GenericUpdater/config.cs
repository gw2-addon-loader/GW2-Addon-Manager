using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2_Addon_Manager
{
    class config
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
