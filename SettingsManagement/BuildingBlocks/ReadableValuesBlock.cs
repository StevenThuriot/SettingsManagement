using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    class ReadableValuesBlock : DecoratorBlock
    {
        public ReadableValuesBlock(Block block) : base(block)
        {
        }

        protected override void GenerateIl()
        {
            var properties = Properties;
            var readableValuesMethodBuilder = Builder.DefineMethod("GetReadableValues", MethodAttributes.Public
                                                                     | MethodAttributes.Final
                                                                     | MethodAttributes.HideBySig
                                                                     | MethodAttributes.NewSlot
                                                                     | MethodAttributes.Virtual,
                                                                     typeof(IEnumerable<string>),
                                                                     Type.EmptyTypes);

            var rvmIl = readableValuesMethodBuilder.GetILGenerator();
            rvmIl.EmitInt(properties.Count);
            rvmIl.Emit(OpCodes.Newarr, typeof(string));

            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];

                rvmIl.Emit(OpCodes.Dup);
                rvmIl.EmitInt(i);
                rvmIl.Emit(OpCodes.Ldarg_0);
                rvmIl.Emit(OpCodes.Ldfld, property.FieldBuilder);
                rvmIl.Emit(OpCodes.Callvirt, property.BackingFieldType.GetMethod("GetReadableValue", Type.EmptyTypes));
                rvmIl.Emit(OpCodes.Stelem_Ref);
            }

            rvmIl.Emit(OpCodes.Ret);

            var toStringMethodBuilder = Builder.DefineMethod("ToString", MethodAttributes.Public
                                                                                | MethodAttributes.HideBySig
                                                                                | MethodAttributes.NewSlot
                                                                                | MethodAttributes.Virtual
                                                                                | MethodAttributes.Final,
                                                                                CallingConventions.HasThis,
                                                                                typeof(string),
                                                                                Type.EmptyTypes);

            Builder.DefineMethodOverride(toStringMethodBuilder, typeof(object).GetMethod("ToString"));

            var tsmIl = toStringMethodBuilder.GetILGenerator();

            tsmIl.EmitString(Builder.Name + " { ");
            tsmIl.EmitString(", ");
            tsmIl.Emit(OpCodes.Ldarg_0);
            tsmIl.Emit(OpCodes.Call, readableValuesMethodBuilder);
            tsmIl.Emit(OpCodes.Call, typeof(string).GetMethod("Join", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(IEnumerable<string>) }, null));
            tsmIl.EmitString(" }");
            tsmIl.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null));
            tsmIl.Emit(OpCodes.Ret);
        }
    }
}
