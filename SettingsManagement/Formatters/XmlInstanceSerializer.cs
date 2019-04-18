using SettingsManagement.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SettingsManagement.Formatters
{
    /// <summary>
    /// Xml Instance Serializer
    /// </summary>
    public class XmlInstanceSerializer : ISettingsSerializer
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
                builder.Append(indent).Append("<").Append(setting.Key).AppendLine(">");
                builder.AppendValueAndDescription(setting, indent);
                builder.Append(indent).Append("</").Append(setting.Key).AppendLine(">");
            }

            return builder.Append("</root>").ToString();
        }
    }
}
