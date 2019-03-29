using System;
using System.Collections.Generic;
using System.Reflection;

namespace SettingsManagement
{
    static class SettingsBuilderHelper
    {
        static readonly IDictionary<Type, MethodInfo> _initMethods = new Dictionary<Type, MethodInfo>();
        static readonly IDictionary<Type, MethodInfo> _creationMethods = new Dictionary<Type, MethodInfo>();
        static readonly IDictionary<Type, MethodInfo> _createAndParseMethods = new Dictionary<Type, MethodInfo>();

        public static MethodInfo ResolveInit(Type type)
        {
            if (!_initMethods.TryGetValue(type, out var info))
            {
                _initMethods[type] = info = typeof(SettingsBuilder<>).MakeGenericType(type).GetMethod("Init");
            }

            return info;
        }
		
        public static MethodInfo ResolveCreate(Type type)
        {
            if (!_creationMethods.TryGetValue(type, out var info))
            {
                _creationMethods[type] = info = typeof(SettingsBuilder<>).MakeGenericType(type).GetMethod("Create");
            }

            return info;
        }

        public static MethodInfo ResolveParseAndCreate(Type type)
        {
            if (!_createAndParseMethods.TryGetValue(type, out var info))
            {
                _createAndParseMethods[type] = info = typeof(SettingsBuilder<>).MakeGenericType(type).GetMethod("ParseAndCreate");
            }

            return info;
        }
    }
}
