using SettingsManagement.Attributes;

namespace SettingsManagement.Tests.Models;

public interface IBrokenSettingsBecauseConverter
{
    [SettingsConverter(typeof(object))]
    string MyProperty{ get; set; }
}

public interface IBrokenSettingsBecauseDefaultValue
{
    object MyProperty{ get; set; }
}

interface IBrokenSettingsBecauseInternal
{
    string MyProperty{ get; set; }
}
