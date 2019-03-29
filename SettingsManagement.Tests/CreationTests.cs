using SettingsManagement.Tests.Models;
using Xunit;

namespace SettingsManagement.Tests
{
    public class CreationTests
    {
        [Fact]
        public void AnEntityIsCreated()
        {
            var settings = SettingsManager.Get<IMySettings>();

            Assert.NotNull(settings);
        }

        [Fact]
        public void AnEntityHasFields()
        {
            var settings = SettingsManager.Get<IMySettings>();
            var value = settings.MyFirstProperty;
        }

        [Fact]
        public void AnEntityHasFieldsThatCanBeSet()
        {
            var settings = SettingsManager.Get<IMySettings>();

            settings.MyFirstProperty = 1;
            Assert.Equal(1, settings.MyFirstProperty);
        }

        [Fact]
        public void FieldsHaveDefaultValues()
        {
            var settings = SettingsManager.Get<IMySettings>();

            Assert.Equal(5, settings.MyFirstProperty);
        }
    }
}
