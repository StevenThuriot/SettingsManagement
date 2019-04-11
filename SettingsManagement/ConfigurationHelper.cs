using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace SettingsManagement
{
    static class ConfigurationHelper
    {
        public static readonly MethodInfo OpenConfigurationMethod = typeof(ConfigurationHelper).GetMethod(nameof(OpenConfiguration), BindingFlags.Public | BindingFlags.Static);
        public static readonly MethodInfo RefreshAppSettingsMethod = typeof(ConfigurationHelper).GetMethod(nameof(RefreshAppSettings), BindingFlags.Public | BindingFlags.Static);
        public static readonly MethodInfo PersistMethod = typeof(ConfigurationHelper).GetMethod(nameof(Persist), BindingFlags.Public | BindingFlags.Static);

        static ConfigurationHelper()
        {
            Debug.Assert(OpenConfigurationMethod != null);
            Debug.Assert(RefreshAppSettingsMethod != null);
            Debug.Assert(PersistMethod != null);
        }


        public static Configuration OpenConfiguration()
        {
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public static void RefreshAppSettings()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void Persist(Configuration configuration)
        {
            configuration.Save(ConfigurationSaveMode.Modified);
            RefreshAppSettings();
        }
    }
}
