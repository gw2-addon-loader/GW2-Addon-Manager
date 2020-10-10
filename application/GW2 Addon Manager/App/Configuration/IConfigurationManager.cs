using GW2_Addon_Manager.App.Configuration.Model;

namespace GW2_Addon_Manager.App.Configuration
{
    /// <summary>
    ///     Manager responsible for all operations related to user configuration.
    ///     Because it is not static, it can be mocked and used in tests.
    ///     <seealso cref="UserConfig" />.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        ///     Formatted application assembly version.
        /// </summary>
        string ApplicationVersion { get; }

        /// <summary>
        ///     User configuration object.
        ///     It is deserialized from XML file once and then stored in memory.
        /// </summary>
        UserConfig UserConfig { get; }

        /// <summary>
        ///     Serializes configuration object to XML, and saves it to file.
        /// </summary>
        void SaveConfiguration();
    }
}