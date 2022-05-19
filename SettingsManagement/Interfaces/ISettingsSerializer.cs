namespace SettingsManagement.Interfaces;

/// <summary>
/// A Formatter Implementation
/// </summary>
public interface ISettingsSerializer
{
    /// <summary>
    /// Format settings to a string
    /// </summary>
    /// <param name="settings">The available settings</param>
    /// <returns></returns>
    string Serialize(IReadOnlyList<ISetting> settings);
}
