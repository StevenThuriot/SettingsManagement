using SettingsManagement.Interfaces;
using System.Configuration;

namespace SettingsManagement
{
    public class DefaultConfigurationManager : IConfigurationManager
    {
        Configuration _configurationManager;

        public void Open()
        {
            _configurationManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public string Get(string key)
        {
            return EnsureConfigurationManager().AppSettings.Settings[key]?.Value;
        }

        Configuration EnsureConfigurationManager()
        {
            if (_configurationManager == null)
                Open();

            return _configurationManager;
        }

        public void Refresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void Persist()
        {
            EnsureConfigurationManager().Save(ConfigurationSaveMode.Modified);
            Refresh();
        }

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

        public void Close()
        {
            _configurationManager = null;
        }


        public static void SetAsDefault()
        {
            DefaultSettings.SetAsDefault<DefaultConfigurationManager>();
        }
    }

}
