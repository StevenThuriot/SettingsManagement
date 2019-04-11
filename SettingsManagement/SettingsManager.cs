using SettingsManagement.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement
{
    public static class SettingsManager
    {
        static readonly AssemblyBuilder _assemblyBuilder;
        static readonly ModuleBuilder _moduleBuilder;

        static SettingsManager()
        {
            var assemblyName = new AssemblyName("SettingsManagement.Emit");
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("SettingsManagement.Emit.Module");
        }

        public static T New<T>() => Creator<T>.BuildNewInstance();

        static class Creator<T>
        {
            public static Func<T> BuildNewInstance { get; }

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
                var block = _moduleBuilder.Create(typeof(T))
                                          .WithConstructor()
                                          .WithRefreshIfNeeded()
                                          .WithPersistIfNeeded()
                                          .WithReadableValuesIfNeeded()
                                          .WithDisposeIfNeeded();

                var type = block.Build();
                var newExpression = Expression.New(type.GetConstructors().Single());
                var lambda = Expression.Lambda<Func<T>>(newExpression);

                BuildNewInstance = lambda.Compile();
            }
        }
    }
}
