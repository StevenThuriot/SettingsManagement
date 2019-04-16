using SettingsManagement.Interfaces;
using System.Configuration;

namespace SettingsManagement
{
    /// <summary>
    /// Default Configuration Manager Implementation
    /// </summary>
    public abstract class DefaultConfigurationManagerBase : IConfigurationManager
    {
        /// <summary>
        /// The Configuration Instance
        /// </summary>
        protected Configuration _configurationManager;

        /// <summary>
        /// Gets an appSetting value
        /// </summary>
        /// <param name="key">The appSetting Key</param>
        /// <returns></returns>
        public string Get(string key)
        {
            return EnsureConfigurationManager().AppSettings.Settings[key]?.Value;
        }

        /// <summary>
        /// Ensures the _configurationManager instance.
        /// </summary>
        /// <returns></returns>
        protected Configuration EnsureConfigurationManager()
        {
            if (_configurationManager == null)
                Open();

            return _configurationManager;
        }

        /// <summary>
        /// Refreshes the appSettings Section
        /// </summary>
        public void Refresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Persists the appSettings Changes
        /// </summary>
        public void Persist()
        {
            EnsureConfigurationManager().Save(ConfigurationSaveMode.Modified);
            Refresh();
        }

        /// <summary>
        /// Sets an appSetting value
        /// </summary>
        /// <param name="key">The appSetting Key</param>
        /// <param name="value">The appSetting Value</param>
        public void Set(string key, string value)
        {
            var configuration = EnsureConfigurationManager();

            var item = configuration.AppSettings.Settings[key];
            if (item == null)
            {
                configuration.AppSettings.Settings.Add(key, value);
            }
            else
            {
                item.Value = value;
            }
        }

        /// <summary>
        /// Closes the Configuration Manager Instance
        /// </summary>
        public void Close()
        {
            _configurationManager = null;
        }

        /// <summary>
        /// Sets this instance as the default manager for the current context.
        /// </summary>
        public void SetAsDefault()
        {
            SettingsContext.Current.Manager = this;
        }

        /// <summary>
        /// Opens the Configuration Manager Instance.
        /// </summary>
        public abstract void Open();
    }
}
