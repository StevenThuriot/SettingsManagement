using SettingsManagement.Interfaces;
using System.Text;

namespace SettingsManagement.Formatters;

/// <summary>
/// Formats to a Json Instance
/// </summary>
public class JsonInstanceSerializer : ISettingsSerializer
{
    /// <summary>
    /// Format settings to Json
    /// </summary>
    /// <param name="settings">The available settings</param>
    /// <returns></returns>
    public string Serialize(IReadOnlyList<ISetting> settings)
    {
        var builder = new StringBuilder("{\r\n  ");

        for (int i = 0; i < settings.Count; i++)
        {
            var setting = settings[i];
            var appendNewLineDelimiter = (i + 1) < settings.Count;

            const string indent = "  ";

            builder.Append("\"")
                   .Append(setting.Key)
                   .Append("\": ")
                   .AppendLine("{")
                   .Append(indent);

            var value = setting.ResolveValue();

            if (!(value is null))
            {
                builder.Append(indent)
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

        return builder.Append("  \r\n}").ToString();
    }
}
