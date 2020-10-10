using System;
using System.Collections.Generic;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    [Obsolete]
    public class OldConfig
    {
        public string application_version;
        public string loader_version;
        public string bin_folder;
        public bool isupdate;
        public bool launch_game;
        public string game_path;
        public string exe_name;
        public string current_addon_list; // hash of the master branch
        public Dictionary<string, bool> default_configuration;
        public Dictionary<string, string> version;
        public Dictionary<string, string> installed;
        public Dictionary<string, bool> disabled;
    }
}