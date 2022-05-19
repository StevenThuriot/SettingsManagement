using System.Collections;
using System.Text;

namespace SettingsManagement.Formatters;

static class JsonHelper
{
    public static void AppendJsonValue(this StringBuilder builder, object value)
    {
        if (value is null)
            return;

        if (value is bool @bool)
        {
            builder.Append(@bool.ToString().ToLowerInvariant());
        }
        else if (value is string @string)
        {
            builder.Append("\"").Append(@string).Append("\"");
        }
        else if (value is IEnumerable enumerable)
        {
            builder.Append("[ ");

            foreach (var item in enumerable)
            {
                builder.AppendJsonValue(item);
                builder.Append(", ");
            }

            for (int i = builder.Length - 1; i >= 0; i--)
            {
                var character = builder[i];

                if (character != ' ' && character != ',' && character != '\r' && character != '\n')
                {
                    if (i < builder.Length - 1)
                        builder.Length = i + 1;

                    break;
                }
            }

            builder.Append("]");
        }
        else if (value.GetType().IsPrimitive)
        {
            builder.Append(value);
        }
        else
        {
            builder.Append("\"").Append(value).Append("\"");
        }
    }
}
