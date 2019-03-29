using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement
{
    public static class SettingsManager
    {
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


        public static T Get<T>()
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

            return (T) myObject;
        }

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

            var fields = ImplementInterface(type, typeBuilder);
            ImplementConstructor(typeBuilder, fields);

            TypeInfo objectTypeInfo = typeBuilder.CreateTypeInfo();
            return objectTypeInfo;
        }

        static void ImplementConstructor(TypeBuilder typeBuilder, IReadOnlyList<FieldBuilder> fields)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public 
                                                                    | MethodAttributes.SpecialName 
                                                                    | MethodAttributes.RTSpecialName
                                                                    | MethodAttributes.HideBySig,
                                                                    CallingConventions.Standard,
                                                                    Type.EmptyTypes);

            var ctrIl = constructorBuilder.GetILGenerator();

            foreach (var field in fields)
            {
                var initMethod = typeof(SettingsBuilder<>).MakeGenericType(field.FieldType.GetGenericArguments()[0])
                                                          .GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );

                ctrIl.Emit(OpCodes.Ldarg_0);
                ctrIl.Emit(OpCodes.Ldstr, field.Name.Substring(1));
                ctrIl.Emit(OpCodes.Call, initMethod);
                ctrIl.Emit(OpCodes.Stfld, field);

                //TODO: Set Converter
                //TODO: Set Description
                //TODO: Set Default Value
            }

            ctrIl.Emit(OpCodes.Ret);
        }

        static IReadOnlyList<FieldBuilder> ImplementInterface(Type type, TypeBuilder typeBuilder)
        {
            typeBuilder.AddInterfaceImplementation(type);

            var fieldBuilders = new List<FieldBuilder>();

            foreach (var property in type.GetProperties())
            {
                var fieldBuilder = CreateProperty(typeBuilder, property.Name, property.PropertyType);
                fieldBuilders.Add(fieldBuilder);
            }

            //TODO: Add Persist and Refresh methods if interface has them

            return fieldBuilders;
        }

        static FieldBuilder CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            var fieldBuilder = tb.DefineField("_" + propertyName, typeof(Setting<>).MakeGenericType(propertyType), FieldAttributes.Private);
            var valueProperty = fieldBuilder.FieldType.GetProperty("Value");

            var propertyBuilder = tb.DefineProperty(propertyName, System.Reflection.PropertyAttributes.None, propertyType, null);
            var getPropMthdBldr = tb.DefineMethod("get_" + propertyName, PropertyAttributes, propertyType, Type.EmptyTypes);

            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Callvirt, valueProperty.GetGetMethod());
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr = tb.DefineMethod("set_" + propertyName, PropertyAttributes, null, new[] { propertyType });
            var setIl = setPropMthdBldr.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldfld, fieldBuilder);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Callvirt, valueProperty.GetSetMethod());
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);

            return fieldBuilder;
        }
    }
}
