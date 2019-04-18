using SettingsManagement.Attributes;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement
{
    class PropertyDescriptor
    {
        public PropertyDescriptor(PropertyInfo property, PropertyBuilder propertyBuilder, FieldBuilder fieldBuilder)
        {
            Property = property;
            PropertyBuilder = propertyBuilder;
            FieldBuilder = fieldBuilder;
        }

        public string Name => Property.Name;
        public Type BackingFieldType => FieldBuilder.FieldType;
        public Type PropertyType => Property.PropertyType;

        public string Description => Property.GetCustomAttribute<DescriptionAttribute>()?.Description;
        public object DefaultValue => Property.GetCustomAttribute<DefaultValueAttribute>()?.Value;
        public Type ConverterType => Property.GetCustomAttribute<SettingsConverterAttribute>()?.ConverterType;

        public PropertyInfo Property { get; }
        public FieldBuilder FieldBuilder { get; }
        public PropertyBuilder PropertyBuilder { get; }
    }
}
