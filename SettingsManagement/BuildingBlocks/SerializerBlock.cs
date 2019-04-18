using SettingsManagement.Attributes;
using SettingsManagement.Formatters;
using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SettingsManagement.BuildingBlocks
{
    class SerializerBlock : DecoratorBlock
    {
        public SerializerBlock(Block block) : base(block)
        {
        }

        protected override void GenerateIl()
        {
            var methodBuilder = Builder.DefineMethod(nameof(ICanSerialize.Serialize), MethodAttributes.Public
                                                                     | MethodAttributes.Final
                                                                     | MethodAttributes.HideBySig
                                                                     | MethodAttributes.NewSlot
                                                                     | MethodAttributes.Virtual,
                                                                     typeof(string),
                                                                     Type.EmptyTypes);

            var mIl = methodBuilder.GetILGenerator();

            mIl.DeclareLocal(typeof(ISetting[]));
            mIl.EmitNewArr(typeof(ISetting), Properties.Count);
            mIl.Emit(OpCodes.Dup);

            for (int i = 0; i < Properties.Count; i++)
            {
                mIl.EmitInt(i);
                mIl.EmitLdarg_0();
                mIl.EmitFld(Properties[i].FieldBuilder);
                mIl.Emit(OpCodes.Stelem_Ref);

                if ((i + 1) < Properties.Count)
                {
                    mIl.Emit(OpCodes.Dup);
                }
                else
                {
                    mIl.EmitStloc_0();
                }
            }

            mIl.EmitNew(Interface.GetCustomAttribute<SettingsSerializerAttribute>()?.Constructor ?? JsonArraySerializer.Constructor);

            //var customSerializerInterface = Interface.GetInterface(typeof(ICanSerializeWith<>).FullName);
            //var serializerType = customSerializerInterface?.GetGenericArguments()[0] ?? typeof(JsonArraySerializer);
            //mIl.EmitNew(serializerType.GetConstructor(Type.EmptyTypes));

            mIl.EmitLdloc_0();
            mIl.Emit(OpCodes.Callvirt, ConfigurationHelper.Serializers.SerializeMethod);
            mIl.EmitRet();
        }
    }
}
