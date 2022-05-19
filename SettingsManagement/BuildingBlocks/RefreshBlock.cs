using SettingsManagement.Interfaces;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks;

class RefreshBlock : DecoratorBlock
{
    public RefreshBlock(Block block) : base(block)
    {
    }

    protected override void GenerateIl()
    {
        BuildRefresh();
        BuildRefreshWithKey();
    }

    void BuildRefresh()
    {
        var methodBuilder = Builder.DefineMethod(nameof(ICanRefresh.Refresh), MethodAttributes.Public
                                                                 | MethodAttributes.Final
                                                                 | MethodAttributes.HideBySig
                                                                 | MethodAttributes.NewSlot
                                                                 | MethodAttributes.Virtual,
                                                                 null,
                                                                 Type.EmptyTypes);

        var mIl = methodBuilder.GetILGenerator();

        mIl.EmitLdarg_0();
        mIl.EmitFld(ConfigurationManagerField);
        mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Managers.RefreshMethod);

        foreach (var property in Properties)
        {
            mIl.EmitLdarg_0();
            mIl.EmitFld(property.FieldBuilder);
            mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.RefreshMethod);
        }

        mIl.EmitRet();
    }

    void BuildRefreshWithKey()
    {
        var methodBuilder = Builder.DefineMethod(nameof(ICanRefresh.Refresh), MethodAttributes.Public
                                                                 | MethodAttributes.Final
                                                                 | MethodAttributes.HideBySig
                                                                 | MethodAttributes.NewSlot
                                                                 | MethodAttributes.Virtual,
                                                                 null,
                                                                 new[] { typeof(string) });

        var mIl = methodBuilder.GetILGenerator();
        var throwNotFound = mIl.DefineLabel();
        var jumpTable = Enumerable.Repeat(mIl, Properties.Count).Select(x => x.DefineLabel()).ToArray();

        mIl.EmitLdarg_0();
        mIl.EmitFld(ConfigurationManagerField);
        mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Managers.RefreshMethod);

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
            mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.RefreshMethod);
            mIl.EmitRet();
        }

        mIl.MarkLabel(throwNotFound);
        mIl.EmitLdarg_1();
        mIl.Emit(OpCodes.Newobj, ConfigurationHelper.Exceptions.KeyNotFound);
        mIl.Emit(OpCodes.Throw);
    }
}
