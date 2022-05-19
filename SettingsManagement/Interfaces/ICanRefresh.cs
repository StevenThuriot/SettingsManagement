namespace SettingsManagement.Interfaces;

/// <summary>
/// I Can Refresh from Configuration Source
/// </summary>
public interface ICanRefresh
{
    /// <summary>
    /// Refreshes values from configuration source.
    /// </summary>
    void Refresh();

    /// <summary>
    /// Refreshes values from configuration source for one single key.
    /// </summary>
    /// <param name="key">The specific key to refresh</param>
    void Refresh(string key);
}
