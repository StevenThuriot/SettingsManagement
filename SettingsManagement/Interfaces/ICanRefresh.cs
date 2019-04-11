namespace SettingsManagement.Interfaces
{
    /// <summary>
    /// I Can Refresh from Configuration Source
    /// </summary>
    public interface ICanRefresh
    {
        /// <summary>
        /// Refreshes values from configuration source.
        /// </summary>
        void Refresh();
    }
}