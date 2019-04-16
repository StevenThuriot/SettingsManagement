﻿namespace SettingsManagement.Interfaces
{
    /// <summary>
    /// Configuration Manager Contract
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Opens the ConfigurationManager
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the ConfigurationManager
        /// </summary>
        void Close();

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
}
