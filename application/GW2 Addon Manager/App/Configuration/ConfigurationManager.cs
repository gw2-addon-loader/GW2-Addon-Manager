using System.Reflection;
using GW2_Addon_Manager.App.Configuration.Model;

namespace GW2_Addon_Manager.App.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        public string CurrentApplicationVersion
        {
            get
            {
                var currentAppVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{currentAppVersion.Major}.{currentAppVersion.Minor}.{currentAppVersion.Build}";
            }
        }

        public UserConfig UserConfig => Properties.Settings.Default.UserConfig ?? new UserConfig();

        public void SaveConfiguration() => Properties.Settings.Default.Save();
    }
}