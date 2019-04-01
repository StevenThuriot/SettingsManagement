using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement
{
    static class ILGen
    {
        public static void EmitNew(this ILGenerator il, ConstructorInfo ci)
        {
            if (ci is null)
                throw new ArgumentNullException(nameof(ci));

            if (ci.DeclaringType.ContainsGenericParameters)
            {
                throw new Exception("Illegal New Generic Params: " + ci.DeclaringType);
            }

            il.Emit(OpCodes.Newobj, ci);
        }

        public static void EmitString(this ILGenerator il, string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            il.Emit(OpCodes.Ldstr, value);
        }

        public static void EmitType(this ILGenerator il, Type value)
        {
            if (value == null)
            {
                il.Emit(OpCodes.Ldnull);
            }
            else
            {
                il.Emit(OpCodes.Ldtoken, value);
            }
        }

        public static void EmitBoolean(this ILGenerator il, bool value)
        {
            if (value)
            {
                il.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4_0);
            }
        }

        public static void EmitChar(this ILGenerator il, char value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_U2);
        }

        public static void EmitByte(this ILGenerator il, byte value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_U1);
        }

        public static void EmitSByte(this ILGenerator il, sbyte value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_I1);
        }

        public static void EmitShort(this ILGenerator il, short value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_I2);
        }

        public static void EmitUShort(this ILGenerator il, ushort value)
        {
            il.EmitInt(value);
            il.Emit(OpCodes.Conv_U2);
        }

        public static void EmitInt(this ILGenerator il, int value)
        {
            OpCode c;
            switch (value)
            {
                case -1:
                    c = OpCodes.Ldc_I4_M1;
                    break;
                case 0:
                    c = OpCodes.Ldc_I4_0;
                    break;
                case 1:
                    c = OpCodes.Ldc_I4_1;
                    break;
                case 2:
                    c = OpCodes.Ldc_I4_2;
                    break;
                case 3:
                    c = OpCodes.Ldc_I4_3;
                    break;
                case 4:
                    c = OpCodes.Ldc_I4_4;
                    break;
                case 5:
                    c = OpCodes.Ldc_I4_5;
                    break;
                case 6:
                    c = OpCodes.Ldc_I4_6;
                    break;
                case 7:
                    c = OpCodes.Ldc_I4_7;
                    break;
                case 8:
                    c = OpCodes.Ldc_I4_8;
                    break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    return;
            }
            il.Emit(c);
        }

        public static void EmitUInt(this ILGenerator il, uint value)
        {
            il.EmitInt((int)value);
            il.Emit(OpCodes.Conv_U4);
        }

        public static void EmitLong(this ILGenerator il, long value)
        {
            il.Emit(OpCodes.Ldc_I8, value);

            //
            // Now, emit convert to give the constant type information.
            //
            // Otherwise, it is treated as unsigned and overflow is not
            // detected if it's used in checked ops.
            //
            il.Emit(OpCodes.Conv_I8);
        }

        public static void EmitULong(this ILGenerator il, ulong value)
        {
            il.Emit(OpCodes.Ldc_I8, (long)value);
            il.Emit(OpCodes.Conv_U8);
        }

        public static void EmitDouble(this ILGenerator il, double value)
        {
            il.Emit(OpCodes.Ldc_R8, value);
        }

        public static void EmitSingle(this ILGenerator il, float value)
        {
            il.Emit(OpCodes.Ldc_R4, value);
        }
        
        public static void EmitConstant(this ILGenerator il, object value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            il.EmitConstant(value, value.GetType());
        }

        public static void EmitConstant(this ILGenerator il, Type type)
        {
            il.EmitType(type);
        }

        public static void EmitConstant(this ILGenerator il, object value, Type type)
        {
            if (value == null)
            {
                // Smarter than the Linq implementation which uses the initobj
                // pattern for all value types (works, but requires a local and
                // more IL)
                il.EmitDefault(type);
            }
            else if (!il.TryEmitILConstant(value, type))
            {
                throw new NotSupportedException($"Constant type {type} is not supported.");
            }
        }

        static bool TryEmitILConstant(this ILGenerator il, object value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    il.EmitBoolean((bool)value);
                    return true;
                case TypeCode.SByte:
                    il.EmitSByte((sbyte)value);
                    return true;
                case TypeCode.Int16:
                    il.EmitShort((short)value);
                    return true;
                case TypeCode.Int32:
                    il.EmitInt((int)value);
                    return true;
                case TypeCode.Int64:
                    il.EmitLong((long)value);
                    return true;
                case TypeCode.Single:
                    il.EmitSingle((float)value);
                    return true;
                case TypeCode.Double:
                    il.EmitDouble((double)value);
                    return true;
                case TypeCode.Char:
                    il.EmitChar((char)value);
                    return true;
                case TypeCode.Byte:
                    il.EmitByte((byte)value);
                    return true;
                case TypeCode.UInt16:
                    il.EmitUShort((ushort)value);
                    return true;
                case TypeCode.UInt32:
                    il.EmitUInt((uint)value);
                    return true;
                case TypeCode.UInt64:
                    il.EmitULong((ulong)value);
                    return true;
                case TypeCode.Decimal:
                    il.EmitDecimal((decimal)value);
                    return true;
                case TypeCode.String:
                    il.EmitString((string)value);
                    return true;
                default:
                    return false;
            }
        }

        public static void EmitDecimal(this ILGenerator il, decimal value)
        {
            if (decimal.Truncate(value) == value)
            {
                if (int.MinValue <= value && value <= int.MaxValue)
                {
                    int intValue = decimal.ToInt32(value);
                    il.EmitInt(intValue);
                    il.EmitNew(typeof(decimal).GetConstructor(new Type[] { typeof(int) }));
                }
                else if (long.MinValue <= value && value <= long.MaxValue)
                {
                    long longValue = decimal.ToInt64(value);
                    il.EmitLong(longValue);
                    il.EmitNew(typeof(decimal).GetConstructor(new Type[] { typeof(long) }));
                }
                else
                {
                    il.EmitDecimalBits(value);
                }
            }
            else
            {
                il.EmitDecimalBits(value);
            }
        }

        static void EmitDecimalBits(this ILGenerator il, decimal value)
        {
            int[] bits = decimal.GetBits(value);
            il.EmitInt(bits[0]);
            il.EmitInt(bits[1]);
            il.EmitInt(bits[2]);
            il.EmitBoolean((bits[3] & 0x80000000) != 0);
            il.EmitByte((byte)(bits[3] >> 16));
            il.EmitNew(typeof(decimal).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) }));
        }

        public static void EmitDefault(this ILGenerator il, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                case TypeCode.DateTime:
                    if (type.IsValueType)
                    {
                        // Type.GetTypeCode on an enum returns the underlying
                        // integer TypeCode, so we won't get here.
                        Debug.Assert(!type.IsEnum);

                        // This is the IL for default(T) if T is a generic type
                        // parameter, so it should work for any type. It's also
                        // the standard pattern for structs.
                        LocalBuilder lb = il.DeclareLocal(type);
                        il.Emit(OpCodes.Ldloca, lb);
                        il.Emit(OpCodes.Initobj, type);
                        il.Emit(OpCodes.Ldloc, lb);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldnull);
                    }
                    break;

                case TypeCode.Empty:
                case TypeCode.String:
                case TypeCode.DBNull:
                    il.Emit(OpCodes.Ldnull);
                    break;

                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Conv_I8);
                    break;

                case TypeCode.Single:
                    il.Emit(OpCodes.Ldc_R4, default(float));
                    break;

                case TypeCode.Double:
                    il.Emit(OpCodes.Ldc_R8, default(double));
                    break;

                case TypeCode.Decimal:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[] { typeof(int) }));
                    break;

                default:
                    throw new InvalidOperationException("Code supposed to be unreachable");
            }
        }
    }
}