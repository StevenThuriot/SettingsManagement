using Microsoft.AspNetCore.DataProtection;
using SettingsManagement.Interfaces;
using System.Runtime.InteropServices;
using System.Security;

namespace SettingsManagment.Secure;

/// <summary>
/// A secure converter which will use the DataProtection API to secure <see cref="string"/> and <see cref="SecureString"/>
/// </summary>
public class SecureConverter : IValueConverter<string>, IValueConverter<SecureString>
{
    private readonly IDataProtector _protector;

    /// <summary>
    /// Creates a new instance of a secure converter which will use the DataProtection API to secure <see cref="string"/> and <see cref="SecureString"/>
    /// </summary>
    public SecureConverter()
    {
        var provider = DataProtectionProvider.Create("AniDBAPI"); //TODO
        _protector = provider.CreateProtector("LoginData");
    }

    string IValueConverter<string>.Convert(string value)
    {
        return _protector.Unprotect(value);
    }

    SecureString IValueConverter<SecureString>.Convert(string value)
    {
        SecureString secureString = new();

        foreach (var character in _protector.Unprotect(value))
        {
            secureString.AppendChar(character);
        }

        secureString.MakeReadOnly();

        return secureString;
    }

    string IValueConverter<string>.ConvertBack(string value)
    {
        return _protector.Protect(value);
    }

    string IValueConverter<SecureString>.ConvertBack(SecureString value)
    {
        IntPtr valuePtr = IntPtr.Zero;
        try
        {
            valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
            var stringValue = Marshal.PtrToStringUni(valuePtr);
            return _protector.Protect(stringValue);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
        }
    }
}