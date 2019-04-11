namespace SettingsManagement.Interfaces
{
    /// <summary>
    /// Can convert values from string to a typed version and back.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public interface IValueConverter<T>
    {
        /// <summary>
        /// Converts the <paramref name="value"/> from string to <typeparam name="T">the converter type</typeparam>.
        /// </summary>
        /// <param name="value">The value as a string</param>
        /// <returns></returns>
        T Convert(string value);

        /// <summary>
        /// Converts the <paramref name="value"/> from <typeparam name="T">the converter type</typeparam> back to string.
        /// </summary>
        /// <param name="value">The value as string</param>
        string ConvertBack(T value);
    }
}
