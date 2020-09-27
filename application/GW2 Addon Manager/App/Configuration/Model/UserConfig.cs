using System.Configuration;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class UserConfig
    {
        public string ApplicationVersion { get; set; }

        public string LoaderVersion { get; set; }

        public string BinFolder { get; set; }

        public bool LaunchGame { get; set; }

        public string GamePath { get; set; } = "C:\\Program Files\\Guild Wars 2";

        public string ExeName { get; set; }

        public AddonsList AddonsList { get; set; } = new AddonsList();
    }
}