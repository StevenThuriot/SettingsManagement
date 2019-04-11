using SettingsManagement.Interfaces;
using System;

namespace SettingsManagement
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingsConverterAttribute : Attribute
    {
        public SettingsConverterAttribute(Type type)
        {
            var converterTypeName = typeof(IValueConverter<>).FullName;
            var interfaceType = type.GetInterface(converterTypeName);

            if (interfaceType == null)
                throw new ArgumentException("Convert type needs to implement " + converterTypeName);

            ConversionType = interfaceType.GetGenericArguments()[0];
            ConverterType = type;
        }

        public Type ConversionType { get; }
        public Type ConverterType { get; }
    }
}
