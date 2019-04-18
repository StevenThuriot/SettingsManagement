namespace SettingsManagement.Interfaces
{
    /// <summary>
    /// I Can Show My Values in a readable format, Refresh from Configuration Source and Persist to Configuration Source.
    /// Implements the <see cref="ICanRefresh"/>, <see cref="ICanReset"/>, <see cref="ICanPersist"/>, <see cref="ICanShowMyValues"/> , <see cref="IAmDescriptive"/> and <see cref="ICanSerialize"/> interfaces.
    /// </summary>
    public interface ISettingsManager :
        ICanRefresh
        , ICanReset
        , ICanPersist
        , ICanShowMyValues
        , IAmDescriptive
        , ICanSerialize
    {
    }
}