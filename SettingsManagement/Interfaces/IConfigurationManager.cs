namespace SettingsManagement.Interfaces
{
    public interface IConfigurationManager
    {
        /// <summary>
        /// Refreshes all settings from configuration source.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Persists all settings to configuration source.
        /// </summary>
        void Persist();

        /// <summary>
        /// Gets the value for a certain key as a string.
        /// </summary>
        /// <param name="key">The unique value key</param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// Sets the value for a certain key as a string.
        /// </summary>
        /// <param name="key">The unique value key</param>
        /// <param name="value">The value as string</param>
        void Set(string key, string value);
    }

    public interface IConfigurationManager<T> : IConfigurationManager
    {
        /// <summary>
        /// Gets the value for a certain key typed as <typeparamref name="T"/>.
        /// </summary>
        /// <param name="key">The unique value key</param>
        new T Get(string key);

        /// <summary>
        /// Sets the value for a certain key typed as <typeparamref name="T"/>.
        /// </summary>
        /// <param name="key">The unique value key</param>
        /// <param name="value">The value typed as <typeparamref name="T"/></param>
        void Set(string key, T value);
    }
}
