using System.Configuration;

namespace SettingsManagement
{
    /// <summary>
    /// Default Configuration Manager Implementation, wrapping around ConfigurationManager.OpenExeConfiguration.
    /// </summary>
    public class DefaultConfigurationManager : DefaultConfigurationManagerBase
    {
        /// <summary>
        /// Opens the Configuration Manager Instance.
        /// </summary>
        public override void Open()
        {
            _configurationManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        /// <summary>
        /// Sets this manager type as the default manager for the current context.
        /// </summary>
        public static void ConfigureAsDefault()
        {
            SettingsContext.AppContext.Manager = new DefaultConfigurationManager();
        }
    }
}
