using System;

namespace SettingsManagement
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingsConverterAttribute : Attribute
    {
        public SettingsConverterAttribute(Type type)
        {
            //TODO: Check if it implements IConverter<>
            ConverterType = type;
        }

        public Type ConverterType { get; }
    }
}
