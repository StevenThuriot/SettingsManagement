using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SettingsManagement.Tests.Models
{
    public class MySettings : IMySettings//, ISettingsManager, IDisposable
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
        }

        public void Persist()
        {
            _MyFirstProperty.Persist();
            _MySecondProperty.Persist();
            _MyThirdProperty.Persist();
            _MyFourthProperty.Persist();

            _configurationManager.Persist();
        }

        public IEnumerable<string> GetReadableValues()
        {
            return new string[]
            {
                _MyFirstProperty.GetReadableValue(),
                _MySecondProperty.GetReadableValue(),
                _MyThirdProperty.GetReadableValue(),
                _MyFourthProperty.GetReadableValue()
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

            _configurationManager.Persist();
        }
    }
}
