using Localization;
using System;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    [Serializable]
    public class UserConfig
    {
        public string LoaderVersion { get; set; }

        public string BinFolder { get; set; }

        public bool LaunchGame { get; set; }

        public string GamePath { get; set; } = "C:\\Program Files\\Guild Wars 2";

        public string ExeName { get; set; }

        public string Culture { get; set; } = CultureConstants.English;

        public AddonsList AddonsList { get; set; } = new AddonsList();
    }
}