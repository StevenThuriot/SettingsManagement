using SettingsManagement.Tests.Models;

namespace SettingsManagement.Tests;

public class SettingTests
{
    static readonly Random _random = new();
    static long GetRandomLong()
    {
        var buffer = new byte[8];
        _random.NextBytes(buffer);

        return BitConverter.ToInt64(buffer, 0);
    }

    [Fact]
    public void AnEntityHasFieldsThatCanBeSet()
    {
        var settings = SettingsManager.New<IMySettings>();

        settings.MyFirstProperty = 1;
        Assert.Equal(1, settings.MyFirstProperty);
    }

    [Fact]
    public void SettingsAreNotRememberedWhenBuildingANewManager()
    {
        var settings = SettingsManager.New<IMySettings>();

        var value = GetRandomLong();
        Assert.NotEqual(value, settings.MyFirstProperty);

        settings.MyFirstProperty = value;
        Assert.Equal(value, settings.MyFirstProperty);

        var settings2 = SettingsManager.New<IMySettings>();
        Assert.NotEqual(value, settings2.MyFirstProperty);
    }

    [Fact]
    public void SettingsAreRememberedBetweenGets()
    {
        using var scope = SettingsContext.BeginScope();
        var settings = scope.Get<IMySettings>();

        var value = GetRandomLong();
        Assert.NotEqual(value, settings.MyFirstProperty);

        settings.MyFirstProperty = value;
        Assert.Equal(value, settings.MyFirstProperty);

        var settings2 = scope.Get<IMySettings>();
        Assert.Equal(value, settings2.MyFirstProperty);
    }

    [Fact]
    public void SettingsAreRememberedBetweenGetsBasedOnKey()
    {
        using var scope = SettingsContext.BeginScope();
        var settings = scope.Get<IMySettings>("TEST1");

        var value = GetRandomLong();
        Assert.NotEqual(value, settings.MyFirstProperty);

        settings.MyFirstProperty = value;
        Assert.Equal(value, settings.MyFirstProperty);

        var settings2 = scope.Get<IMySettings>("TEST1");
        Assert.Equal(value, settings2.MyFirstProperty);

        var settings3 = scope.Get<IMySettings>("TEST2");
        Assert.NotEqual(value, settings3.MyFirstProperty);

        var settings4 = scope.Get<IMySettings>();
        Assert.NotEqual(value, settings4.MyFirstProperty);
    }
}
