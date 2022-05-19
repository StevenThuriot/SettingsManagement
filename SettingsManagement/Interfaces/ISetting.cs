namespace SettingsManagement.Interfaces;

/// <summary>
/// SettingsManager's Setting
/// </summary>
public interface ISetting
{
    /// <summary>
    /// The Setting Key
    /// </summary>
    string Key { get; }

    /// <summary>
    /// The Setting Description
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The current Setting Value
    /// </summary>
    /// <returns></returns>
    object ResolveValue();
}

interface ISettingExtended : ISetting
{
    new string Description { get; set; }
    string GetReadableValue();
    void Persist();
    void Refresh();
    void Reset();
    void Refresh(bool overwriteChanges);
}
