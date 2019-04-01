using SettingsManagement.Tests.Models;
using System;
using Xunit;

namespace SettingsManagement.Tests
{
    public class CreationTests
    {
        [Fact]
        public void AnEntityIsCreated()
        {
            var settings = SettingsManager.New<IMySettings>();

            Assert.NotNull(settings);
        }

        [Fact]
        public void AnEntityHasFields()
        {
            var settings = SettingsManager.New<IMySettings>();
            var value = settings.MyFirstProperty;
        }

        [Fact]
        public void FieldsHaveDefaultValues()
        {
            var settings = SettingsManager.New<IMySettings>();

            Assert.Equal(5, settings.MyFirstProperty);
            Assert.False(settings.MySecondProperty);
            Assert.Equal("Test", settings.MyThirdProperty);
            Assert.Equal(TimeSpan.FromMinutes(20), settings.MyFourthProperty);
            Assert.True(settings.MyFifthProperty);
        }
    }
}
