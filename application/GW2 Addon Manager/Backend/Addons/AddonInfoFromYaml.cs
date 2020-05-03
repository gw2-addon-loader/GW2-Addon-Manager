using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// In-app corollary to update.yaml files.
    /// </summary>
    public class AddonInfoFromYaml : INotifyPropertyChanged
    {
        /// <summary>
        /// An event used to indicate that a property's value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        //for descriptions of these fields, see /Resources/Addons/template-update.yaml
#pragma warning disable CS1591 // (Missing XML comment for publicly visible type or member)
        public bool IsSelected { get; set; }

        public string developer { get; set; }
        public string website { get; set; }

        private string _addon_name;
        public string addon_name
        {
            get { return _addon_name; }
            set
            {
                _addon_name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(addon_name)));
            }
        }
        public string description { get; set; }
        public string tooltip { get; set; }

        public string folder_name { get; set; }
        public string plugin_name { get; set; }
        public string host_type { get; set; }
        public string host_url { get; set; }
        public string version_url { get; set; }
        public string download_type { get; set; }
        public string install_mode { get; set; }

        public List<string> requires { get; set; }
        public List<string> conflicts { get; set; }
        public List<Dictionary<String, String>> alternate_plugin_names { get; set; }

        public List<string> additional_flags { get; set; }
    }
}
