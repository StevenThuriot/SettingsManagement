using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    class ResetBlock : DecoratorBlock
    {
        public ResetBlock(Block block) : base(block)
        {
        }

        protected override void GenerateIl()
        {
            BuildReset();
            BuildResetWithKey();
        }

        void BuildReset()
        {
            var methodBuilder = Builder.DefineMethod(nameof(ICanReset.Reset), MethodAttributes.Public
                                                                                 | MethodAttributes.Final
                                                                                 | MethodAttributes.HideBySig
                                                                                 | MethodAttributes.NewSlot
                                                                                 | MethodAttributes.Virtual,
                                                                                 null,
                                                                                 Type.EmptyTypes);

            var mIl = methodBuilder.GetILGenerator();

            foreach (var property in Properties)
            {
                mIl.EmitLdarg_0();
                mIl.EmitFld(property.FieldBuilder);
                mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.ResetMethod);
            }

            mIl.EmitRet();
        }

        void BuildResetWithKey()
        {
            var methodBuilder = Builder.DefineMethod(nameof(ICanReset.Reset), MethodAttributes.Public
                                                                                 | MethodAttributes.Final
                                                                                 | MethodAttributes.HideBySig
                                                                                 | MethodAttributes.NewSlot
                                                                                 | MethodAttributes.Virtual,
                                                                                 null,
                                                                                 new [] { typeof(string) });

            var mIl = methodBuilder.GetILGenerator();
            var throwNotFound = mIl.DefineLabel();
            var jumpTable = Enumerable.Repeat(mIl, Properties.Count).Select(x => x.DefineLabel()).ToArray();

            mIl.EmitLdarg_1();
            mIl.Emit(OpCodes.Brfalse_S, throwNotFound);

            for (int i = 0; i < Properties.Count; i++)
            {
                mIl.EmitLdarg_1();
                mIl.EmitString(Properties[i].Name);
                mIl.Emit(OpCodes.Call, ConfigurationHelper.Strings.Equality);
                mIl.Emit(OpCodes.Brtrue_S, jumpTable[i]);
            }

            mIl.Emit(OpCodes.Br_S, throwNotFound);

            for (int i = 0; i < Properties.Count; i++)
            {
                mIl.MarkLabel(jumpTable[i]);
                mIl.EmitLdarg_0();
                mIl.EmitFld(Properties[i].FieldBuilder);
                mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.ResetMethod);
                mIl.EmitRet();
            }

            mIl.MarkLabel(throwNotFound);
            mIl.EmitLdarg_1();
            mIl.Emit(OpCodes.Newobj, ConfigurationHelper.Exceptions.KeyNotFound);
            mIl.Emit(OpCodes.Throw);
        }
    }
}
