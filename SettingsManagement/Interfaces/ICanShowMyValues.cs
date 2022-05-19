namespace SettingsManagement.Interfaces;

/// <summary>
/// I Can Show My Values in a readable format
/// </summary>
public interface ICanShowMyValues
{
    /// <summary>
    /// Returns a list of Manager values in a readable format.
    /// </summary>
    IEnumerable<string> GetReadableValues();
}
