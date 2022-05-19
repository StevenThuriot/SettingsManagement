[![Build status](https://ci.appveyor.com/api/projects/status/a4803q20bl472th7/branch/master?svg=true)](https://ci.appveyor.com/project/StevenThuriot/settingsmanagement/branch/master)

# What can it do?!

Currently supported are:
- Custom Configuration Managers
  - Allows wrapping around 
    - app.config
    - or if you'd so desire, even a SQL Server!
- Custom Serializers
  - Json
  - XML
  - ... 
    - As instances
    - Or arrays
  - Anything you want!
- Default value through attributes if not supplied by the Configuration Manager
- Custom Converters per property (e.g. for TimeSpan)
  - In the SettingsManagment.Secure you'll find a few converters using the DataProtection API to secure your values when persisting
- Context scopes and children of (think things like DI Containers)
  - With extension methods to automatically set the scopes over threads as well!

# Usage Sample 
This module will build a SettingsManager for you based on an interface (or coming later, a base class).

Given this interface: 

```csharp
using SettingsManagement.Attributes;
using SettingsManagement.Interfaces;

using System;
using System.ComponentModel;

[SettingsSerializer(typeof(CustomSerializer))]
public interface IMySettings :
    ICanRefresh
    , ICanReset
    , ICanPersist
    , ICanShowMyValues
    , IAmDescriptive
    , ICanSerialize
    , IDisposable
{
    [DefaultValue(5L), Description("This is a description")]
    long MyFirstProperty { get; set; }

    bool MySecondProperty { get; set; }

    [DefaultValue("Test"), Description("This is another description")]
    string MyThirdProperty { get; set; }

    [DefaultValue("00:20"), SettingsConverter(typeof(TimeSpanConverter))]
    TimeSpan MyFourthProperty { get; set; }

    [DefaultValue("Ja"), SettingsConverter(typeof(JaNeeConverter))]
    bool MyFifthProperty { get; set; }
}
```

You can easily get your manager up and running using this simple line of code:

```csharp
var settings = SettingsManager.New<IMySettings>();
```

## Scopes

### AppContext scope

A default App-wide scope is provided and ever present.
You can access it like this:

```csharp
var settings = SettingsContext.AppContext.Get<IMySettings>();
```

All scope inheritly have this scope as their ultimate parent.

_( Or in other words, this is the only scope that doesn't have a parent. )_

### Simple scope

```csharp  
using (var scope = SettingsContext.BeginScope())
{
    var settings = scope.Get<IMySettings>();
}
```

After disposal, the managers created on that scope get removed.

Scopes will always try to resolve instances from their parents, too.

### Nesting Scopes

```csharp
using (var scope1 = SettingsContext.BeginScope())
{
    using (var scope2 = scope1.BeginChildScope())
    {
        using (var scope3 = scope2.BeginChildScope())
        {
            var settings = scope1.Get<IMySettings>();
            Assert.NotNull(settings);

            Assert.True(scope2.HasManager<IMySettings>(), "Scopes should have instances from their parent scopes");
            settings = scope2.Get<IMySettings>();
            Assert.NotNull(settings);

            Assert.True(scope3.HasManager<IMySettings>(), "Scopes should have instances from their parent scopes");
            settings = scope3.Get<IMySettings>();
            Assert.NotNull(settings);
        }

        Assert.True(scope2.HasManager<IMySettings>(), "Child scopes should not influence instances from their parent scopes");
    }

    Assert.True(scope1.HasManager<IMySettings>(), "Disposal of other scopes shouldn't influence the current scopes");
}
```


# Supported Interfaces

A few interfaces are supplied that will give the built SettingsManager extra functionality!

## ICanRefresh

Can refresh the settings from values stored in the Configuration Manager.

```csharp
/// <summary>
/// Refreshes values from configuration source.
/// </summary>
void Refresh();

/// <summary>
/// Refreshes values from configuration source for one single key.
/// </summary>
/// <param name="key">The specific key to refresh</param>
void Refresh(string key);
```

## ICanReset

Can reset the settings to their defaults.

```csharp
/// <summary>
/// Resets values.
/// </summary>
void Reset();

/// <summary>
/// Resets the value for one single key.
/// </summary>
void Reset(string key);
```

## ICanShowMyValues

Can show its key-value pairs in a readable format.

```csharp
/// <summary>
/// Returns a list of Manager values in a readable format.
/// </summary>
IEnumerable<string> GetReadableValues();
```

## IAmDescriptive

Can show the descriptions for each property.

```csharp
/// <summary>
/// Gets an overview of all descriptions
/// </summary>
IReadOnlyDictionary<string, string> GetDescriptions();

/// <summary>
/// Returns a setting description
/// </summary>
/// <param name="key">The value's unique key</param>
string GetDescription(string key);
```

## ICanSerialize

Can serialize its key-value pairs using a custom serializer

```csharp
/// <summary>
/// Converts the current settings manager to the selected format
/// </summary>
string Serialize();
```

## ICanPersist

Can persist its values back to the Configuration Manager.

```csharp
/// <summary>
/// Persists values to configuration source.
/// </summary>
void Persist();
```

## IDisposable

This will trigger the `Persist` method when the manager is disposed.

## Configuration Manager
### Default 
The default manager used is the `InMemoryManager`. This is an In-Memory implementation of the configuration manager.


Obviously, it's not able to persist values between sessions.

### Extras

In the `SettingsManagment.System.Configuration` package, you can find a few other implementations:

- DefaultConfigurationManager
  - Wraps around the `ConfigurationManager` class.
- SimpleFileConfigurationManager
  - Wraps around a simple on disk file. Each line is a setting

### Or implement your own!

It's as easy as implementing the `IConfigurationManager` interface!

```csharp
/// <summary>
/// Opens the ConfigurationManager
/// </summary>
void Open();

/// <summary>
/// Closes the ConfigurationManager
/// </summary>
void Close();

/// <summary>
/// Refreshes all settings from configuration source.
/// </summary>
void Refresh();

/// <summary>
/// Persists all settings to configuration source.
/// </summary>
void Persist();

/// <summary>
/// Checks if a certain key exists.
/// </summary>
/// <param name="key">The unique value key</param>
/// <returns>If true the value has been found</returns>
bool Contains(string key);

/// <summary>
/// Gets the value for a certain key as a string.
/// </summary>
/// <param name="key">The unique value key</param>
/// <returns></returns>
string Get(string key);

/// <summary>
/// Gets the value for a certain key as a string.
/// </summary>
/// <param name="key">The unique value key</param>
/// <param name="value">The value belonging to the key</param>
/// <returns>True the value has been found</returns>
bool TryGet(string key, out string value);

/// <summary>
/// Sets the value for a certain key as a string.
/// </summary>
/// <param name="key">The unique value key</param>
/// <param name="value">The value as string</param>
void Set(string key, string value);
```

# Get Packages

You can get SettingsManagement by grabbing the latest [NuGet packages](https://www.nuget.org/packages/SettingsManagement/).
If you're feeling adventurous, [continuous integration builds](https://ci.appveyor.com/project/StevenThuriot/settingsmanagement) are also available!

```
dotnet add package SettingsManagement
```

Default ConfigurationManager implementations are also available as a separate package
```
dotnet add package SettingsManagment.System.Configuration
```
