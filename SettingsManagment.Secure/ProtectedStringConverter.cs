using SettingsManagement.Interfaces;

namespace SettingsManagment.Secure;

/// <summary>
/// A secure converter which will use the DataProtection API to secure <see cref="string"/>
/// </summary>
public class ProtectedStringConverter : IValueConverter<string>
{
    private readonly IValueConverter<string> _converter = new SecureConverter();

    /// <summary>
    /// Converts the <paramref name="value" /> from string to string.
    /// </summary>
    /// <param name="value">The value as a string</param>
    /// <returns></returns>
    public string Convert(string value)
    {
        return _converter.Convert(value);
    }

    /// <summary>
    /// Converts the <paramref name="value" /> from string back to string.
    /// </summary>
    /// <param name="value">The value as string</param>
    public string ConvertBack(string value)
    {
        return _converter.ConvertBack(value);
    }
}
