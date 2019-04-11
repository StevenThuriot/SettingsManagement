namespace SettingsManagement.Interfaces
{
    /// <summary>
    /// I Can Show My Values in a readable format, Refresh from Configuration Source and Persist to Configuration Source.
    /// Implements the <see cref="ICanShowMyValues"/>, <see cref="ICanRefresh"/> and <see cref="ICanPersist"/> interfaces.
    /// </summary>
    public interface ISettingsManager : ICanPersist, ICanRefresh, ICanShowMyValues
    {
    }
}