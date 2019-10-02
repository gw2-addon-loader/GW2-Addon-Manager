using System;
using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// In-app corollary to update.yaml files.
    /// </summary>
    public class AddonInfo
    {
        //for xaml
        public bool IsSelected { get; set; }

        public string developer { get; set; }
        public string website { get; set; }
        public string addon_name { get; set; }
        public string description { get; set; }
        public string tooltip { get; set; }

        public string folder_name { get; set; }
        public string host_type { get; set; }
        public string host_url { get; set; }
        public string version_url { get; set; }
        public string download_type { get; set; }
        public string install_mode { get; set; }

        public List<string> requires { get; set; }
        public List<string> conflicts { get; set; }
        public List<Dictionary<String, String>> alternate_plugin_names { get; set; }
    }
}
