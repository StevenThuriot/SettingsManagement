using SettingsManagement.Interfaces;

namespace SettingsManagement.Tests.Models
{
    class JaNeeConverter : IValueConverter<bool>
    {
        public bool Convert(string value)
        {
            return value?.ToUpperInvariant() == "JA";
        }

        public string ConvertBack(bool value)
        {
            return value ? "Ja" : "Nee";
        }
    }
}
