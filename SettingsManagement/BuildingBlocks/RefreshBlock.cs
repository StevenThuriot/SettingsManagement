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

            mIl.Emit(OpCodes.Call, ConfigurationHelper.RefreshAppSettingsMethod);

            foreach (var property in Properties)
            {
                mIl.Emit(OpCodes.Ldarg_0);
                mIl.Emit(OpCodes.Ldfld, property.FieldBuilder);
                mIl.Emit(OpCodes.Callvirt, property.BackingFieldType.GetMethod("Refresh", Type.EmptyTypes));
            }

            mIl.Emit(OpCodes.Ret);
        }
    }
}
