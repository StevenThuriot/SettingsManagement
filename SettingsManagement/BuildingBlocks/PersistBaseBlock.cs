using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    class PersistBaseBlock : DecoratorBlock
    {
        readonly string _methodName;

        public PersistBaseBlock(Block block, string methodName) : base(block)
        {
            _methodName = methodName;
        }

        protected override void GenerateIl()
        {
            var methodBuilder = Builder.DefineMethod(_methodName, MethodAttributes.Public
                                                                     | MethodAttributes.Final
                                                                     | MethodAttributes.HideBySig
                                                                     | MethodAttributes.NewSlot
                                                                     | MethodAttributes.Virtual);

            var mIl = methodBuilder.GetILGenerator();

            var configuration = mIl.DeclareLocal(ConfigurationHelper.OpenConfigurationMethod.ReturnType);

            mIl.Emit(OpCodes.Call, ConfigurationHelper.OpenConfigurationMethod);
            mIl.Emit(OpCodes.Stloc, configuration);

            foreach (var property in Properties)
            {
                mIl.Emit(OpCodes.Ldarg_0);
                mIl.Emit(OpCodes.Ldfld, property.FieldBuilder);
                mIl.Emit(OpCodes.Ldloc_0);
                mIl.Emit(OpCodes.Callvirt, property.BackingFieldType.GetMethod("Persist", new Type[] { ConfigurationHelper.OpenConfigurationMethod.ReturnType }));
            }

            mIl.Emit(OpCodes.Ldloc_0);
            mIl.Emit(OpCodes.Call, ConfigurationHelper.PersistMethod);

            mIl.Emit(OpCodes.Ret);
        }
    }
}
