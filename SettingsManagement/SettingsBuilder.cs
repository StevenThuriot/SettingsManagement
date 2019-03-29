using System;

namespace SettingsManagement
{
    static class SettingsBuilder<T>
    {
        public static Setting<T> Init(string key)
        {
            return new Setting<T>(key, default(T), ConversionHelper<T>.Convert);
        }

        public static Setting<T> Create(string key, T defaultValue, Func<string, T> converter)
        {
            if (converter == null)
                converter = ConversionHelper<T>.Convert;

            return new Setting<T>(key, defaultValue, converter);
        }

        public static Setting<T> ParseAndCreate(string key, string defaultValue, Func<string, T> converter)
        {
            if (converter == null)
                converter = ConversionHelper<T>.Convert;

            var convertedDefaultValue = converter(defaultValue);
            return new Setting<T>(key, convertedDefaultValue, converter);
        }
    }
}
