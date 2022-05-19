using SettingsManagement.Interfaces;
using System.Reflection;

namespace SettingsManagement.Attributes;

/// <summary>
/// Allows defining a custom serializer for a certain type.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class SettingsSerializerAttribute : Attribute
{
    /// <summary>
    /// Constructs a SettingsSerializer
    /// </summary>
    /// <param name="type">The serializer type.</param>
    /// <remarks>The type should implement ISettingsSerializer </remarks>
    public SettingsSerializerAttribute(Type type)
    {
        if (type.IsAbstract)
            throw new ArgumentException("The Settings Serializer cannot be an abstract class");

        if (type.IsInterface)
            throw new ArgumentException("The Settings Serializer cannot be an interface");

        if (!typeof(ISettingsSerializer).IsAssignableFrom(type))
            throw new ArgumentException("The Settings Serializer needs to implement " + typeof(ISettingsSerializer).FullName);

        SerializerType = type;
        Constructor = type.GetConstructor(Type.EmptyTypes) ?? throw new ArgumentException("The Settings Serializer must implement a default Constructor");
    }

    /// <summary>
    /// The type of serializer
    /// </summary>
    public Type SerializerType { get; }

    /// <summary>
    /// The serializer Constructor
    /// </summary>
    public ConstructorInfo Constructor { get; }
}
