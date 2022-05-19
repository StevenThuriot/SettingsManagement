using SettingsManagement.Interfaces;
using System.Security;

namespace SettingsManagment.Secure;

/// <summary>
/// A secure converter which will use the DataProtection API to secure <see cref="string"/>
/// </summary>
public class ProtectedSecureStringConverter : IValueConverter<SecureString>
{
    private readonly IValueConverter<SecureString> _converter = new SecureConverter();

    /// <summary>
    /// Converts the <paramref name="value" /> from string to SecureString.
    /// </summary>
    /// <param name="value">The value as a string</param>
    /// <returns></returns>
    public SecureString Convert(string value)
    {
        return _converter.Convert(value);
    }

    /// <summary>
    /// Converts the <paramref name="value" /> from SecureString back to string.
    /// </summary>
    /// <param name="value">The value as string</param>
    public string ConvertBack(SecureString value)
    {
        return _converter.ConvertBack(value);
    }
}