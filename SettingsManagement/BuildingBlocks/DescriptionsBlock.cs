using SettingsManagement.Interfaces;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks;

class DescriptionsBlock : DecoratorBlock
{
    public DescriptionsBlock(Block block) : base(block)
    {
    }

    protected override void GenerateIl()
    {
        BuildDescriptions();
        BuildDescriptionWithKey();
    }

    private void BuildDescriptions()
    {
        var methodBuilder = Builder.DefineMethod(nameof(IAmDescriptive.GetDescriptions), MethodAttributes.Public
                                                                             | MethodAttributes.Final
                                                                             | MethodAttributes.HideBySig
                                                                             | MethodAttributes.NewSlot
                                                                             | MethodAttributes.Virtual,
                                                                             typeof(IReadOnlyDictionary<string, string>),
                                                                             Type.EmptyTypes
                                                                             );

        var mIl = methodBuilder.GetILGenerator();

        mIl.EmitNew(ConfigurationHelper.StringDictionaries.DefaultCtor);

        foreach (var property in Properties)
        {
            mIl.Emit(OpCodes.Dup);
            mIl.EmitString(property.Name);
            mIl.EmitLdarg_0();
            mIl.EmitFld(property.FieldBuilder);
            mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.DescriptionGetter);
            mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.StringDictionaries.SetIndexer);
        }

        mIl.EmitRet();
    }

    private void BuildDescriptionWithKey()
    {
        var methodBuilder = Builder.DefineMethod(nameof(IAmDescriptive.GetDescription), MethodAttributes.Public
                                                                             | MethodAttributes.Final
                                                                             | MethodAttributes.HideBySig
                                                                             | MethodAttributes.NewSlot
                                                                             | MethodAttributes.Virtual,
                                                                             typeof(string),
                                                                             new[] { typeof(string) });

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
            mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.DescriptionGetter);
            mIl.EmitRet();
        }

        mIl.MarkLabel(throwNotFound);
        mIl.EmitLdarg_1();
        mIl.EmitNew(ConfigurationHelper.Exceptions.KeyNotFound);
        mIl.Emit(OpCodes.Throw);
    }
}
