using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    [JsonObject]
    public record AddonState(string Nickname, string VersionId, bool Installed, bool Disabled, List<string> InstalledFiles)
    {
        public static AddonState Default(string name) => new AddonState(name, null, false, false, new List<string>());
    }
}