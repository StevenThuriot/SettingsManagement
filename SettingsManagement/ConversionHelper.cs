using System;

namespace SettingsManagement
{
    static class ConversionHelper<T>
    {
        public static T Convert(string value)
        {
            return (T)System.Convert.ChangeType(value, typeof(T));
        }
    }
}
