using SettingsManagement.Interfaces;
using System;

namespace SettingsManagement
{
    static class SettingsBuilder<T>
    {
        //TODO: Receive ConfigurationManager instead of converter
        public static Setting<T> Create(string key, T defaultValue, Type converterType, IConfigurationManager configurationManager)
        {
            var converter = ConversionHelper<T>.Resolve(converterType);
            return new Setting<T>(key, defaultValue, converter, configurationManager);
        }

        public static Setting<T> ParseAndCreate(string key, string defaultValue, Type converterType, IConfigurationManager configurationManager)
        {
            var converter = ConversionHelper<T>.Resolve(converterType);
            var convertedDefaultValue = converter.Convert(defaultValue);
            return new Setting<T>(key, convertedDefaultValue, converter, configurationManager);
        }
    }
}
