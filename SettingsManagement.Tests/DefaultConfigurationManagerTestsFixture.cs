using System;

namespace SettingsManagement.Tests
{
    class DefaultConfigurationManagerTestsFixture : IDisposable
    {
        public DefaultConfigurationManagerTestsFixture()
        {
            DefaultSettings.Manager = new DefaultConfigurationManager();
        }

        public void Dispose()
        {
            DefaultSettings.Manager?.Close();
        }
    }
}
