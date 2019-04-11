using SettingsManagement.Tests.Models;
using Xunit;

namespace SettingsManagement.Tests
{
    public class RefreshTests
    {
        [Fact]
        public void CanRefresh()
        {
            var settings = SettingsManager.New<IMySettings>();

            settings.Refresh();
        }

        [Fact]
        public void CanPersist()
        {
            var settings = SettingsManager.New<IMySettings>();

            var original = settings.MyFirstProperty;

            settings.MyFirstProperty++;

            settings.Persist();

            var settings2 = SettingsManager.New<IMySettings>();

            Assert.NotEqual(original, settings2.MyFirstProperty);
            Assert.Equal(settings.MyFirstProperty, settings2.MyFirstProperty);
        }

        [Fact]
        public void CanDispose()
        {
            using (var settings = SettingsManager.New<IMySettings>())
            {
                settings.ToString();
            }
        }

        [Fact]
        public void HasReadableValues()
        {
            var settings = SettingsManager.New<IMySettings>();
            var values = settings.GetReadableValues();

            Assert.NotNull(values);
            Assert.NotEmpty(values);
        }

        [Fact]
        public void HasToString()
        {
            var settings = SettingsManager.New<IMySettings>();
            var values = settings.ToString();

            Assert.NotNull(values);
            Assert.NotEmpty(values);

            var comparison = "IMySettingsManager { " + string.Join(", ", settings.GetReadableValues()) + " }";
            Assert.Equal(comparison, settings.ToString());
        }
    }
}
