using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SettingsManagement
{
    static class ConversionHelper<T>
    {
        static readonly IDictionary<Type, Func<string, T>> _cachedConverters = new Dictionary<Type, Func<string, T>>();
        public static Func<string, T> Resolve(Type converterType)
        {
            if (!_cachedConverters.TryGetValue(converterType ?? typeof(T), out var converter))
            {
                lock (_cachedConverters)
                {
                    if (!_cachedConverters.TryGetValue(converterType ?? typeof(T), out converter))
                    {
                        if (converterType == null)
                        {
                            if (typeof(T) == typeof(string))
                            {
                                converter = (Func<string, T>)(object)new Func<string, string>(s => s);
                            }
                            else
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

                                converter = lambda.Compile();
                            }

                            _cachedConverters[typeof(T)] = converter;
                        }
                        else
                        {
                            var converterInstance = (IConverter<T>)Activator.CreateInstance(converterType);
                            converter = new Func<string, T>(s => converterInstance.Convert(s));

                            _cachedConverters[converterType] = converter;
                        }

                    }
                }
            }

            return converter;
        }
    }
}
