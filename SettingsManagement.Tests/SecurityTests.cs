using SettingsManagement.Attributes;
using SettingsManagement.Interfaces;
using SettingsManagment.Secure;
using System.Runtime.InteropServices;
using System.Security;

namespace SettingsManagement.Tests;

public interface ISecureSettings : ICanPersist, ICanReset
{
    [SettingsConverter(typeof(SecureConverter), typeof(string))]
    string StringValue { get; set; }
    [SettingsConverter(typeof(SecureConverter), typeof(SecureString))]
    SecureString SecureStringValue { get; set; }

    [SettingsConverter(typeof(ProtectedStringConverter))]
    string StringValue2 { get; set; }
}

public class SecurityTests
{
    [Fact]
    public void SecureConverterShouldProtectMyData()
    {
        InMemoryManager manager = new();
        var settings = SettingsManager.New<ISecureSettings>(manager);

        Assert.NotNull(settings);

        settings.StringValue = "test1";

        settings.SecureStringValue = new SecureString();
        settings.SecureStringValue.AppendChar('t');
        settings.SecureStringValue.AppendChar('e');
        settings.SecureStringValue.AppendChar('s');
        settings.SecureStringValue.AppendChar('t');
        settings.SecureStringValue.AppendChar('2');
        settings.SecureStringValue.MakeReadOnly();

        settings.StringValue2 = "test3";

        settings.Persist();

        Assert.NotNull(manager.Get(nameof(settings.StringValue)));

        //Make sure manager values are obscured
        Assert.NotEqual(settings.StringValue, manager.Get(nameof(settings.StringValue)));
        Assert.NotEqual(ReadSecureString(settings.SecureStringValue), manager.Get(nameof(settings.SecureStringValue)));
        Assert.NotEqual(settings.StringValue2, manager.Get(nameof(settings.StringValue2)));

        settings = SettingsManager.New<ISecureSettings>(manager);

        Assert.NotNull(settings.StringValue);
        Assert.NotNull(settings.SecureStringValue);

        //Make sure manager values are no longer obscured on our settings class
        Assert.Equal("test1", settings.StringValue);
        Assert.Equal("test2", ReadSecureString(settings.SecureStringValue));
        Assert.Equal("test3", settings.StringValue2);
    }

    private static string ReadSecureString(SecureString value)
    {
        IntPtr valuePtr = IntPtr.Zero;
        try
        {
            valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
            return Marshal.PtrToStringUni(valuePtr);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
        }
    }
}
