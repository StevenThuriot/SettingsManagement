using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks;

class ConstructorBlock : DecoratorBlock
{
    public ConstructorBlock(Block block) : base(block)
    {
    }

    protected override void GenerateIl()
    {
        var constructorBuilder = Builder.DefineConstructor(MethodAttributes.Public
                                                                | MethodAttributes.SpecialName
                                                                | MethodAttributes.RTSpecialName
                                                                | MethodAttributes.HideBySig,
                                                                CallingConventions.Standard,
                                                                new[] { ConfigurationManagerField.FieldType });

        var ctrIl = constructorBuilder.GetILGenerator();

        ctrIl.EmitLdarg_0();
        ctrIl.EmitLdarg_1();
        ctrIl.Emit(OpCodes.Stfld, ConfigurationManagerField);

        foreach (var property in Properties)
        {
            var converterType = property.ConverterType;
            var defaultValue = property.DefaultValue;

            MethodInfo creationMethod;
            Type defaultValueType;
            if (defaultValue is string)
            {
                creationMethod = SettingsBuilderHelper.ResolveCreateAndParse(property.PropertyType);
                defaultValueType = typeof(string);
            }
            else
            {
                creationMethod = SettingsBuilderHelper.ResolveCreate(property.PropertyType);
                defaultValueType = property.PropertyType;
            }

            ctrIl.EmitLdarg_0();
            ctrIl.Emit(OpCodes.Ldstr, property.Name);
            ctrIl.EmitConstant(defaultValue, defaultValueType);
            ctrIl.EmitConstant(converterType);
            ctrIl.EmitLdarg_1();
            ctrIl.Emit(OpCodes.Call, creationMethod);
            ctrIl.Emit(OpCodes.Stfld, property.FieldBuilder);

            var description = property.Description;
            if (!string.IsNullOrWhiteSpace(description))
            {
                ctrIl.EmitLdarg_0();
                ctrIl.EmitFld(property.FieldBuilder);
                ctrIl.Emit(OpCodes.Ldstr, description);

                var setDescription = ConfigurationHelper.Settings.DescriptionSetter;
                ctrIl.Emit(OpCodes.Callvirt, setDescription);
            }
        }

        ctrIl.EmitRet();
    }
}
