using Localization;
using System;
using System.Collections.Generic;

namespace GW2AddonManager
{
    [Serializable]
    public record Configuration(
        string LoaderVersion,
        string BinFolder,
        bool LaunchGame,
        string ExeName,
        string Culture,
        string GamePath,
        IReadOnlyList<string> LoaderInstalledFiles,
        IReadOnlyDictionary<string, AddonState> AddonsState)
    {
        public static Configuration Default => new Configuration(null, "bin64", false, "Gw2-64.exe", CultureConstants.English, null, new List<string>(), new Dictionary<string, AddonState>());
    }
}