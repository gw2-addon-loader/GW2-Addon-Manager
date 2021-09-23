using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// In-app corollary to update.yaml files.
    /// </summary>
    public class AddonInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// An event used to indicate that a property's value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles whether the Addon is selected or not in list boxes.
        /// </summary>
        public bool IsSelected { get; set; }

        //for descriptions of these fields, see /Resources/Addons/template-update.yaml
#pragma warning disable CS1591 // (Missing XML comment for publicly visible type or member)

        public string Developer { get; set; }
        public string Website { get; set; }

        public string AddonName { get; set; }
        public string Description { get; set; }
        public string Tooltip { get; set; }

        public string FolderName { get; set; }
        public string PluginName { get; set; }
        public string HostType { get; set; }
        public string HostUrl { get; set; }
        public string VersionUrl { get; set; }
        public string DownloadType { get; set; }
        public string InstallMode { get; set; }

        public List<string> Requires { get; set; }
        public List<string> Conflicts { get; set; }
        public List<Dictionary<string, string>> AlternatePluginNames { get; set; }

        public List<string> AdditionalFlags { get; set; }
    }
}
