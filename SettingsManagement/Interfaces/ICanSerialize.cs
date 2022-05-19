namespace SettingsManagement.Interfaces;

/// <summary>
/// I Know About Serialization
/// </summary>
public interface ICanSerialize
{
    /// <summary>
    /// Converts the current settings manager to the selected format
    /// </summary>
    string Serialize();
}

///// <summary>
///// I Know About Serialization with a custom serializer
///// </summary>
//public interface ICanSerializeWith<TSerializer> : ICanSerialize
//    where TSerializer : ISettingsSerializer
//{

//}
