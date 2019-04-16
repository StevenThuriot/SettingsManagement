#if NET452
using System.Web.Configuration;

namespace SettingsManagement
{
    /// <summary>
    /// Default Configuration Manager Implementation, wrapping around WebConfigurationManager.OpenWebConfiguration.
    /// </summary>
    /// <remarks>DefaultConfigurationManager should be used in a netstandard environment.</remarks>
    public class DefaultWebConfigurationManager : DefaultConfigurationManagerBase
    {
        /// <summary>
        /// Opens the Web Configuration Manager Instance.
        /// </summary>
        public override void Open()
        {
            _configurationManager = WebConfigurationManager.OpenWebConfiguration("~");
        }

        /// <summary>
        /// Sets this manager type as the default manager for the current context.
        /// </summary>
        public static void ConfigureAsDefault()
        {
            SettingsContext.AppContext.Manager = new DefaultWebConfigurationManager();
        }
    }
}
#endif