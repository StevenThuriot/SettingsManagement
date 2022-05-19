using SettingsManagement.Interfaces;

namespace SettingsManagement;

/// <summary>
/// Manager Scope Context
/// </summary>
public sealed partial class SettingsContext : IDisposable
{
    bool _disposed;
    readonly string _scopeName;
    readonly SettingsContext _parentContext;
    readonly SimpleSettingsStack _myStack;
    readonly IDictionary<string, object> _managers;

    /// <summary>
    /// Default Manager
    /// </summary>
    public IConfigurationManager Manager
    {
        get => _defaultConfigurationManager ?? _parentContext?.Manager;
        set => _defaultConfigurationManager = value;
    }
    IConfigurationManager _defaultConfigurationManager;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => _scopeName;

    SettingsContext() //Only for app-wide context
    {
        _scopeName = nameof(AppContext);
        _parentContext = null;
        _defaultConfigurationManager = new InMemoryManager();
        _managers = new Dictionary<string, object>();
        (_myStack = _localScopeStack.Value).Push(this);
    }

    SettingsContext(string scopeName, SettingsContext parentContext, IConfigurationManager configurationManager)
    {
        _scopeName = "Scope: " + scopeName ?? "Unnamed";
        _parentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));
        _defaultConfigurationManager = configurationManager;
        _managers = new Dictionary<string, object>();
        (_myStack = _localScopeStack.Value).Push(this);
    }

    /// <summary>
    /// Context Finalizer
    /// </summary>
    ~SettingsContext()
    {
        Dispose(false);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
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

    static readonly ThreadLocal<SimpleSettingsStack> _localScopeStack = new(() => new SimpleSettingsStack());

    /// <summary>
    /// Get the current scope. If not scoped, the Application wide one will be returned.
    /// </summary>
    public static SettingsContext Current => _localScopeStack.Value.Peek();

    /// <summary>
    /// Application-wide context scope
    /// </summary>
    public static readonly SettingsContext AppContext = new();

    /// <summary>
    /// Creates a new management scope
    /// </summary>
    /// <param name="name">The scope name</param>
    /// <param name="configurationManager">The scopes manager</param>
    /// <returns></returns>
    public static SettingsContext BeginScope(string name = null, IConfigurationManager configurationManager = null) => AppContext.BeginChildScope(name, configurationManager);

    /// <summary>
    /// Creates a new management scope
    /// </summary>
    /// <param name="configurationManager">The scopes manager</param>
    /// <returns></returns>
    public static SettingsContext BeginScope(IConfigurationManager configurationManager) => BeginScope(null, configurationManager);

    /// <summary>
    /// Creates a new management scope
    /// </summary>
    /// <typeparam name="TConfigurationManager">The type of manager</typeparam>
    /// <param name="name">The scope name</param>
    /// <returns></returns>
    public static SettingsContext BeginScope<TConfigurationManager>(string name = null)
        where TConfigurationManager : IConfigurationManager, new()
        => BeginScope(name, new TConfigurationManager());

    /// <summary>
    /// Create a new management scope with the current scope as a parent
    /// </summary>
    /// <returns></returns>
    public SettingsContext BeginChildScope(string name = null, IConfigurationManager configurationManager = null) => new(name, this, configurationManager);

    /// <summary>
    /// Create a new management scope with the current scope as a parent
    /// </summary>
    /// <returns></returns>
    public SettingsContext BeginChildScope(IConfigurationManager configurationManager) => BeginChildScope(null, configurationManager);

    /// <summary>
    /// Used to copy over the referenced context to a new thread.
    /// </summary>
    /// <param name="context">The context to use for the new thread.</param>
    /// <returns></returns>
    public static SettingsContext Set(SettingsContext context) => new(context._scopeName, context, context._defaultConfigurationManager);

    /// <summary>
    /// Removes a manager from current scope.
    /// </summary>
    /// <typeparam name="T">The type of manager to remove</typeparam>
    /// <param name="key">The key of the manager (null is an option as well)</param>
    /// <returns>True if the manager is removed</returns>
    public bool Remove<T>(string key = null)
    {
        return _managers.Remove(ResolveManagerKey<T>(key));
    }

    /// <summary>
    /// Removes a manager from current and parent scopes.
    /// </summary>
    /// <typeparam name="T">The type of manager to remove</typeparam>
    /// <param name="key">The key of the manager (null is an option as well)</param>
    /// <returns>True if the manager is removed</returns>
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

    /// <summary>
    /// Gets or creates a manager from current or parent scopes, given a certain key and/or manager.
    /// </summary>
    /// <typeparam name="T">The Type of SettingsManager</typeparam>
    /// <param name="key">The manager unique identifying key</param>
    /// <param name="configurationManager">The internal configuration manager used.</param>
    /// <returns></returns>
    public T Get<T>(string key = null, IConfigurationManager configurationManager = null)
    {
        key = ResolveManagerKey<T>(key);

        if (TryGetManager(key, out T manager))
            return manager;

        lock (_managers)
        {
            if (TryGetManager(key, out manager))
                return manager;

            manager = SettingsManager.New<T>(configurationManager);
            _managers[key] = manager;

            return manager;
        }
    }

    static string ResolveManagerKey<T>(string key) => key + "_" + typeof(T).FullName;

    /// <summary>
    /// Checks wether a manager has been created yet.
    /// </summary>
    /// <typeparam name="T">The Type of SettingsManager</typeparam>
    /// <param name="key">The manager unique identifying key</param>
    /// <param name="includeParents">Wether it's recursive or not</param>
    /// <returns>True if the manager has an existing instance</returns>
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
