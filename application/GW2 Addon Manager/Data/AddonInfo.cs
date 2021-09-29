using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace GW2AddonManager
{
    public enum DownloadType
    {
        [EnumMember(Value = "dll")]
        DLL,
        [EnumMember(Value = "archive")]
        Archive
    }

    public enum InstallMode
    {
        [EnumMember(Value = "binary")]
        Binary,
        [EnumMember(Value = "arc")]
        ArcDPSAddon
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public record AddonInfo(
        string Nickname,
        string Developer,
        string Website,
        string AddonName,
        string Description,
        string Tooltip,
        string HostType,
        string HostUrl,
        string VersionUrl,
        [JsonConverter(typeof(StringEnumConverter))]
        DownloadType DownloadType,
        [JsonConverter(typeof(StringEnumConverter))]
        InstallMode InstallMode,
        string PluginName,
        string PluginNamePattern,
        List<string> Files,
        List<string> Requires,
        List<string> Conflicts,
        string VersionId,
        bool VersionIdIsHumanReadable,
        string DownloadUrl,
        bool SelfUpdate);
}
