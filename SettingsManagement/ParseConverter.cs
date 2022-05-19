using SettingsManagement.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace SettingsManagement;

class ParseConverter<T> : IValueConverter<T>
{
    static readonly Func<string, T> _converter;

    static ParseConverter()
    {
        var parseMethod = typeof(T).GetMethod("Parse",
                                                BindingFlags.Public | BindingFlags.Static,
                                                null,
                                                new[] { typeof(string) },
                                                null);

        if (parseMethod == null)
            throw new NotSupportedException("No parse found for type " + typeof(T));

        var input = Expression.Parameter(typeof(string), "stringValue");
        var caller = Expression.Call(parseMethod, input);
        var lambda = Expression.Lambda<Func<string, T>>(caller, input);

        _converter = lambda.Compile();
    }

    public T Convert(string value)
    {
        return _converter(value);
    }

    public string ConvertBack(T value)
    {
        if (value is null)
            return "";

        return value.ToString();
    }
}
