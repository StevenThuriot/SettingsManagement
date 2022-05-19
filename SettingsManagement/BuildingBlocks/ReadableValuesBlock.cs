using SettingsManagement.Interfaces;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks;

class ReadableValuesBlock : DecoratorBlock
{
    public ReadableValuesBlock(Block block) : base(block)
    {
    }

    protected override void GenerateIl()
    {
        var properties = Properties;
        var readableValuesMethodBuilder = Builder.DefineMethod(nameof(ICanShowMyValues.GetReadableValues), MethodAttributes.Public
                                                                 | MethodAttributes.Final
                                                                 | MethodAttributes.HideBySig
                                                                 | MethodAttributes.NewSlot
                                                                 | MethodAttributes.Virtual,
                                                                 typeof(IEnumerable<string>),
                                                                 Type.EmptyTypes);

        var rvmIl = readableValuesMethodBuilder.GetILGenerator();
        rvmIl.EmitInt(properties.Count);
        rvmIl.EmitNewArr(typeof(string));

        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties[i];

            rvmIl.Emit(OpCodes.Dup);
            rvmIl.EmitInt(i);
            rvmIl.EmitLdarg_0();
            rvmIl.EmitFld(property.FieldBuilder);
            rvmIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Settings.ReadableValueMethod);
            rvmIl.Emit(OpCodes.Stelem_Ref);
        }

        rvmIl.EmitRet();

        var toStringMethodBuilder = Builder.DefineMethod(nameof(string.ToString), MethodAttributes.Public
                                                                            | MethodAttributes.HideBySig
                                                                            | MethodAttributes.NewSlot
                                                                            | MethodAttributes.Virtual
                                                                            | MethodAttributes.Final,
                                                                            CallingConventions.HasThis,
                                                                            typeof(string),
                                                                            Type.EmptyTypes);

        Builder.DefineMethodOverride(toStringMethodBuilder, ConfigurationHelper.Strings.ToStringMethod);

        var tsmIl = toStringMethodBuilder.GetILGenerator();

        tsmIl.EmitString(Builder.Name + " { ");
        tsmIl.EmitString(", ");
        tsmIl.EmitLdarg_0();
        tsmIl.Emit(OpCodes.Call, readableValuesMethodBuilder);
        tsmIl.Emit(OpCodes.Call, ConfigurationHelper.Strings.JoinMethod);
        tsmIl.EmitString(" }");
        tsmIl.Emit(OpCodes.Call, ConfigurationHelper.Strings.ConcatMethod);
        tsmIl.EmitRet();
    }
}
