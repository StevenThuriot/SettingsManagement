using SettingsManagement.Tests.Models;

namespace SettingsManagement.Tests;

public class DefensiveTests
{
    [Fact]
    public void ConvertersHaveToBeIValueConverters()
    {
        var ex = Assert.Throws<ArgumentException>(() => SettingsManager.New<IBrokenSettingsBecauseConverter>());

        Assert.NotNull(ex);
        Assert.NotNull(ex.StackTrace);
        Assert.NotEmpty(ex.StackTrace);
        Assert.Contains("Convert type needs to implement", ex.Message);
    }

    [Fact]
    public void TypesHaveToBeParsableOrHaveAParser()
    {
        var ex = Assert.Throws<NotSupportedException>(() => SettingsManager.New<IBrokenSettingsBecauseDefaultValue>());

        Assert.NotNull(ex);
        Assert.NotNull(ex.StackTrace);
        Assert.NotEmpty(ex.StackTrace);
        Assert.Contains("No parse found for type", ex.Message);
    }

    [Fact]
    public void TypesHaveToBeAccesible()
    {
        var ex = Assert.Throws<TypeLoadException>(() => SettingsManager.New<IBrokenSettingsBecauseInternal>());

        Assert.NotNull(ex);
        Assert.NotNull(ex.StackTrace);
        Assert.NotEmpty(ex.StackTrace);
        Assert.Contains("inaccessible", ex.Message);
    }
}
