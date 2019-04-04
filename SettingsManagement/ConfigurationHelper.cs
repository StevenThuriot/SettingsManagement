using System;
using System.Configuration;
using System.Reflection;

namespace SettingsManagement
{
    static class ConfigurationHelper
    {
        public static readonly MethodInfo OpenConfigurationMethod = new Func<Configuration>(OpenConfiguration).Method;
        public static readonly MethodInfo RefreshAppSettingsMethod = new Action(RefreshAppSettings).Method;
        public static readonly MethodInfo PersistMethod = new Action<Configuration>(Persist).Method;

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
            configuration.Save();
            RefreshAppSettings();
        }
    }
}
