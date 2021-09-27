using Localization;
using System;
using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    [Serializable]
    public record Configuration(
        string LoaderVersion,
        string BinFolder,
        bool LaunchGame,
        string ExeName,
        string Culture,
        string GamePath,
        ulong AddonsHash,
        IReadOnlyDictionary<string, AddonState> AddonsState)
    {
        public static Configuration Default => new Configuration(null, "bin64", false, "Gw2-64.exe", CultureConstants.English, null, 0, new Dictionary<string, AddonState>());
    }
}