using Localization;
using System;
using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    [Serializable]
    public record UserConfig(
        string LoaderVersion,
        string BinFolder,
        bool LaunchGame,
        string ExeName,
        string Culture,
        string GamePath,
        string AddonsHash,
        Dictionary<string, AddonState> AddonsState)
    {
        public static UserConfig Default => new UserConfig(null, "bin64", false, "Gw2-64.exe", CultureConstants.English, null, null, new Dictionary<string, AddonState>());
    }
}