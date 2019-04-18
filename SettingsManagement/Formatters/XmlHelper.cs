using SettingsManagement.Interfaces;
using System.Collections;
using System.Text;

namespace SettingsManagement.Formatters
{
    static class XmlHelper
    {
        public static void AppendValueAndDescription(this StringBuilder builder, ISetting setting, string indent)
        {
            object value = setting.ResolveValue();
            if (!(value is null))
            {
                builder.Append(indent)
                       .Append(indent)
                       .Append("<Value>");

                if (value is bool @bool)
                {
                    builder.Append(@bool.ToString().ToLowerInvariant());
                }
                else if (!(value is string) && value is IEnumerable enumerable)
                {
                    builder.Append(string.Concat(", ", enumerable));
                }
                else if (value.GetType().IsPrimitive)
                {
                    builder.Append(value);
                }
                else
                {
                    builder.Append(value);
                }

                builder.AppendLine("/<Value>");
            }

            if (!string.IsNullOrWhiteSpace(setting.Description))
            {
                builder.Append(indent)
                       .Append(indent)
                       .Append("<Description>")
                       .Append(setting.Description)
                       .AppendLine("</Description>");
            }
        }
    }
}
