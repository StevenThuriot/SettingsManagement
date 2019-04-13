using SettingsManagement.Interfaces;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    class RefreshBlock : DecoratorBlock
    {
        public RefreshBlock(Block block) : base(block)
        {
        }

        protected override void GenerateIl()
        {
            var methodBuilder = Builder.DefineMethod("Refresh", MethodAttributes.Public
                                                                     | MethodAttributes.Final
                                                                     | MethodAttributes.HideBySig
                                                                     | MethodAttributes.NewSlot
                                                                     | MethodAttributes.Virtual);

            var mIl = methodBuilder.GetILGenerator();

            mIl.Emit(OpCodes.Ldarg_0);
            mIl.Emit(OpCodes.Ldfld, ConfigurationManagerField);
            mIl.Emit(OpCodes.Call, ConfigurationHelper.Managers.RefreshMethod);

            foreach (var property in Properties)
            {
                mIl.Emit(OpCodes.Ldarg_0);
                mIl.Emit(OpCodes.Ldfld, property.FieldBuilder);
                mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.RefreshMethod);
            }

            mIl.Emit(OpCodes.Ret);
        }
    }
}
