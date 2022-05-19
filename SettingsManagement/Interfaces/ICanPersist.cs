namespace SettingsManagement.Interfaces;

/// <summary>
/// I Can Persist to Configuration Source
/// </summary>
public interface ICanPersist
{
    /// <summary>
    /// Persists values to configuration source.
    /// </summary>
    void Persist();
}
