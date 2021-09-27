using Newtonsoft.Json;
using System;

namespace GW2_Addon_Manager
{
    [JsonObject]
    public record AddonState(string Nickname, string Version, bool Installed, bool Disabled)
    {
        public static AddonState Default(string name) => new AddonState(name, null, false, false);
    }
}