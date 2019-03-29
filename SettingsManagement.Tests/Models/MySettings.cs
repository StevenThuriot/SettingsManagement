using System;

namespace SettingsManagement.Tests.Models
{
    public class MySettings : IMySettings, ISettingsManager
    {
        public MySettings()
        {
            //converter == TypeConvertAttribute 
            //If not present --> T Parse(string) method?
            //Else null

            _MyFirstProperty = SettingsBuilder<int>.Create(nameof(MyFirstProperty), 5, int.Parse);
            _MyFirstProperty.Description = "This is a description";

            _MySecondProperty = SettingsBuilder<bool>.Create(nameof(MySecondProperty), default(bool), null);

            _MyThirdProperty = SettingsBuilder<string>.Create(nameof(MyThirdProperty), "Test", null);
            _MyThirdProperty.Description = "This is another description";

            _MyFourthProperty = SettingsBuilder<TimeSpan>.ParseAndCreate(nameof(MyFourthProperty), "02:00", TimeSpan.Parse);
        }

        readonly Setting<int> _MyFirstProperty;
        public int MyFirstProperty
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


        public void Refresh()
        {
            ConfigurationHelper.RefreshAppSettings();

            _MyFirstProperty.Refresh();
            _MySecondProperty.Refresh();
            _MyThirdProperty.Refresh();
        }

        public void Persist()
        {
            var configuration = ConfigurationHelper.OpenConfiguration();

            _MyFirstProperty.Persist(configuration);
            _MySecondProperty.Persist(configuration);
            _MyThirdProperty.Persist(configuration);

            ConfigurationHelper.Persist(configuration);
        }
    }
}
