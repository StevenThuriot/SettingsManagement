using SettingsManagement.Interfaces;
using System.Configuration;

namespace SettingsManagement
{
    public class DefaultConfigurationManager : IConfigurationManager
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public void Refresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void Persist()
        {
            //Save??
            Refresh();
        }

        public void Set(string key, string value)
        {
            ConfigurationManager.AppSettings[key] = value;
        }
    }

}
