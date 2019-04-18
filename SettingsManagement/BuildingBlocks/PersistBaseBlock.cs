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
                mIl.EmitLdarg_0();
                mIl.EmitFld(property.FieldBuilder);
                mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.PersistMethod);
            }

            mIl.EmitLdarg_0();
            mIl.EmitFld(ConfigurationManagerField);
            mIl.Emit(OpCodes.Call, ConfigurationHelper.Managers.PersistMethod);

            mIl.EmitRet();
        }
    }
}
