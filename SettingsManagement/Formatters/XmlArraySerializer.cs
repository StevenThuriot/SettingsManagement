using SettingsManagement.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SettingsManagement.Formatters
{
    /// <summary>
    /// Xml Array Serializer
    /// </summary>
    public class XmlArraySerializer : ISettingsSerializer
    {
        /// <summary>
        /// Format settings to Xml
        /// </summary>
        /// <param name="settings">The available settings</param>
        /// <returns></returns>
        public string Serialize(IReadOnlyList<ISetting> settings)
        {
            var builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<root>\r\n");

            const string indent = "  ";

            foreach (var setting in settings)
            {
                builder.Append(indent).AppendLine("<element>").Append(indent)
                       .Append(indent)
                       .Append("<Key>")
                       .Append(setting.Key)
                       .AppendLine("</Key>");

                builder.AppendValueAndDescription(setting, indent);

                builder.Append(indent).AppendLine("</element>");
            }

            return builder.Append("</root>").ToString();
        }
    }
}
