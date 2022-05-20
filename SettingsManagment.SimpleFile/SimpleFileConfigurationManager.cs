using SettingsManagement;
using SettingsManagement.Interfaces;
using System.Reflection;

namespace SettingsManagment.SimpleFile;

/// <summary>
/// Default Configuration Manager Implementation, wrapping around a simple file on disk.
/// </summary>
public class SimpleFileConfigurationManager : IConfigurationManager
{
    private readonly Dictionary<string, string> _settings = new();
    private readonly string _path;

    /// <summary>
    /// Creates a new manager using assembly entry name as path
    /// </summary>
    public SimpleFileConfigurationManager()
    {
        var name = Assembly.GetEntryAssembly()?.GetName().Name ?? "app";
        _path = name + ".settings";
    }

    /// <summary>
    /// Creates a new manager
    /// </summary>
    /// <param name="path"></param>
    public SimpleFileConfigurationManager(string path)
    {
        _path = path;
    }

    /// <summary>
    /// Closes the ConfigurationManager
    /// </summary>
    public void Close()
    {
        _settings.Clear();
    }

    /// <summary>
    /// Checks if a certain key exists.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <returns>If true the value has been found</returns>
    public bool Contains(string key)
    {
        return _settings.ContainsKey(key);
    }

    /// <summary>
    /// Gets the value for a certain key as a string.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <returns></returns>
    public string Get(string key)
    {
        return _settings[key];
    }

    /// <summary>
    /// Opens the Configuration Manager Instance.
    /// </summary>
    public void Open()
    {
        if (!File.Exists(_path))
        {
            return;
        }

        using var file = File.OpenText(_path);

        string? line;
        while ((line = file.ReadLine()) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                var parts = line.Split(new[] { '=' }, 2);
                _settings[parts[0]] = parts.Length == 2 ? parts[1] : "";
            }
        }
    }

    /// <summary>
    /// Persists all settings to configuration source.
    /// </summary>
    public void Persist()
    {
        using var file = File.CreateText(_path);

        var values = from item in _settings
                     select $"{item.Key}={item.Value}";

        file.Write(string.Join(Environment.NewLine, values));
    }

    /// <summary>
    /// Refreshes all settings from configuration source.
    /// </summary>
    public void Refresh()
    {
        //Close();
        Open();
    }

    /// <summary>
    /// Sets the value for a certain key as a string.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <param name="value">The value as string</param>
    public void Set(string key, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            _settings.Remove(key);
        }
        else
        {
            _settings[key] = value;
        }
    }

    /// <summary>
    /// Gets the value for a certain key as a string.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <param name="value">The value belonging to the key</param>
    /// <returns>True the value has been found</returns>
    public bool TryGet(string key, out string value)
    {
        return _settings.TryGetValue(key, out value!);
    }

    /// <summary>
    /// Sets this manager type as the default manager for the current context.
    /// </summary>
    public static void ConfigureAsDefault()
    {
        SettingsContext.AppContext.Manager = new SimpleFileConfigurationManager();
    }

    /// <summary>
    /// Sets this manager type as the default manager for the current context.
    /// </summary>
    public static void ConfigureAsDefault(string path)
    {
        SettingsContext.AppContext.Manager = new SimpleFileConfigurationManager(path);
    }
}
