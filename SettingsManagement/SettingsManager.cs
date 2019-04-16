using SettingsManagement.BuildingBlocks;
using SettingsManagement.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;

namespace SettingsManagement
{
    static class SettingsManager
    {
        static readonly AssemblyBuilder _assemblyBuilder;
        static readonly ModuleBuilder _moduleBuilder;

        const string ERRORMSG = "Configuration Manager is NULL. Either supply one or set the default manager in DefaultSettings.Manager.";

        static SettingsManager()
        {
            var assemblyName = new AssemblyName("SettingsManagement.Emit");
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("SettingsManagement.Emit.Module");
        }

        public static TSettingsManager New<TSettingsManager>(IConfigurationManager manager = null)
        {
            if (manager == null)
                manager = SettingsContext.Current.Manager ?? throw new ArgumentNullException(nameof(manager), ERRORMSG);

            try
            {
                return Creator<TSettingsManager>.BuildNewInstance(manager);
            }
            catch (TypeInitializationException ex) when (ex.InnerException != null)
            {
                //Catch the exception our Creator CTOR throws and unwrap it.
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

                throw; //Won't execute but needed to keep the compiler happy.
            }
        }

        static class Creator<TSettingsManager>
        {
            public static Func<IConfigurationManager, TSettingsManager> BuildNewInstance { get; }

            /// <summary>
            /// Cached Building Delegate
            /// </summary>
            /// <remarks>
            /// <para>
            /// Static constructors are guaranteed to be run only once per application domain, before any instances of a class are created or any static members are accessed.
            /// http://msdn.microsoft.com/en-us/library/aa645612.aspx
            /// </para>
            /// <para>
            /// The implementation shown is thread safe for the initial construction, that is, no locking or null testing is required for constructing the Singleton object. 
            /// </para>
            /// <para>
            /// However, this does not mean that any use of the instance will be synchronised.There are a variety of ways that this can be done; I've shown one below.
            /// </para>
            /// </remarks>
            static Creator()
            {
                var block = _moduleBuilder.Create(typeof(TSettingsManager))
                                          .WithConstructor()
                                          .WithRefreshIfNeeded()
                                          .WithPersistIfNeeded()
                                          .WithReadableValuesIfNeeded()
                                          .WithDisposeIfNeeded()
                                          ;

                var type = block.Build();
                var ctor = type.GetConstructors().Single();
                var configurationParameter = Expression.Parameter(typeof(IConfigurationManager), "configurationManager");
                var newExpression = Expression.New(ctor, configurationParameter);
                var lambda = Expression.Lambda<Func<IConfigurationManager, TSettingsManager>>(newExpression, configurationParameter);

                BuildNewInstance = lambda.Compile();
            }
        }
    }
}
