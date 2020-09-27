using System.Configuration;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    public class AddonData : ConfigurationElement
    {
        [ConfigurationProperty(nameof(Name), IsKey = true, IsRequired = true)]
        public string Name
        {
            get => (string) this[nameof(Name)];
            set => this[nameof(Name)] = value;
        }

        [ConfigurationProperty(nameof(Version))]
        public string Version
        {
            get => (string) this[nameof(Version)];
            set => this[nameof(Version)] = value;
        }

        [ConfigurationProperty(nameof(Installed))]
        public bool Installed
        {
            get => (bool) this[nameof(Installed)];
            set => this[nameof(Installed)] = value;
        }

        [ConfigurationProperty(nameof(Disabled))]
        public bool Disabled
        {
            get => (bool) this[nameof(Disabled)];
            set => this[nameof(Disabled)] = value;
        }
    }
}