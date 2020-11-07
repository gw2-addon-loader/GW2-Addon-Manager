using System;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    /// <summary>
    ///     Addon configuration data model.
    /// </summary>
    [Serializable]
    public class AddonData
    {
        /// <summary>
        ///     Addon name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Currently installed addon version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Determines if addon is installed.
        /// </summary>
        public bool Installed { get; set; }

        /// <summary>
        ///     Determines if addon is disabled.
        /// </summary>
        public bool Disabled { get; set; }
    }
}