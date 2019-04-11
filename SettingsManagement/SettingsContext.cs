using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SettingsManagement
{
    public sealed partial class SettingsContext : IDisposable
    {
        bool _disposed;
        readonly string _scopeName;
        readonly SettingsContext _parentContext;
        readonly SimpleSettingsStack _myStack;
        readonly IDictionary<string, object> _managers;

        public override string ToString() => _scopeName;

        SettingsContext() //Only for app-wide context
        {
            _scopeName = nameof(AppContext);
            _parentContext = null;
            _managers = new Dictionary<string, object>();
            (_myStack = _localScopeStack.Value).Push(this);
        }

        SettingsContext(string scopeName, SettingsContext parentContext)
        {
            _scopeName = "Scope: " + scopeName ?? "Unnamed";
            _parentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));
            _managers = new Dictionary<string, object>();
            (_myStack = _localScopeStack.Value).Push(this);
        }

        ~SettingsContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                var popped = _myStack.Pop();

                var disposables = _managers.Values.OfType<IDisposable>().ToArray();

                _managers.Clear();

                Array.ForEach(disposables, x => x.Dispose());

                if (!ReferenceEquals(popped, this))
                    throw new NotSupportedException("Improper Scope Usage. Disposed Settings scope is different from Current. Inner scopes should always be disposed first.");
            }

            _disposed = true;
        }

        static readonly ThreadLocal<SimpleSettingsStack> _localScopeStack = new ThreadLocal<SimpleSettingsStack>(() => new SimpleSettingsStack());

        /// <summary>
        /// Application-wide context scope
        /// </summary>
        public static readonly SettingsContext AppContext = new SettingsContext();

        /// <summary>
        /// Get the current scope. If not scoped, the Application wide one will be returned.
        /// </summary>
        public static SettingsContext Current => _localScopeStack.Value.Peek();

        /// <summary>
        /// Create a new management scope
        /// </summary>
        /// <returns></returns>
        public static SettingsContext BeginScope(string name = null) => AppContext.BeginChildScope(name);

        /// <summary>
        /// Create a new management scope with the current scope as a parent
        /// </summary>
        /// <returns></returns>
        public SettingsContext BeginChildScope(string name = null) => new SettingsContext(name, this);

        /// <summary>
        /// Used to copy over the referenced context to a new thread.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SettingsContext Set(SettingsContext context) => new SettingsContext(context._scopeName, context);

        public bool Remove<T>(string key = null)
        {
            return _managers.Remove(ResolveManagerKey<T>(key));
        }

        public bool RemoveRecursive<T>(string key = null)
        {
            key = ResolveManagerKey<T>(key);

            var context = this;
            do
            {
                if (context._managers.Remove(key))
                    return true;
            } while ((context = context._parentContext) != null);

            return false;
        }

        public T Get<T>(string key = null)
        {
            key = ResolveManagerKey<T>(key);

            if (TryGetManager(key, out T manager))
                return manager;

            lock (_managers)
            {
                if (TryGetManager(key, out manager))
                    return manager;

                manager = SettingsManager.New<T>();
                _managers[key] = manager;

                return manager;
            }
        }

        static string ResolveManagerKey<T>(string key) => key + "_" + typeof(T).FullName;

        public bool HasManager<T>(string key = null, bool includeParents = true)
        {
            return HasManagerInternal(ResolveManagerKey<T>(key), includeParents);
        }

        bool HasManagerInternal(string key, bool includeParents)
        {
            var context = this;
            do
            {
                if (context._managers.ContainsKey(key))
                    return true;
            } while (includeParents && (context = context._parentContext) != null);

            return false;
        }

        bool TryGetManager<T>(string key, out T manager)
        {
            var context = this;

            do
            {
                if (context._managers.TryGetValue(key, out var cachedManager))
                {
                    manager = (T)cachedManager;
                    return true;
                }
            } while ((context = context._parentContext) != null);

            manager = default;
            return false;
        }
    }
}
