using Localization;
using System;
using System.Collections.Generic;

namespace GW2AddonManager
{
    [Serializable]
    public record Configuration(
        string LoaderVersion,
        bool LaunchGame,
        string Culture,
        string GamePath,
        IReadOnlyList<string> LoaderInstalledFiles,
        IReadOnlyDictionary<string, AddonState> AddonsState)
    {
        public static Configuration Default => new Configuration(null, false, CultureConstants.English, null, new List<string>(), new Dictionary<string, AddonState>());
    }
}