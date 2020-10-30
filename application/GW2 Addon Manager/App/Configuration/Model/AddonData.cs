using System;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    [Serializable]
    public class AddonData
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public bool Installed { get; set; }

        public bool Disabled { get; set; }
    }
}