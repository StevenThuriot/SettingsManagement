using SettingsManagement.Interfaces;
using System.Reflection;
using System.Text;

namespace SettingsManagement;

static class ConfigurationHelper
{
    public static class Managers
    {
        public static readonly MethodInfo RefreshMethod = typeof(IConfigurationManager).GetMethod(nameof(IConfigurationManager.Refresh), Type.EmptyTypes);
        public static readonly MethodInfo PersistMethod = typeof(IConfigurationManager).GetMethod(nameof(IConfigurationManager.Persist), Type.EmptyTypes);
    }

    public static class Settings
    {
        public static readonly PropertyInfo DescriptionProperty = typeof(ISettingExtended).GetProperty(nameof(ISettingExtended.Description));

        public static MethodInfo DescriptionGetter => DescriptionProperty.GetGetMethod();
        public static MethodInfo DescriptionSetter => DescriptionProperty.GetSetMethod();

        public static readonly MethodInfo RefreshMethod = typeof(ISettingExtended).GetMethod(nameof(ISettingExtended.Refresh), Type.EmptyTypes);
        public static readonly MethodInfo ResetMethod = typeof(ISettingExtended).GetMethod(nameof(ISettingExtended.Reset), Type.EmptyTypes);
        public static readonly MethodInfo PersistMethod = typeof(ISettingExtended).GetMethod(nameof(ISettingExtended.Persist), Type.EmptyTypes);
        public static readonly MethodInfo ReadableValueMethod = typeof(ISettingExtended).GetMethod(nameof(ISettingExtended.GetReadableValue), Type.EmptyTypes);
    }

    public static class Strings
    {
        public static readonly MethodInfo Equality = typeof(string).GetMethod("op_Equality");
        //public static readonly MethodInfo IsNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty");
        public static readonly MethodInfo ToStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
        public static readonly MethodInfo JoinMethod = typeof(string).GetMethod("Join", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(IEnumerable<string>) }, null);
        public static readonly MethodInfo ConcatMethod = typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null);
    }

    public static class Types
    {
        public static readonly MethodInfo GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
    }

    public static class StringDictionaries
    {
        public static readonly ConstructorInfo DefaultCtor = typeof(Dictionary<string, string>).GetConstructor(Type.EmptyTypes);
        public static readonly MethodInfo SetIndexer = typeof(Dictionary<string, string>).GetMethod("set_Item");
    }

    public static class Exceptions
    {
        public static readonly ConstructorInfo KeyNotFound = typeof(KeyNotFoundException).GetConstructor(new[] { typeof(string) });
    }

    public static class StringBuilders
    {
        public static readonly ConstructorInfo CtorWithString = typeof(StringBuilder).GetConstructor(new[] { typeof(string) });
        public static readonly MethodInfo AppendString = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });

    }

    public static class Serializers
    {
        public static readonly MethodInfo SerializeMethod = typeof(ISettingsSerializer).GetMethod(nameof(ISettingsSerializer.Serialize), new[] { typeof(IReadOnlyList<ISetting>) });
    }

}
