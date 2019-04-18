using SettingsManagement.Formatters;
using SettingsManagement.Interfaces;
using System.Collections.Generic;

namespace SettingsManagement.Tests
{
    public class TestableSerializer : ISettingsSerializer
    {
        public static string LastResult;

        public string Serialize(IReadOnlyList<ISetting> settings)
        {
            return LastResult = new XmlArraySerializer().Serialize(settings);
        }
    }
}
