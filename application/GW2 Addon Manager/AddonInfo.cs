using System;
using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    class AddonInfo
    {
        public string developer;
        public string website;
        public string addon_name;
        public string plugin_name;
        public string description;
        public string tooltip;

        public string plugin_location;

        public List<string> requires;
        public List<string> conflicts;
        public List<Dictionary<String, String>> alternate_plugin_names;
    }
}
