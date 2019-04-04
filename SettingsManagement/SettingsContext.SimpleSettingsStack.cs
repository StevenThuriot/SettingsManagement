using System;

namespace SettingsManagement
{
    partial class SettingsContext
    {
        sealed class SimpleSettingsStack
        {
            int _size;
            SettingsContext[] _array = new SettingsContext[2];

            public int Count => _size;

            public SettingsContext Peek()
            {
                if (_size == 0)
                    return AppContext;

                return _array[_size - 1];
            }

            public void Push(SettingsContext item)
            {
                if (_size == _array.Length)
                {
                    var newArray = new SettingsContext[2 * _array.Length];
                    Array.Copy(_array, 0, newArray, 0, _size);
                    _array = newArray;
                }

                _array[_size++] = item;
            }

            public SettingsContext Pop()
            {
                if (_size == 0)
                    return AppContext;

                var item = _array[--_size];
                _array[_size] = AppContext; // Free memory quicker.

                return item;
            }
        }
    }
}
