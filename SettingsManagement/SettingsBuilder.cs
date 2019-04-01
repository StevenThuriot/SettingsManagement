using System;

namespace SettingsManagement
{
    static class SettingsBuilder<T>
    {
        public static Setting<T> Create(string key, T defaultValue, Type converterType)
        {
            var converter = ConversionHelper<T>.Resolve(converterType);

            return new Setting<T>(key, defaultValue, converter);
        }

        public static Setting<T> ParseAndCreate(string key, string defaultValue, Type converterType)
        {
            var converter = ConversionHelper<T>.Resolve(converterType);
            var convertedDefaultValue = converter(defaultValue);

            return new Setting<T>(key, convertedDefaultValue, converter);
        }
    }
}
