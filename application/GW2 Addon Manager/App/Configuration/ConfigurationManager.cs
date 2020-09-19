using System.Configuration;
using GW2_Addon_Manager.App.Configuration.Model;

namespace GW2_Addon_Manager.App.Configuration
{
    public static class ConfigurationManager
    {
        public static UserConfigSection UserConfig =>
            System.Configuration.ConfigurationManager.GetSection(nameof(UserConfigSection)) as UserConfigSection;

        public static void SaveConfiguration()
        {
            UserConfig.CurrentConfiguration.Save(ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection(UserConfig.SectionInformation.Name);
        }
    }
}