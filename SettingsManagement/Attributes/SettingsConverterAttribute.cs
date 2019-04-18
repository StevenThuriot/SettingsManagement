using SettingsManagement.Interfaces;
using System;

namespace SettingsManagement.Attributes
{
    /// <summary>
    /// Allows defining a custom Converter for a certain type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingsConverterAttribute : Attribute
    {
        /// <summary>
        /// Constructs a SettingsConverter for a type.
        /// </summary>
        /// <param name="type">The converter type.</param>
        /// <remarks>The type should implement IValueConverter&lt;&gt; </remarks>
        public SettingsConverterAttribute(Type type)
        {
            var converterTypeName = typeof(IValueConverter<>).FullName;
            var interfaceType = type.GetInterface(converterTypeName);

            if (interfaceType == null)
                throw new ArgumentException("Convert type needs to implement " + converterTypeName);

            ConversionType = interfaceType.GetGenericArguments()[0];
            ConverterType = type;
        }

        /// <summary>
        /// The type that the converter is used to convert.
        /// </summary>
        public Type ConversionType { get; }

        /// <summary>
        /// The type of the converter itself.
        /// </summary>
        public Type ConverterType { get; }
    }
}
