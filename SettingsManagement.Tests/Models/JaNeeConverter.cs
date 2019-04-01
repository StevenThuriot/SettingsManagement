namespace SettingsManagement.Tests.Models
{
    class JaNeeConverter : IConverter<bool>
    {
        public bool Convert(string value)
        {
            return value?.ToUpperInvariant() == "JA";
        }
    }
}
