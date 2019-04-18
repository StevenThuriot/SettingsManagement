using SettingsManagement.Formatters;
using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;

//This class represents the generated class at runtime

namespace SettingsManagement.Tests.Models
{
    public class MySettings : IMySettings
    {
        private readonly IConfigurationManager _configurationManager;
        public MySettings(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;

            _MyFirstProperty = SettingsBuilder<long>.Create(nameof(MyFirstProperty), 1345L, null, configurationManager);
            _MyFirstProperty.Description = "This is a description";

            _MySecondProperty = SettingsBuilder<bool>.Create(nameof(MySecondProperty), default(bool), null, configurationManager);

            _MyThirdProperty = SettingsBuilder<string>.Create(nameof(MyThirdProperty), "Test", null, configurationManager);
            _MyThirdProperty.Description = "This is another description";

            _MyFourthProperty = SettingsBuilder<TimeSpan>.ParseAndCreate(nameof(MyFourthProperty), "02:00", typeof(TimeSpanConverter), configurationManager);

            _MyFifthProperty = SettingsBuilder<bool>.ParseAndCreate(nameof(MyFifthProperty), "Ja", typeof(JaNeeConverter), configurationManager);
        }

        readonly Setting<long> _MyFirstProperty;
        public long MyFirstProperty
        {
            get => _MyFirstProperty.Value;
            set => _MyFirstProperty.Value = value;
        }

        readonly Setting<bool> _MySecondProperty;
        public bool MySecondProperty
        {
            get => _MySecondProperty.Value;
            set => _MySecondProperty.Value = value;
        }

        readonly Setting<string> _MyThirdProperty;
        public string MyThirdProperty
        {
            get => _MyThirdProperty.Value;
            set => _MyThirdProperty.Value = value;
        }

        readonly Setting<TimeSpan> _MyFourthProperty;
        public TimeSpan MyFourthProperty
        {
            get => _MyFourthProperty.Value;
            set => _MyFourthProperty.Value = value;
        }

        readonly Setting<bool> _MyFifthProperty;
        public bool MyFifthProperty
        {
            get => _MyFifthProperty.Value;
            set => _MyFifthProperty.Value = value;
        }

        public void Refresh()
        {
            _configurationManager.Refresh();

            _MyFirstProperty.Refresh();
            _MySecondProperty.Refresh();
            _MyThirdProperty.Refresh();
            _MyFourthProperty.Refresh();
            _MyFifthProperty.Refresh();
        }

        public void Refresh(string key)
        {
            _configurationManager.Refresh();

            switch (key)
            {
                case "MyFirstProperty":
                    _MyFirstProperty.Refresh();
                    break;

                case "MySecondProperty":
                    _MySecondProperty.Refresh();
                    break;

                case "MyThirdProperty":
                    _MyThirdProperty.Refresh();
                    break;

                case "MyFourthProperty":
                    _MyFourthProperty.Refresh();
                    break;

                case "MyFifthProperty":
                    _MyFifthProperty.Refresh();
                    break;

                default:
                    throw new KeyNotFoundException(key);
            }
        }

        public void Persist()
        {
            _MyFirstProperty.Persist();
            _MySecondProperty.Persist();
            _MyThirdProperty.Persist();
            _MyFourthProperty.Persist();
            _MyFifthProperty.Persist();

            _configurationManager.Persist();
        }

        public IEnumerable<string> GetReadableValues()
        {
            return new string[]
            {
                _MyFirstProperty.GetReadableValue(),
                _MySecondProperty.GetReadableValue(),
                _MyThirdProperty.GetReadableValue(),
                _MyFourthProperty.GetReadableValue(),
                _MyFifthProperty.GetReadableValue()
            };
        }

        public override string ToString()
        {
            return "IMySettingsManager { " + string.Join(", ", GetReadableValues()) + " }";
        }

        public void Dispose()
        {
            _MyFirstProperty.Persist();
            _MySecondProperty.Persist();
            _MyThirdProperty.Persist();
            _MyFourthProperty.Persist();
            _MyFifthProperty.Persist();

            _configurationManager.Persist();
        }

        public void Reset()
        {
            _MyFirstProperty.Reset();
            _MySecondProperty.Reset();
            _MyThirdProperty.Reset();
            _MyFourthProperty.Reset();
            _MyFifthProperty.Reset();
        }

        public void Reset(string key)
        {
            switch (key)
            {
                case "MyFirstProperty":
                    _MyFirstProperty.Value = _MyFirstProperty.DefaultValue;
                    break;

                case "MySecondProperty":
                    _MySecondProperty.Reset();
                    break;

                case "MyThirdProperty":
                    _MyThirdProperty.Reset();
                    break;

                case "MyFourthProperty":
                    _MyFourthProperty.Reset();
                    break;

                case "MyFifthProperty":
                    _MyFifthProperty.Reset();
                    break;

                default:
                    throw new KeyNotFoundException(key);
            }
        }

        public IReadOnlyDictionary<string, string> GetDescriptions()
        {
            return new Dictionary<string, string>
            {
                ["MyFirstProperty"] = _MyFirstProperty.Description,
                ["MySecondProperty"] = _MySecondProperty.Description,
                ["MyThirdProperty"] = _MyThirdProperty.Description,
                ["MyFourthProperty"] = _MyFourthProperty.Description,
                ["MyFifthProperty"] = _MyFifthProperty.Description,
            };
        }

        public string GetDescription(string key)
        {
            switch (key)
            {
                case "MyFirstProperty":
                    return _MyFirstProperty.Description;

                case "MySecondProperty":
                    return _MySecondProperty.Description;

                case "MyThirdProperty":
                    return _MyThirdProperty.Description;

                case "MyFourthProperty":
                    return _MyFourthProperty.Description;

                case "MyFifthProperty":
                    return _MyFifthProperty.Description;

                default:
                    throw new KeyNotFoundException(key);
            }
        }

        public string Serialize()
        {
            var settings = new ISetting[] { _MyFirstProperty, _MySecondProperty, _MyThirdProperty, _MyFourthProperty, _MyFifthProperty };
            ISettingsSerializer formatter = new JsonArraySerializer();
            return formatter.Serialize(settings);
        }
    }
}
