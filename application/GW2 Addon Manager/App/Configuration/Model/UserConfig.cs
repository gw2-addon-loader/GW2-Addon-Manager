using Localization;
using System;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    /// <summary>
    /// Application configuration model.
    /// </summary>
    [Serializable]
    public class UserConfig
    {
        /// <summary>
        /// Addons loader version. 
        /// </summary>
        public string LoaderVersion { get; set; }

        /// <summary>
        /// Path to folder with game files. Depends on system type (32 or 64 bit).
        /// </summary>
        public string BinFolder { get; set; }

        /// <summary>
        /// Indicates if game should launch after addons update is completed. 
        /// </summary>
        public bool LaunchGame { get; set; }

        /// <summary>
        /// Path to game executable files. 
        /// </summary>
        public string GamePath { get; set; } = "C:\\Program Files\\Guild Wars 2";

        /// <summary>
        /// Game exe name. Depends on system type (32 or 64 bit). 
        /// </summary>
        public string ExeName { get; set; }

        /// <summary>
        /// Application language.
        /// </summary>
        public string Culture { get; set; } = CultureConstants.English;

        /// <summary>
        /// List of current addons. 
        /// </summary>
        public AddonsList AddonsList { get; set; } = new AddonsList();
    }
}