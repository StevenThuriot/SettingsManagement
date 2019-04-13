using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    sealed class CreateTypeBlock : Block
    {
        const MethodAttributes PropertyAttributes = MethodAttributes.Public
                                                             | MethodAttributes.Final
                                                             | MethodAttributes.HideBySig
                                                             | MethodAttributes.SpecialName
                                                             | MethodAttributes.NewSlot
                                                             | MethodAttributes.Virtual;

        public override TypeBuilder Builder { get; }

        public override Type Interface { get; }

        public override IReadOnlyList<PropertyDescriptor> Properties { get; }

        public override FieldBuilder ConfigurationManagerField { get; }

        public CreateTypeBlock(Type type, ModuleBuilder moduleBuilder)
        {
            var typename = "SettingsManagement.Emit." + type.FullName;
            if (!typename.EndsWith("Manager", StringComparison.OrdinalIgnoreCase))
                typename += "Manager";

            Interface = type;
            Builder = moduleBuilder.DefineType(typename, TypeAttributes.Public
                                                            | TypeAttributes.Class
                                                            | TypeAttributes.AutoClass
                                                            | TypeAttributes.AnsiClass
                                                            | TypeAttributes.BeforeFieldInit
                                                            | TypeAttributes.AutoLayout,
                                                            null);

            ConfigurationManagerField = Builder.DefineField("___configurationManager", typeof(IConfigurationManager), FieldAttributes.Private);

            Builder.AddInterfaceImplementation(Interface);

            var properties = new List<PropertyDescriptor>();

            foreach (var property in Interface.GetProperties())
            {
                var fieldBuilder = CreateProperty(Builder, property);
                properties.Add(fieldBuilder);
            }

            Properties = properties;
        }

        public override Type Build()
        {
            return Builder.CreateTypeInfo().AsType();
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
