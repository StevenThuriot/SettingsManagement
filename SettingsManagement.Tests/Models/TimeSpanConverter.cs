using System;

namespace SettingsManagement.Tests.Models
{
    class TimeSpanConverter : IConverter<TimeSpan>
    {
        public TimeSpan Convert(string value)
        {
            return TimeSpan.Parse(value);
        }
    }
}
