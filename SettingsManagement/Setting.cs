using SettingsManagement.Interfaces;
using System;
using System.Collections;
using System.Linq;

namespace SettingsManagement
{
    sealed class Setting<T> : ISetting
    {
        T _defaultValue;
        readonly IConfigurationManager _configurationManager;
        private readonly IValueConverter<T> _converter;

        public T Value { get; set; }

        public Type Type { get; } = typeof(T);

        public string Key { get; }
        public string Description { get; set; } = "";


        public Setting(string key, T defaultValue, IValueConverter<T> converter, IConfigurationManager configurationManager)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _defaultValue = defaultValue;

            var stringValue = _configurationManager.Get(Key);
            if (string.IsNullOrEmpty(stringValue))
            {
                Value = _defaultValue;
            }
            else
            {
                Value = _converter.Convert(stringValue);

                if (_defaultValue is null)
                    _defaultValue = Value;
            }
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

            Value = _defaultValue = _converter.Convert(_configurationManager.Get(Key));
        }

        public void Persist()
        {
            _configurationManager.Set(Key, _converter.ConvertBack(Value));
        }

        public string GetReadableValue()
        {
            var value = Value;

            if (value == null)
                return "";

            if (value is string @string)
                return @string;

            if (value is IEnumerable enumerable)
            {
                var @array = enumerable.Cast<object>().ToArray();
                return $"{Key} = {enumerable.GetType().FullName} (Count = {@array.Length}) {{ {string.Join(", ", @array)} }}";
            }

            return $"{Key} = {value}";
        }
    }
}