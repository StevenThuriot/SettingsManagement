using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;

namespace SettingsManagement
{
    static class ConversionHelper<T>
    {
        static readonly IDictionary<Type, IValueConverter<T>> _cachedConverters = new Dictionary<Type, IValueConverter<T>>();
        public static IValueConverter<T> Resolve(Type converterType)
        {
            Type key = converterType ?? typeof(T);
            if (!_cachedConverters.TryGetValue(key, out var converter))
            {
                lock (_cachedConverters)
                {
                    if (!_cachedConverters.TryGetValue(key, out converter))
                    {
                        if (converterType == null)
                        {
                            if (typeof(T) == typeof(string))
                            {
                                converter = (IValueConverter<T>)(object)new StringConverter();
                            }
                            else
                            {
                                converter = new ParseConverter<T>();
                            }

                            _cachedConverters[typeof(T)] = converter;
                        }
                        else
                        {
                            converter = (IValueConverter<T>)Activator.CreateInstance(converterType);
                            _cachedConverters[converterType] = converter;
                        }

                    }
                }
            }

            return converter;
        }
    }
}
