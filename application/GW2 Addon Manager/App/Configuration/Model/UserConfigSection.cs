using System.Configuration;
using System.Windows.Markup.Localizer;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    public class UserConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(nameof(BinFolder))]
        public string BinFolder
        {
            get => (string) this[nameof(BinFolder)];
            set => this[nameof(BinFolder)] = value;
        }

        [ConfigurationProperty(nameof(LaunchGame))]
        public bool LaunchGame
        {
            get => (bool) this[nameof(LaunchGame)];
            set => this[nameof(LaunchGame)] = value;
        }

        [ConfigurationProperty(nameof(GamePath))]
        public string GamePath
        {
            get => (string) this[nameof(GamePath)];
            set => this[nameof(GamePath)] = value;
        }

        [ConfigurationProperty(nameof(ExeName))]
        public string ExeName
        {
            get => (string) this[nameof(ExeName)];
            set => this[nameof(ExeName)] = value;
        }

        [ConfigurationProperty(nameof(AddonsList), IsDefaultCollection = false)]
        public AddonsList AddonsList => (AddonsList) this[nameof(AddonsList)];
    }
}