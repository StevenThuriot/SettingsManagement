using SettingsManagement.Interfaces;
using System;
using System.Collections;
using System.Linq;

namespace SettingsManagement
{
    sealed class Setting<T> : ISettingExtended
    {
        readonly IConfigurationManager _configurationManager;
        readonly IValueConverter<T> _converter;

        public T Value { get; set; }
        object ISetting.ResolveValue() => Value;

        public T DefaultValue { get; private set; }

        public Type Type { get; } = typeof(T);

        public string Key { get; }
        public string Description { get; set; }

        public Setting(string key, T defaultValue, IValueConverter<T> converter, IConfigurationManager configurationManager)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            DefaultValue = defaultValue;

            var stringValue = _configurationManager.Get(Key);
            if (string.IsNullOrEmpty(stringValue))
            {
                Value = DefaultValue;
            }
            else
            {
                Value = _converter.Convert(stringValue);

                if (DefaultValue is null)
                    DefaultValue = Value;
            }
        }

        public void Reset()
        {
            Value = DefaultValue;
        }

        public void Refresh()
        {
            Refresh(true);
        }

        public void Refresh(bool overwriteChanges)
        {
            if (!overwriteChanges)
            {
                if (!Equals(DefaultValue, Value))
                {
                    return;
                }
            }

            if (_configurationManager.TryGet(Key, out var stringValue))
            {
                var value = _converter.Convert(stringValue);
                Value = DefaultValue = value;
            }
        }

        public void Persist()
        {
            var value = _converter.ConvertBack(Value);
            _configurationManager.Set(Key, value);
        }

        public string GetReadableValue()
        {
            var value = Value;

            if (value == null)
                return $"{Key} = null";

            if (value is string @string)
                return $"{Key} = {@string}";

            if (value is IEnumerable enumerable)
            {
                var @array = enumerable.Cast<object>().ToArray();
                return $"{Key} = {enumerable.GetType().FullName} (Count = {@array.Length}) {{ {string.Join(", ", @array)} }}";
            }

            return $"{Key} = {value}";
        }
    }
}