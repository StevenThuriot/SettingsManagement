using SettingsManagement.Interfaces;

namespace SettingsManagement;

/// <summary>
/// An In-Memory implementation of the configuration manager
/// </summary>
public class InMemoryManager : IConfigurationManager
{
    readonly IDictionary<string, string> _dictionary = new Dictionary<string, string>();
    readonly bool _crashOnUnknownGets;

    /// <summary>
    /// Creates an instance of the In-Memory Manager.
    /// </summary>
    public InMemoryManager(bool crashOnUnknownGets = false)
    {
        _crashOnUnknownGets = crashOnUnknownGets;
    }

    /// <summary>
    /// Closes the ConfigurationManager
    /// </summary>
    public void Close() { }

    /// <summary>
    /// Checks if a certain key exists.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <returns>If true the value has been found</returns>
    public bool Contains(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// Gets the value for a certain key as a string.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <returns></returns>
    public string Get(string key)
    {
        if (_dictionary.TryGetValue(key, out var value))
            return value;

        if (_crashOnUnknownGets)
            throw new KeyNotFoundException(key + " was not found");

        return null;
    }

    /// <summary>
    /// Opens the ConfigurationManager
    /// </summary>
    public void Open() { }

    /// <summary>
    /// Persists all settings to configuration source.
    /// </summary>
    public void Persist()
    {
        //throw new NotSupportedException("In-Memory Persisting is not supported");
    }

    /// <summary>
    /// Refreshes all settings from configuration source.
    /// </summary>
    public void Refresh() { }

    /// <summary>
    /// Sets the value for a certain key as a string.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <param name="value">The value as string</param>
    public void Set(string key, string value)
    {
        _dictionary[key] = value;
    }

    /// <summary>
    /// Gets the value for a certain key as a string.
    /// </summary>
    /// <param name="key">The unique value key</param>
    /// <param name="value">The value belonging to the key</param>
    /// <returns>True the value has been found</returns>
    public bool TryGet(string key, out string value)
    {
        return _dictionary.TryGetValue(key, out value);
    }
}
