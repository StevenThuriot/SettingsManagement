using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement
{
    public static class SettingsManager
    {
        static class TypedSettingsManager<T>
        {
            static readonly IDictionary<string, T> _managers = new Dictionary<string, T>();

            public static T New()
            {
                if (!_managerTypes.TryGetValue(typeof(T), out var managerType))
                {
                    lock (_managerTypes)
                    {
                        if (!_managerTypes.TryGetValue(typeof(T), out managerType))
                        {
                            var myTypeInfo = CompileResultTypeInfo(typeof(T));
                            _managerTypes[typeof(T)] = managerType = myTypeInfo.AsType();
                        }
                    }
                }

                var myObject = Activator.CreateInstance(managerType);
                return (T)myObject;
            }

            public static T Get(string key)
            {
                key += "_" + typeof(T).FullName;

                if (!_managers.TryGetValue(key, out var manager))
                {
                    lock (_managers)
                    {
                        if (!_managers.TryGetValue(key, out manager))
                        {
                            _managers[key] = manager = New();
                        }
                    }
                }

                return manager;
            }
        }

        const MethodAttributes PropertyAttributes = MethodAttributes.Public
                                                             | MethodAttributes.Final
                                                             | MethodAttributes.HideBySig
                                                             | MethodAttributes.SpecialName
                                                             | MethodAttributes.NewSlot
                                                             | MethodAttributes.Virtual;

        static readonly AssemblyBuilder _assemblyBuilder;
        static readonly ModuleBuilder _moduleBuilder;

        static readonly IDictionary<Type, Type> _managerTypes = new Dictionary<Type, Type>();

        static SettingsManager()
        {
            var assemblyName = new AssemblyName("SettingsManagement.Emit");
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("SettingsManagement.Emit.Module");
        }

        public static T New<T>() => TypedSettingsManager<T>.New();

        public static T Get<T>(string key = null) => TypedSettingsManager<T>.Get(key);

        public static TypeInfo CompileResultTypeInfo(Type type)
        {
            var typeBuilder = _moduleBuilder.DefineType("SettingsManagement.Emit." + type.FullName,
                                                            TypeAttributes.Public
                                                            | TypeAttributes.Class
                                                            | TypeAttributes.AutoClass
                                                            | TypeAttributes.AnsiClass
                                                            | TypeAttributes.BeforeFieldInit
                                                            | TypeAttributes.AutoLayout,
                                                            null);

            var properties = ImplementInterface(type, typeBuilder);
            ImplementConstructor(typeBuilder, properties);

            return typeBuilder.CreateTypeInfo();
        }

        static void ImplementConstructor(TypeBuilder typeBuilder, IReadOnlyList<PropertyDescriptor> properties)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public
                                                                    | MethodAttributes.SpecialName
                                                                    | MethodAttributes.RTSpecialName
                                                                    | MethodAttributes.HideBySig,
                                                                    CallingConventions.Standard,
                                                                    Type.EmptyTypes);

            var ctrIl = constructorBuilder.GetILGenerator();

            foreach (var property in properties)
            {
                var converterType = property.ConverterType;
                var defaultValue = property.DefaultValue;

                MethodInfo creationMethod;
                Type defaultValueType;
                if (defaultValue is string)
                {
                    creationMethod = SettingsBuilderHelper.ResolveCreateAndParse(property.PropertyType);
                    defaultValueType = typeof(string);
                }
                else
                {
                    creationMethod = SettingsBuilderHelper.ResolveCreate(property.PropertyType);
                    defaultValueType = property.PropertyType;
                }

                ctrIl.Emit(OpCodes.Ldarg_0);
                ctrIl.Emit(OpCodes.Ldstr, property.Name);
                ctrIl.EmitConstant(defaultValue, defaultValueType);
                ctrIl.EmitConstant(converterType);
                ctrIl.Emit(OpCodes.Call, creationMethod);
                ctrIl.Emit(OpCodes.Stfld, property.FieldBuilder);

                var description = property.Description;
                if (!string.IsNullOrWhiteSpace(description))
                {
                    ctrIl.Emit(OpCodes.Ldarg_0);
                    ctrIl.Emit(OpCodes.Ldfld, property.FieldBuilder);
                    ctrIl.Emit(OpCodes.Ldstr, description);

                    var setDescription = property.BackingFieldType.GetProperty("Description").GetSetMethod();
                    ctrIl.Emit(OpCodes.Callvirt, setDescription);
                }
            }

            ctrIl.Emit(OpCodes.Ret);
        }

        static IReadOnlyList<PropertyDescriptor> ImplementInterface(Type type, TypeBuilder typeBuilder)
        {
            typeBuilder.AddInterfaceImplementation(type);

            var properties = new List<PropertyDescriptor>();

            foreach (var property in type.GetProperties())
            {
                var fieldBuilder = CreateProperty(typeBuilder, property);
                properties.Add(fieldBuilder);
            }

            if (typeof(ISettingsManager).IsAssignableFrom(type))
            {
                //TODO
                //Add Persist
                //Add Refresh
                //Add GetReadableValues
            }

            return properties;
        }

        static PropertyDescriptor CreateProperty(TypeBuilder typeBuilder, PropertyInfo property)
        {
            var propertyName = property.Name;
            var propertyType = property.PropertyType;

            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, typeof(Setting<>).MakeGenericType(propertyType), FieldAttributes.Private);
            var valueProperty = fieldBuilder.FieldType.GetProperty("Value");

            var propertyBuilder = typeBuilder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.None, propertyType, null);
            var getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, PropertyAttributes, propertyType, Type.EmptyTypes);

            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Callvirt, valueProperty.GetGetMethod());
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, PropertyAttributes, null, new[] { propertyType });
            var setIl = setPropMthdBldr.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldfld, fieldBuilder);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Callvirt, valueProperty.GetSetMethod());
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);

            return new PropertyDescriptor(property, propertyBuilder, fieldBuilder);
        }
    }
}
