using System;

namespace SettingsManagement.Tests.Models
{
    class TimeSpanConverter : IValueConverter<TimeSpan>
    {
        public TimeSpan Convert(string value)
        {
            if (TimeSpan.TryParse(value, out var result))
                return result;

            return TimeSpan.Zero;
        }

        public string ConvertBack(TimeSpan value)
        {
            return value.ToString();
        }
    }
}
