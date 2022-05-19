using SettingsManagement.Tests.Models;
using Xunit;

namespace SettingsManagement.Tests;

public class InterfaceImplementationTests
{
    [Fact]
    public void CanRefresh()
    {
        var settings = SettingsManager.New<IMySettings>();
        settings.Refresh();
    }

    [Fact]
    public void CanRefreshWithKey()
    {
        var settings = SettingsManager.New<IMySettings>();
        settings.Refresh(nameof(IMySettings.MyFirstProperty));
    }

    [Fact]
    public void CanReset()
    {
        var settings = SettingsManager.New<IMySettings>();

        Assert.Equal("Test", settings.MyThirdProperty);
        Assert.True(settings.MyFifthProperty);

        settings.MyThirdProperty = "Blub";
        Assert.Equal("Blub", settings.MyThirdProperty);

        settings.MyFifthProperty = false;
        Assert.False(settings.MyFifthProperty);

        settings.Reset();
        Assert.Equal("Test", settings.MyThirdProperty);
        Assert.True(settings.MyFifthProperty);
    }

    [Fact]
    public void CanResetWithKey()
    {
        var settings = SettingsManager.New<IMySettings>();

        Assert.Equal("Test", settings.MyThirdProperty);
        Assert.True(settings.MyFifthProperty);
        settings.MyThirdProperty = "Blub";
        Assert.Equal("Blub", settings.MyThirdProperty);
        Assert.True(settings.MyFifthProperty);
        settings.Reset("MyThirdProperty");
        Assert.Equal("Test", settings.MyThirdProperty);
        Assert.True(settings.MyFifthProperty);
    }

    [Fact]
    public void HasDescriptions()
    {
        var settings = SettingsManager.New<IMySettings>();
        var descriptions = settings.GetDescriptions();

        Assert.NotNull(descriptions);
        Assert.NotEmpty(descriptions);
        Assert.True(descriptions.ContainsKey(nameof(IMySettings.MyFirstProperty)));
        Assert.Equal("This is a description", descriptions[nameof(IMySettings.MyFirstProperty)]);
    }

    [Fact]
    public void CanGetSpecificDescriptions()
    {
        var settings = SettingsManager.New<IMySettings>();
        var description = settings.GetDescription(nameof(IMySettings.MyFirstProperty));

        Assert.NotNull(description);
        Assert.Equal("This is a description", description);
    }

    [Fact]
    public void CanPersist()
    {
        var settings = SettingsManager.New<IMySettings>(new DefaultConfigurationManager());

        var original = settings.MyFirstProperty;

        settings.MyFirstProperty++;

        settings.Persist();

        var settings2 = SettingsManager.New<IMySettings>(new DefaultConfigurationManager());

        Assert.NotEqual(original, settings2.MyFirstProperty);
        Assert.Equal(settings.MyFirstProperty, settings2.MyFirstProperty);
    }

    [Fact]
    public void CanDispose()
    {
        using var settings = SettingsManager.New<IMySettings>();
        settings.ToString();
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

    [Fact]
    public void CanSerialize()
    {
        var settings = SettingsManager.New<IMySettings>();
        var serialized = settings.Serialize();

        Assert.NotNull(serialized);
        Assert.NotEmpty(serialized);
        Assert.Equal(TestableSerializer.LastResult, serialized);
    }

    [Fact]
    public void EmptyInterfacesShouldntCrash() //Even if they're not useful
    {
        var settings = SettingsManager.New<IAmEmpty>();
        Assert.NotNull(settings);
    }
}
