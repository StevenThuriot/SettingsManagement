using System;
using System.Collections;
using System.Configuration;
using System.Linq;

namespace SettingsManagement
{
    sealed class Setting<T>
    {
        T _defaultValue;
        readonly Func<string, T> _converter;

        public T Value { get; set; }

        public Type Type { get; } = typeof(T);

        public string Key { get; }
        public string Description { get; set; }


        public Setting(string key, T defaultValue, Func<string, T> converter)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));

            Value = _defaultValue = defaultValue;

            Refresh(true);
        }

        public T ResetValue()
        {
            return Value = _defaultValue;
        }

        public void Refresh()
        {
            Refresh(true);
        }

        public void Refresh(bool overwriteChanges)
        {
            if (!overwriteChanges)
            {
                if (!Equals(_defaultValue, Value))
                {
                    return;
                }
            }

            if (ConfigurationManager.AppSettings.AllKeys.Any(n => n == Key))
            {
                Value = _defaultValue = _converter(ConfigurationManager.AppSettings.Get(Key));
            }
        }

        public void Persist(Configuration configuration)
        {
            Persist(configuration.AppSettings.Settings);
        }

        public void Persist(KeyValueConfigurationCollection settings)
        {
            string stringValue = Value?.ToString() ?? "";

            var configItem = settings[Key];
            if (configItem == null)
            {
                settings.Add(Key, stringValue);
            }
            else
            {
                configItem.Value = stringValue;
            }
        }

        public string GetReadableValue()
        {
            if (Value == null)
                return null;

            if (Value is string @string)
                return @string;

            if (Value is IEnumerable enumerable)
            {
                var @array = enumerable.Cast<object>().ToArray();
                return $"{enumerable.GetType().FullName}(Count = {@array.Length}) {{ {string.Join(", ", @array)} }}";
            }

            return Value.ToString();
        }
    }
}