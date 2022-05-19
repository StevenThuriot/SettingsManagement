using System.Reflection;

namespace SettingsManagement;

static class SettingsBuilderHelper
{
    const BindingFlags FLAGS = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
    static readonly IDictionary<Type, MethodInfo> _creationMethods = new Dictionary<Type, MethodInfo>();
    static readonly IDictionary<Type, MethodInfo> _createAndParseMethods = new Dictionary<Type, MethodInfo>();

    public static MethodInfo ResolveCreate(Type type)
    {
        if (!_creationMethods.TryGetValue(type, out var info))
        {
            _creationMethods[type] = info = typeof(SettingsBuilder<>).MakeGenericType(type).GetMethod("Create", FLAGS);
        }

        return info;
    }

    public static MethodInfo ResolveCreateAndParse(Type type)
    {
        if (!_createAndParseMethods.TryGetValue(type, out var info))
        {
            _createAndParseMethods[type] = info = typeof(SettingsBuilder<>).MakeGenericType(type).GetMethod("ParseAndCreate", FLAGS);
        }

        return info;
    }
}
