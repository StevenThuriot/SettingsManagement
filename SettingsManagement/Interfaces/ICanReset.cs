namespace SettingsManagement.Interfaces;

/// <summary>
/// I Can Reset from Configuration Source
/// </summary>
public interface ICanReset
{
    /// <summary>
    /// Resets values.
    /// </summary>
    void Reset();

    /// <summary>
    /// Resets the value for one single key.
    /// </summary>
    void Reset(string key);
}
