using SettingsManagement.Interfaces;
using System.Reflection;
using System.Text;

namespace SettingsManagement.Formatters;

/// <summary>
/// Formats to a Json Array
/// </summary>
public class JsonArraySerializer : ISettingsSerializer
{
    internal static readonly ConstructorInfo Constructor = typeof(JsonArraySerializer).GetConstructor(Type.EmptyTypes);

    /// <summary>
    /// Format settings to Json
    /// </summary>
    /// <param name="settings">The available settings</param>
    /// <returns></returns>
    public string Serialize(IReadOnlyList<ISetting> settings)
    {
        var builder = new StringBuilder("[\r\n  ");

        for (int i = 0; i < settings.Count; i++)
        {
            var setting = settings[i];
            var appendNewLineDelimiter = (i + 1) < settings.Count;
            const string indent = "  ";

            builder.AppendLine("{").Append(indent)
                   .Append(indent)
                   .Append("\"Key\": \"")
                   .Append(setting.Key)
                   .Append("\"");

            var value = setting.ResolveValue();

            if (!(value is null))
            {
                builder.AppendLine(",").Append(indent)
                       .Append(indent)
                       .Append("\"Value\": ")
                       .AppendJsonValue(value);
            }

            if (!string.IsNullOrWhiteSpace(setting.Description))
            {
                builder.AppendLine(",").Append(indent)
                       .Append(indent)
                       .Append("\"Description\": \"")
                       .Append(setting.Description)
                       .Append("\"");
            }

            builder.AppendLine().Append(indent).Append("}");

            if (appendNewLineDelimiter)
            {
                builder.AppendLine(",").Append(indent);
            }
        }

        return builder.Append("  \r\n]").ToString();
    }
}
