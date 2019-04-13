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

            foreach (var property in Properties)
            {
                mIl.Emit(OpCodes.Ldarg_0);
                mIl.Emit(OpCodes.Ldfld, property.FieldBuilder);
                mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.PersistMethod);
            }

            mIl.Emit(OpCodes.Ldarg_0);
            mIl.Emit(OpCodes.Ldfld, ConfigurationManagerField);
            mIl.Emit(OpCodes.Call, ConfigurationHelper.Managers.PersistMethod);

            mIl.Emit(OpCodes.Ret);
        }
    }
}
