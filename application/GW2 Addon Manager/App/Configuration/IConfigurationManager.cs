namespace GW2_Addon_Manager.App.Configuration
{
    public interface IConfigurationManager
    {
        string CurrentApplicationVersion { get; }
        Model.UserConfig UserConfig { get; }

        void SaveConfiguration();
    }
}