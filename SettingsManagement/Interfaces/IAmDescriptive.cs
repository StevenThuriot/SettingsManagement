namespace SettingsManagement.Interfaces;

/// <summary>
/// I Can Show Descriptions
/// </summary>
public interface IAmDescriptive
{
    /// <summary>
    /// Gets an overview of all descriptions
    /// </summary>
    IReadOnlyDictionary<string, string> GetDescriptions();

    /// <summary>
    /// Returns a setting description
    /// </summary>
    /// <param name="key">The value's unique key</param>
    string GetDescription(string key);
}
