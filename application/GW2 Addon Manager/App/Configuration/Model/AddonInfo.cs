using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GW2_Addon_Manager
{
    public record AddonInfo(
        string Developer,
        string Website,
        string AddonName,
        string Description,
        string Tooltip,
        string FolderName,
        string PluginName,
        string HostType,
        string HostUrl,
        string VersionUrl,
        string DownloadType,
        string InstallMode,
        List<string> Requires,
        List<string> Conflicts,
        List<Dictionary<string, string>> AlternatePluginNames,
        List<string> AdditionalFlags);
}
