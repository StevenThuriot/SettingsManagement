using SettingsManagement.Interfaces;

namespace SettingsManagement
{
    /// <summary>
    /// Default Settings for Manager Building
    /// </summary>
    public static class DefaultSettings
    {
        /// <summary>
        /// Default Manager for App 
        /// </summary>
        /// <remarks>Refactor to SettingsContext --> App Level</remarks>
        public static IConfigurationManager Manager { get; set; }

        /// <summary>
        /// Wether or not to try and auto-resolve a manager. This only works if you have _one_ single manager implementation loaded.
        /// </summary>
        /// <remarks>Default Value == true</remarks>
        public static bool TryToAutoResolveManager { get; set; } = true;

        /// <summary>
        /// Set a manager instance as default
        /// </summary>
        /// <param name="manager"></param>
        public static void SetAsDefault(this IConfigurationManager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Set a manager type as default
        /// </summary>
        public static void SetAsDefault<T>()
            where T : IConfigurationManager, new()
        {
            Manager = new T();
        }
    }
}
