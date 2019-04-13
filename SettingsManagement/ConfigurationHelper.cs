using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SettingsManagement
{
    static class ConfigurationHelper
    {
        public static class Managers
        {
            public static readonly MethodInfo RefreshMethod = typeof(IConfigurationManager).GetMethod(nameof(IConfigurationManager.Refresh), Type.EmptyTypes);
            public static readonly MethodInfo PersistMethod = typeof(IConfigurationManager).GetMethod(nameof(IConfigurationManager.Persist), Type.EmptyTypes);
        }

        public static class Settings
        {
            public static readonly MethodInfo RefreshMethod = typeof(ISetting).GetMethod(nameof(ISetting.Refresh), Type.EmptyTypes);
            public static readonly MethodInfo PersistMethod = typeof(ISetting).GetMethod(nameof(ISetting.Persist), Type.EmptyTypes);
            public static readonly MethodInfo ReadableValueMethod = typeof(ISetting).GetMethod(nameof(ISetting.GetReadableValue), Type.EmptyTypes);
            public static readonly MethodInfo ToStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
            public static readonly MethodInfo JoinMethod = typeof(string).GetMethod("Join", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(IEnumerable<string>) }, null);
            public static readonly MethodInfo ConcatMethod = typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null);
        }
    }
}
