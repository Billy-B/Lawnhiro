using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class QueryProvider : IQueryProvider
    {
        //internal static readonly QueryProvider Instance = new QueryProvider();

        private class ReduceExpressionVisitor : ExpressionVisitor
        {
            private static readonly MethodInfo _getDefaultValueGeneric = typeof(ReduceExpressionVisitor).GetMethod("getDefaultValueGeneric", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            private static readonly MethodInfo _objAsGeneric = typeof(ReduceExpressionVisitor).GetMethod("objAs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            private static readonly MethodInfo _objIsGeneric = typeof(ReduceExpressionVisitor).GetMethod("objIs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            private ConditionalWeakTable<Expression, Expression> _reducedExpressionMapper = new ConditionalWeakTable<Expression, Expression>();

            private static readonly Type _enumerableQueryType = Type.GetType("System.Linq.EnumerableQuery`1, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            
            private static object computeBinaryOperation(object left, object right, ExpressionType operation)
            {
                TypeCode type;
                if (left == null)
                {
                    type = TypeCode.Empty;
                }
                else
                {
                    type = Type.GetTypeCode(left.GetType());
                }
                switch (operation)
                {
                    case ExpressionType.Add:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left + (byte)right;
                            case TypeCode.Char:
                                return (char)left + (char)right;
                            case TypeCode.Double:
                                return (double)left + (double)right;
                            case TypeCode.Int16:
                                return (short)left + (short)right;
                            case TypeCode.Int32:
                                return (int)left + (int)right;
                            case TypeCode.Int64:
                                return (long)left + (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left + (sbyte)right;
                            case TypeCode.Single:
                                return (float)left + (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left + (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left + (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left + (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.And:
                        switch (type)
                        {
                            case TypeCode.Boolean:
                                return (bool)left & (bool)right;
                            case TypeCode.Byte:
                                return (byte)left & (byte)right;
                            case TypeCode.Char:
                                return (char)left & (char)right;
                            case TypeCode.Int16:
                                return (short)left & (short)right;
                            case TypeCode.Int32:
                                return (int)left & (int)right;
                            case TypeCode.Int64:
                                return (long)left & (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left & (sbyte)right;
                            case TypeCode.UInt16:
                                return (ushort)left & (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left & (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left & (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.AndAlso:
                        return (bool)left && (bool)right;
                    case ExpressionType.Divide:
                        try
                        {
                            switch (type)
                            {
                                case TypeCode.Byte:
                                    return (byte)left / (byte)right;
                                case TypeCode.Char:
                                    return (char)left / (char)right;
                                case TypeCode.Double:
                                    return (double)left / (double)right;
                                case TypeCode.Int16:
                                    return (short)left / (short)right;
                                case TypeCode.Int32:
                                    return (int)left / (int)right;
                                case TypeCode.Int64:
                                    return (long)left / (long)right;
                                case TypeCode.SByte:
                                    return (sbyte)left / (sbyte)right;
                                case TypeCode.Single:
                                    return (float)left / (float)right;
                                case TypeCode.UInt16:
                                    return (ushort)left / (ushort)right;
                                case TypeCode.UInt32:
                                    return (uint)left / (uint)right;
                                case TypeCode.UInt64:
                                    return (ulong)left / (ulong)right;
                                default:
                                    throw new InvalidOperationException();
                            }
                        }
                        catch (DivideByZeroException)
                        {
                            throw new DivideByZeroException("Right side of division expression reduces to zero.");
                        }
                    case ExpressionType.Equal:
                        switch (type)
                        {
                            case TypeCode.Boolean:
                                return (bool)left == (bool)right;
                            case TypeCode.Byte:
                                return (byte)left == (byte)right;
                            case TypeCode.Char:
                                return (char)left == (char)right;
                            case TypeCode.Double:
                                return (double)left == (double)right;
                            case TypeCode.Int16:
                                return (short)left == (short)right;
                            case TypeCode.Int32:
                                return (int)left == (int)right;
                            case TypeCode.Int64:
                                return (long)left == (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left == (sbyte)right;
                            case TypeCode.Single:
                                return (float)left == (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left == (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left == (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left == (ulong)right;
                            default:
                                return left == right;
                        }
                    case ExpressionType.ExclusiveOr:
                        switch (type)
                        {
                            case TypeCode.Boolean:
                                return (bool)left ^ (bool)right;
                            case TypeCode.Byte:
                                return (byte)left ^ (byte)right;
                            case TypeCode.Char:
                                return (char)left ^ (char)right;
                            case TypeCode.Int16:
                                return (short)left ^ (short)right;
                            case TypeCode.Int32:
                                return (int)left ^ (int)right;
                            case TypeCode.Int64:
                                return (long)left ^ (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left ^ (sbyte)right;
                            case TypeCode.UInt16:
                                return (ushort)left ^ (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left ^ (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left ^ (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.GreaterThan:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left > (byte)right;
                            case TypeCode.Char:
                                return (char)left > (char)right;
                            case TypeCode.Double:
                                return (double)left > (double)right;
                            case TypeCode.Int16:
                                return (short)left > (short)right;
                            case TypeCode.Int32:
                                return (int)left > (int)right;
                            case TypeCode.Int64:
                                return (long)left > (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left > (sbyte)right;
                            case TypeCode.Single:
                                return (float)left > (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left > (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left > (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left > (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.GreaterThanOrEqual:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left >= (byte)right;
                            case TypeCode.Char:
                                return (char)left >= (char)right;
                            case TypeCode.Double:
                                return (double)left >= (double)right;
                            case TypeCode.Int16:
                                return (short)left >= (short)right;
                            case TypeCode.Int32:
                                return (int)left >= (int)right;
                            case TypeCode.Int64:
                                return (long)left >= (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left >= (sbyte)right;
                            case TypeCode.Single:
                                return (float)left >= (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left >= (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left >= (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left >= (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.LeftShift:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left << (int)right;
                            case TypeCode.Char:
                                return (char)left << (int)right;
                            case TypeCode.Int16:
                                return (short)left << (int)right;
                            case TypeCode.Int32:
                                return (int)left << (int)right;
                            case TypeCode.Int64:
                                return (long)left << (int)right;
                            case TypeCode.SByte:
                                return (sbyte)left << (int)right;
                            case TypeCode.UInt16:
                                return (ushort)left << (int)right;
                            case TypeCode.UInt32:
                                return (uint)left << (int)right;
                            case TypeCode.UInt64:
                                return (ulong)left << (int)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.LessThan:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left < (byte)right;
                            case TypeCode.Char:
                                return (char)left < (char)right;
                            case TypeCode.Double:
                                return (double)left < (double)right;
                            case TypeCode.Int16:
                                return (short)left < (short)right;
                            case TypeCode.Int32:
                                return (int)left < (int)right;
                            case TypeCode.Int64:
                                return (long)left < (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left < (sbyte)right;
                            case TypeCode.Single:
                                return (float)left < (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left < (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left < (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left < (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.LessThanOrEqual:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left <= (byte)right;
                            case TypeCode.Char:
                                return (char)left <= (char)right;
                            case TypeCode.Double:
                                return (double)left <= (double)right;
                            case TypeCode.Int16:
                                return (short)left <= (short)right;
                            case TypeCode.Int32:
                                return (int)left <= (int)right;
                            case TypeCode.Int64:
                                return (long)left <= (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left <= (sbyte)right;
                            case TypeCode.Single:
                                return (float)left <= (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left <= (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left <= (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left <= (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.Modulo:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left % (byte)right;
                            case TypeCode.Char:
                                return (char)left % (char)right;
                            case TypeCode.Double:
                                return (double)left % (double)right;
                            case TypeCode.Int16:
                                return (short)left % (short)right;
                            case TypeCode.Int32:
                                return (int)left % (int)right;
                            case TypeCode.Int64:
                                return (long)left % (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left % (sbyte)right;
                            case TypeCode.Single:
                                return (float)left % (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left % (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left % (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left % (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.Multiply:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left * (byte)right;
                            case TypeCode.Char:
                                return (char)left * (char)right;
                            case TypeCode.Double:
                                return (double)left * (double)right;
                            case TypeCode.Int16:
                                return (short)left * (short)right;
                            case TypeCode.Int32:
                                return (int)left * (int)right;
                            case TypeCode.Int64:
                                return (long)left * (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left * (sbyte)right;
                            case TypeCode.Single:
                                return (float)left * (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left * (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left * (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left * (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.NotEqual:
                        switch (type)
                        {
                            case TypeCode.Boolean:
                                return (bool)left != (bool)right;
                            case TypeCode.Byte:
                                return (byte)left != (byte)right;
                            case TypeCode.Char:
                                return (char)left != (char)right;
                            case TypeCode.Double:
                                return (double)left != (double)right;
                            case TypeCode.Int16:
                                return (short)left != (short)right;
                            case TypeCode.Int32:
                                return (int)left != (int)right;
                            case TypeCode.Int64:
                                return (long)left != (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left != (sbyte)right;
                            case TypeCode.Single:
                                return (float)left != (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left != (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left != (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left != (ulong)right;
                            default:
                                return left != right;
                        }
                    case ExpressionType.Or:
                        switch (type)
                        {
                            case TypeCode.Boolean:
                                return (bool)left | (bool)right;
                            case TypeCode.Byte:
                                return (byte)left | (byte)right;
                            case TypeCode.Char:
                                return (char)left | (char)right;
                            case TypeCode.Int16:
                                return (short)left | (short)right;
                            case TypeCode.Int32:
                                return (int)left | (int)right;
                            case TypeCode.Int64:
                                return (long)left | (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left | (sbyte)right;
                            case TypeCode.UInt16:
                                return (ushort)left | (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left | (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left | (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.OrElse:
                        return (bool)left || (bool)right;
                    case ExpressionType.Power:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return Math.Pow((byte)left, (byte)right);
                            case TypeCode.Char:
                                return Math.Pow((char)left, (char)right);
                            case TypeCode.Double:
                                return Math.Pow((double)left, (double)right);
                            case TypeCode.Int16:
                                return Math.Pow((short)left, (short)right);
                            case TypeCode.Int32:
                                return Math.Pow((int)left, (int)right);
                            case TypeCode.Int64:
                                return Math.Pow((long)left, (long)right);
                            case TypeCode.SByte:
                                return Math.Pow((sbyte)left, (sbyte)right);
                            case TypeCode.Single:
                                return Math.Pow((float)left, (float)right);
                            case TypeCode.UInt16:
                                return Math.Pow((ushort)left, (ushort)right);
                            case TypeCode.UInt32:
                                return Math.Pow((uint)left, (uint)right);
                            case TypeCode.UInt64:
                                return Math.Pow((ulong)left, (ulong)right);
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.RightShift:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left >> (int)right;
                            case TypeCode.Char:
                                return (char)left >> (int)right;
                            case TypeCode.Int16:
                                return (short)left >> (int)right;
                            case TypeCode.Int32:
                                return (int)left >> (int)right;
                            case TypeCode.Int64:
                                return (long)left >> (int)right;
                            case TypeCode.SByte:
                                return (sbyte)left >> (int)right;
                            case TypeCode.UInt16:
                                return (ushort)left >> (int)right;
                            case TypeCode.UInt32:
                                return (uint)left >> (int)right;
                            case TypeCode.UInt64:
                                return (ulong)left >> (int)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.Subtract:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte)left - (byte)right;
                            case TypeCode.Char:
                                return (char)left - (char)right;
                            case TypeCode.Double:
                                return (double)left - (double)right;
                            case TypeCode.Int16:
                                return (short)left - (short)right;
                            case TypeCode.Int32:
                                return (int)left - (int)right;
                            case TypeCode.Int64:
                                return (long)left - (long)right;
                            case TypeCode.SByte:
                                return (sbyte)left - (sbyte)right;
                            case TypeCode.Single:
                                return (float)left - (float)right;
                            case TypeCode.UInt16:
                                return (ushort)left - (ushort)right;
                            case TypeCode.UInt32:
                                return (uint)left - (uint)right;
                            case TypeCode.UInt64:
                                return (ulong)left - (ulong)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    default:
                        throw new InvalidOperationException("Operation " + operation + " not valid binary operation.");
                }
            }

            private static bool computeNullableLogic(object left, object right, Type nullableType, ExpressionType operation)
            {
                Type underlyingType = Nullable.GetUnderlyingType(nullableType);
                if (underlyingType == null)
                {
                    throw new ArgumentException("Must be nullable type.", "nullableType");
                }
                TypeCode type = Type.GetTypeCode(underlyingType);
                switch (operation)
                {
                    case ExpressionType.Equal:
                        switch (type)
                        {
                            case TypeCode.Boolean:
                                return (bool?)left == (bool?)right;
                            case TypeCode.Byte:
                                return (byte?)left == (byte?)right;
                            case TypeCode.Char:
                                return (char?)left == (char?)right;
                            case TypeCode.Double:
                                return (double?)left == (double?)right;
                            case TypeCode.Int16:
                                return (short?)left == (short?)right;
                            case TypeCode.Int32:
                                return (int?)left == (int?)right;
                            case TypeCode.Int64:
                                return (long?)left == (long?)right;
                            case TypeCode.SByte:
                                return (sbyte?)left == (sbyte?)right;
                            case TypeCode.Single:
                                return (float?)left == (float?)right;
                            case TypeCode.UInt16:
                                return (ushort?)left == (ushort?)right;
                            case TypeCode.UInt32:
                                return (uint?)left == (uint?)right;
                            case TypeCode.UInt64:
                                return (ulong?)left == (ulong?)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.GreaterThan:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte?)left > (byte?)right;
                            case TypeCode.Char:
                                return (char?)left > (char?)right;
                            case TypeCode.Double:
                                return (double?)left > (double?)right;
                            case TypeCode.Int16:
                                return (short?)left > (short?)right;
                            case TypeCode.Int32:
                                return (int?)left > (int?)right;
                            case TypeCode.Int64:
                                return (long?)left > (long?)right;
                            case TypeCode.SByte:
                                return (sbyte?)left > (sbyte?)right;
                            case TypeCode.Single:
                                return (float?)left > (float?)right;
                            case TypeCode.UInt16:
                                return (ushort?)left > (ushort?)right;
                            case TypeCode.UInt32:
                                return (uint?)left > (uint?)right;
                            case TypeCode.UInt64:
                                return (ulong?)left > (ulong?)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.GreaterThanOrEqual:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte?)left >= (byte?)right;
                            case TypeCode.Char:
                                return (char?)left >= (char?)right;
                            case TypeCode.Double:
                                return (double?)left >= (double?)right;
                            case TypeCode.Int16:
                                return (short?)left >= (short?)right;
                            case TypeCode.Int32:
                                return (int?)left >= (int?)right;
                            case TypeCode.Int64:
                                return (long?)left >= (long?)right;
                            case TypeCode.SByte:
                                return (sbyte?)left >= (sbyte?)right;
                            case TypeCode.Single:
                                return (float?)left >= (float?)right;
                            case TypeCode.UInt16:
                                return (ushort?)left >= (ushort?)right;
                            case TypeCode.UInt32:
                                return (uint?)left >= (uint?)right;
                            case TypeCode.UInt64:
                                return (ulong?)left >= (ulong?)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.LessThan:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte?)left < (byte?)right;
                            case TypeCode.Char:
                                return (char?)left < (char?)right;
                            case TypeCode.Double:
                                return (double?)left < (double?)right;
                            case TypeCode.Int16:
                                return (short?)left < (short?)right;
                            case TypeCode.Int32:
                                return (int?)left < (int?)right;
                            case TypeCode.Int64:
                                return (long?)left < (long?)right;
                            case TypeCode.SByte:
                                return (sbyte?)left < (sbyte?)right;
                            case TypeCode.Single:
                                return (float?)left < (float?)right;
                            case TypeCode.UInt16:
                                return (ushort?)left < (ushort?)right;
                            case TypeCode.UInt32:
                                return (uint?)left < (uint?)right;
                            case TypeCode.UInt64:
                                return (ulong?)left < (ulong?)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.LessThanOrEqual:
                        switch (type)
                        {
                            case TypeCode.Byte:
                                return (byte?)left <= (byte?)right;
                            case TypeCode.Char:
                                return (char?)left <= (char?)right;
                            case TypeCode.Double:
                                return (double?)left <= (double?)right;
                            case TypeCode.Int16:
                                return (short?)left <= (short?)right;
                            case TypeCode.Int32:
                                return (int?)left <= (int?)right;
                            case TypeCode.Int64:
                                return (long?)left <= (long?)right;
                            case TypeCode.SByte:
                                return (sbyte?)left <= (sbyte?)right;
                            case TypeCode.Single:
                                return (float?)left <= (float?)right;
                            case TypeCode.UInt16:
                                return (ushort?)left <= (ushort?)right;
                            case TypeCode.UInt32:
                                return (uint?)left <= (uint?)right;
                            case TypeCode.UInt64:
                                return (ulong?)left <= (ulong?)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    case ExpressionType.NotEqual:
                        switch (type)
                        {
                            case TypeCode.Boolean:
                                return (bool?)left != (bool?)right;
                            case TypeCode.Byte:
                                return (byte?)left != (byte?)right;
                            case TypeCode.Char:
                                return (char?)left != (char?)right;
                            case TypeCode.Double:
                                return (double?)left != (double?)right;
                            case TypeCode.Int16:
                                return (short?)left != (short?)right;
                            case TypeCode.Int32:
                                return (int?)left != (int?)right;
                            case TypeCode.Int64:
                                return (long?)left != (long?)right;
                            case TypeCode.SByte:
                                return (sbyte?)left != (sbyte?)right;
                            case TypeCode.Single:
                                return (float?)left != (float?)right;
                            case TypeCode.UInt16:
                                return (ushort?)left != (ushort?)right;
                            case TypeCode.UInt32:
                                return (uint?)left != (uint?)right;
                            case TypeCode.UInt64:
                                return (ulong?)left != (ulong?)right;
                            default:
                                throw new InvalidOperationException();
                        }
                    default:
                        throw new InvalidOperationException("Operation " + operation + " not valid binary operation.");
                }
            }

            private static object negate(object operand)
            {
                if (operand == null)
                {
                    throw new ArgumentNullException("operand");
                }
                switch (Type.GetTypeCode(operand.GetType()))
                {
                    case TypeCode.Byte:
                        return -(byte)operand;
                    case TypeCode.Char:
                        return -(char)operand;
                    case TypeCode.Double:
                        return -(double)operand;
                    case TypeCode.Int16:
                        return -(short)operand;
                    case TypeCode.Int32:
                        return -(int)operand;
                    case TypeCode.Int64:
                        return -(long)operand;
                    case TypeCode.SByte:
                        return -(sbyte)operand;
                    case TypeCode.Single:
                        return -(float)operand;
                    case TypeCode.UInt16:
                        return -(ushort)operand;
                    case TypeCode.UInt32:
                        return -(uint)operand;
                    default:
                        throw new InvalidOperationException();
                }
            }

            private static object onesComplement(object operand)
            {
                if (operand == null)
                {
                    throw new ArgumentNullException("operand");
                }
                switch (Type.GetTypeCode(operand.GetType()))
                {
                    case TypeCode.Byte:
                        return ~(byte)operand;
                    case TypeCode.Char:
                        return ~(char)operand;
                    case TypeCode.Int16:
                        return ~(short)operand;
                    case TypeCode.Int32:
                        return ~(int)operand;
                    case TypeCode.Int64:
                        return ~(long)operand;
                    case TypeCode.SByte:
                        return ~(sbyte)operand;
                    case TypeCode.UInt16:
                        return ~(ushort)operand;
                    case TypeCode.UInt32:
                        return ~(uint)operand;
                    case TypeCode.UInt64:
                        return ~(ulong)operand;
                    default:
                        throw new InvalidOperationException();
                }
            }

            private static object objAs<T>(object obj)
                where T : class
            {
                return obj as T;
            }

            private static bool objIs<T>(object obj)
            {
                return obj is T;
            }

            private Expression getUndreducedExpression(Expression node)
            {
                Expression ret;
                if (!_reducedExpressionMapper.TryGetValue(node, out ret))
                {
                    ret = node;
                }
                return ret;
            }

            public override Expression Visit(Expression node)
            {
                Expression ret = base.Visit(node);
                if (ret != node)
                {
                    _reducedExpressionMapper.Add(ret, node);
                }
                return ret;
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {

                Expression operand = Visit(node.Operand);
                ConstantExpression operandAsConst = operand as ConstantExpression;
                if (operandAsConst != null)
                {
                    object obj = operandAsConst.Value;
                    object value;
                    Type nullableUnderlyingType = Nullable.GetUnderlyingType(node.Type);
                    switch (node.NodeType)
                    {
                        case ExpressionType.Convert:
                            if (nullableUnderlyingType == operandAsConst.Type)
                            {
                                value = obj;
                            }
                            else if (node.Method != null)
                            {
                                value = node.Method.Invoke(null, new[] { obj });
                            }
                            else
                            {
                                value = Convert.ChangeType(obj, node.Type);
                            }
                            break;
                        case ExpressionType.Negate:
                            if (nullableUnderlyingType != null)
                            {
                                if (obj == null)
                                {
                                    value = null;
                                }
                                else if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = negate(obj);
                                }
                            }
                            else
                            {
                                if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = negate(obj);
                                }
                            }
                            break;
                        case ExpressionType.Not:
                            if (nullableUnderlyingType != null)
                            {
                                if (obj == null)
                                {
                                    value = null;
                                }
                                else if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = !(bool)obj;
                                }
                            }
                            else
                            {
                                if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = !(bool)obj;
                                }
                            }
                            break;
                        case ExpressionType.OnesComplement:
                            if (nullableUnderlyingType != null)
                            {
                                if (obj == null)
                                {
                                    value = null;
                                }
                                else if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = onesComplement(obj);
                                }
                            }
                            else
                            {
                                if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = onesComplement(obj);
                                }
                            }
                            break;
                        case ExpressionType.UnaryPlus:
                            if (nullableUnderlyingType != null)
                            {
                                if (obj == null)
                                {
                                    value = null;
                                }
                                else if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = obj;
                                }
                            }
                            else
                            {
                                if (node.Method != null)
                                {
                                    value = node.Method.Invoke(null, new[] { obj });
                                }
                                else
                                {
                                    value = obj;
                                }
                            }
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    return Expression.Constant(value, node.Type);
                }
                UnaryExpression operandAsUnary = operand as UnaryExpression;
                if (operandAsUnary != null)
                {
                    if (operandAsUnary.NodeType == node.NodeType)
                    {
                        if (node.NodeType == ExpressionType.Negate || node.NodeType == ExpressionType.Not || node.NodeType == ExpressionType.OnesComplement)
                        {
                            return operandAsUnary.Operand;
                        }
                    }
                }
                return base.VisitUnary(node);
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                Expression reducedExpression = Visit(node.Expression);
                ConstantExpression reducedExpressionAsConst = reducedExpression as ConstantExpression;
                if (reducedExpressionAsConst != null)
                {
                    object obj = reducedExpressionAsConst.Value;
                    object value;
                    switch (node.NodeType)
                    {
                        case ExpressionType.TypeAs:
                            value = _objAsGeneric.MakeGenericMethod(node.TypeOperand).Invoke(null, new[] { obj });
                            break;
                        case ExpressionType.TypeIs:
                            value = _objIsGeneric.MakeGenericMethod(node.TypeOperand).Invoke(null, new[] { obj });
                            break;
                        default:
                            throw new InvalidOperationException("ExpressionType " + node.NodeType + " is invalid for TypeBinaryExpression.");
                    }
                    return Expression.Constant(value, node.Type);
                }
                return base.VisitTypeBinary(node);
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                Expression reducedObject = Visit(node.Object);
                Expression[] reducedArguments = node.Arguments.Select(e => Visit(e)).ToArray();
                ConstantExpression reducedObjectAsConst = reducedObject as ConstantExpression;
                if (reducedObjectAsConst != null && reducedArguments.All(e => e is ConstantExpression))
                {
                    object[] indexValues = reducedArguments.Cast<ConstantExpression>().Select(e => e.Value).ToArray();
                    object obj = reducedObjectAsConst.Value;
                    object value;
                    if (node.Indexer == null)
                    {
                        Array array = obj as Array;
                        if (array == null)
                        {
                            throw new InvalidOperationException("No indexer property, but constant expression does not represent array.");
                        }
                        int[] indices = indexValues.Select(v => Convert.ToInt32(v)).ToArray();
                        value = array.GetValue(indices);
                    }
                    else
                    {
                        value = node.Indexer.GetValue(obj, indexValues);
                    }
                    return Expression.Constant(value, node.Type);
                }
                return base.VisitIndex(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                Expression left = Visit(node.Left);
                Expression right = Visit(node.Right);
                ConstantExpression leftConst = left as ConstantExpression;
                ConstantExpression rightConst = right as ConstantExpression;
                if (leftConst != null && rightConst != null)
                {
                    object value;
                    object leftObj = leftConst.Value;
                    object rightObj = rightConst.Value;
                    Type nullableUnderlyingType = Nullable.GetUnderlyingType(node.Type);
                    if (node.NodeType == ExpressionType.Coalesce)
                    {
                        value = leftObj ?? rightObj;
                    }
                    else if (nullableUnderlyingType != null) // must add null checking
                    {
                        if (node.Type == typeof(bool))
                        {
                            if (node.Method != null)
                            {
                                if (node.NodeType == ExpressionType.Equal)
                                {
                                    if (leftObj == null && rightObj == null)
                                    {
                                        value = true;
                                    }
                                    else if (leftObj == null || rightObj == null)
                                    {
                                        value = false;
                                    }
                                    else
                                    {
                                        value = node.Method.Invoke(null, new[] { leftObj, rightObj });
                                    }
                                }
                                else if (node.NodeType == ExpressionType.NotEqual)
                                {
                                    if (leftObj == null && rightObj == null)
                                    {
                                        value = false;
                                    }
                                    else if (leftObj == null || rightObj == null)
                                    {
                                        value = true;
                                    }
                                    else
                                    {
                                        value = node.Method.Invoke(null, new[] { leftObj, rightObj });
                                    }
                                }
                                else
                                {
                                    if (leftObj == null || rightObj == null)
                                    {
                                        value = false;
                                    }
                                    else
                                    {
                                        value = node.Method.Invoke(null, new[] { leftObj, rightObj });
                                    }
                                }
                            }
                            else
                            {
                                value = computeNullableLogic(leftObj, rightObj, node.Type, node.NodeType);
                            }
                        }
                        else
                        {
                            if (leftObj == null || rightObj == null)
                            {
                                value = null;
                            }
                            else if (node.Method != null)
                            {
                                value = node.Method.Invoke(null, new[] { leftObj, rightObj });
                            }
                            else
                            {
                                value = computeBinaryOperation(leftObj, rightObj, node.NodeType);
                            }
                        }
                    }
                    else
                    {
                        if (node.Method != null)
                        {
                            value = node.Method.Invoke(null, new[] { leftObj, rightObj });
                        }
                        else
                        {
                            value = computeBinaryOperation(leftObj, rightObj, node.NodeType);
                        }
                    }
                    return Expression.Constant(value, node.Type);
                }
                else if (leftConst != null && leftConst.Type == typeof(bool))
                {
                    return reduceBoolBinaryExpression(right, (bool)leftConst.Value, node.NodeType);
                }
                else if (rightConst != null && rightConst.Type == typeof(bool))
                {
                    return reduceBoolBinaryExpression(left, (bool)rightConst.Value, node.NodeType);
                }
                return base.VisitBinary(node);
            }

            private static Expression reduceBoolBinaryExpression(Expression expression, bool constValue, ExpressionType operation)
            {
                switch (operation)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        if (constValue) // (expression && true) ~ expression
                        {
                            return expression;
                        }
                        else // (expression && false) ~ false
                        {
                            return Expression.Constant(false);
                        }
                    case ExpressionType.Equal:
                        if (constValue) // (expression == true) ~ expression
                        {
                            return expression;
                        }
                        else // (expression == false) ~ !expression
                        {
                            return Expression.Not(expression);
                        }
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.NotEqual:
                        if (constValue) // (expression != true) ~ (expression ^ true) ~ !expression
                        {
                            return Expression.Not(expression);
                        }
                        else // (expression != false) ~ (expression ^ false) ~ expression
                        {
                            return expression;
                        }
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        if (constValue) // (expression || true) ~ true
                        {
                            return Expression.Constant(true);
                        }
                        else // (expression || false) ~ expression
                        {
                            return expression;
                        }
                    default:
                        throw new InvalidOperationException("Operation " + operation + " is not valid on Boolean expressions.");
                }
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                FieldInfo field;
                PropertyInfo property;
                if (node.Expression != null)
                {
                    Expression reducedExpression = Visit(node.Expression);
                    ConstantExpression asConst = reducedExpression as ConstantExpression;
                    if (asConst != null)
                    {
                        object obj = asConst.Value;
                        object value;
                        if (obj == null)
                        {
                            if (node.Member.DeclaringType.IsNullable())
                            {
                                if (node.Member.Name == "HasValue")
                                {
                                    value = false;
                                }
                                else if (node.Member.Name == "Value")
                                {
                                    throw new InvalidOperationException("Nullable object must have a value");
                                }
                                else
                                {
                                    throw new InvalidOperationException("Unknown member " + node.Member);
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Expression reduces to calling member " + node.Member + " on a null reference.");
                            }
                        }
                        else
                        {
                            field = node.Member as FieldInfo;
                            if (field != null)
                            {
                                value = field.GetValue(obj);
                            }
                            else
                            {
                                property = node.Member as PropertyInfo;
                                if (property == null)
                                {
                                    throw new InvalidOperationException("Member " + node.Member + " is not field or property?");
                                }
                                value = property.GetValue(obj);
                            }
                        }
                        return Expression.Constant(value, node.Type);
                    }
                }
                field = node.Member as FieldInfo;
                if (field != null)
                {
                    if (field.IsStatic)
                    {
                        object value = field.GetValue(null);
                        return Expression.Constant(value, node.Type);
                    }
                    else
                    {
                        return node;
                    }
                }
                property = node.Member as PropertyInfo;
                if (property != null)
                {
                    if (property.GetGetMethod(true).IsStatic)
                    {
                        object value = property.GetValue(null);
                        return Expression.Constant(value, node.Type);
                    }
                    else
                    {
                        return node;
                    }
                }
                else
                {
                    throw new InvalidOperationException("MemberExpression " + node + " does not represent a field or property.");
                }
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                Expression ifTrue = Visit(node.IfTrue);
                Expression ifFalse = Visit(node.IfFalse);
                Expression test = Visit(node.Test);
                ConstantExpression testAsConst = test as ConstantExpression;
                if (testAsConst != null)
                {
                    if ((bool)testAsConst.Value)
                    {
                        return ifTrue;
                    }
                    else
                    {
                        return ifFalse;
                    }
                }
                return base.VisitConditional(node);
            }

            private static object getDefaultValue(Type type)
            {
                return _getDefaultValueGeneric.MakeGenericMethod(type).Invoke(null, null);
            }

            private static T getDefaultValueGeneric<T>()
            {
                return default(T);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                MethodInfo method = node.Method;
                Expression[] reducedArguments = node.Arguments.Select(a => Visit(a)).ToArray();
                if (method.IsStatic)
                {
                    if (reducedArguments.All(a => a is ConstantExpression))
                    {
                        object[] parameters = reducedArguments.Cast<ConstantExpression>().Select(e => e.Value).ToArray();
                        IQueryable[] queryableObjects = parameters.OfType<IQueryable>().ToArray();
                        if (queryableObjects.All(o => o.GetType() == _enumerableQueryType))
                        {
                            object value = method.Invoke(null, parameters);
                            return Expression.Constant(value, node.Type);
                        }
                    }
                    else if (reducedArguments.All(a => a is ConstantExpression || a.NodeType == ExpressionType.Quote))
                    {
                        ConstantExpression[] constExpressions = reducedArguments.OfType<ConstantExpression>().ToArray();
                        object[] objects = constExpressions.Select(e => e.Value).ToArray();
                        IQueryable[] queryableObjects = objects.OfType<IQueryable>().ToArray();
                        if (queryableObjects.All(o => o.GetType() == _enumerableQueryType))
                        {
                            List<Expression> arguments = new List<Expression>();
                            foreach (Expression argument in reducedArguments)
                            {
                                UnaryExpression argAsUnary = argument as UnaryExpression;
                                if (argAsUnary == null)
                                {
                                    arguments.Add(argument);
                                }
                                else
                                {
                                    Debug.Assert(argAsUnary.NodeType == ExpressionType.Quote);
                                    LambdaExpression lambda = (LambdaExpression)argAsUnary.Operand;
                                    arguments.Add(Expression.Constant(lambda));
                                }
                            }
                            return Visit(Expression.Call(method, arguments));
                        }
                    }
                    if (method.IsGenericMethod && method.DeclaringType == typeof(Queryable))
                    {
                        MethodInfo genericDef = node.Method.GetGenericMethodDefinition();
                        Type genericType = node.Method.GetGenericArguments()[0];

                        Expression[] arguments = node.Arguments.Select(e => Visit(e)).ToArray();
                        MethodCallExpression firstArgAsMethodCall = arguments[0] as MethodCallExpression;
                        //LambdaExpression baseWhereExpression = extractLambda(arguments[1]);
                        if (genericDef == QueryableMethods.All)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            LambdaExpression negatedPredicate = Expression.Lambda(predicate.GetType().GetGenericArguments()[0], Expression.Not(predicate.Body), predicate.Parameters);
                            return Visit(Expression.Call(QueryableMethods.AnyMatchExpression.MakeGenericMethod(genericType), arguments[0], Expression.Quote(negatedPredicate)));
                        }
                        else if (genericDef == QueryableMethods.AnyMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.Any(true) ~ seq.Any()
                                {
                                    return Visit(Expression.Call(QueryableMethods.Any.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.Any(false) ~ false
                                {
                                    return Expression.Constant(false);
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.CountMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.Count(true) ~ seq.Count()
                                {
                                    return Visit(Expression.Call(QueryableMethods.Count.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.Count(false) ~ 0
                                {
                                    return Expression.Constant(0);
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.FirstMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.First(true) ~ seq.First()
                                {
                                    return Visit(Expression.Call(QueryableMethods.First.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.First(false) ~ ERROR
                                {
                                    throw new InvalidOperationException("Sequence contains no elements.");
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.FirstOrDefaultMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.FirstOrDefault(true) ~ seq.FirstOrDefault()
                                {
                                    return Visit(Expression.Call(QueryableMethods.FirstOrDefault.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.FirstOrDefault(false) ~ default(T)
                                {
                                    return Expression.Constant(getDefaultValue(genericType), genericType);
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.LastMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.Last(true) ~ seq.Last()
                                {
                                    return Visit(Expression.Call(QueryableMethods.Last.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.Last(false) ~ ERROR
                                {
                                    throw new InvalidOperationException("Sequence contains no elements.");
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.LastOrDefaultMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.LastOrDefault(true) ~ seq.LastOrDefault()
                                {
                                    return Visit(Expression.Call(QueryableMethods.LastOrDefault.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.LastOrDefault(false) ~ default(T)
                                {
                                    return Expression.Constant(getDefaultValue(genericType), genericType);
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.LongCountMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.LongCount(true) ~ seq.LongCount()
                                {
                                    return Visit(Expression.Call(QueryableMethods.LongCount.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.LongCount(false) ~ 0
                                {
                                    return Expression.Constant(0L);
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.SingleMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.Single(true) ~ seq.Single()
                                {
                                    return Visit(Expression.Call(QueryableMethods.Single.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.Single(false) ~ ERROR
                                {
                                    throw new InvalidOperationException("Sequence contains no elements.");
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.SingleOrDefaultMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.SingleOrDefault(true) ~ seq.SingleOrDefault()
                                {
                                    return Visit(Expression.Call(QueryableMethods.SingleOrDefault.MakeGenericMethod(genericType), arguments[0]));
                                }
                                else // seq.SingleOrDefault(false) ~ default(T)
                                {
                                    return Expression.Constant(getDefaultValue(genericType), genericType);
                                }
                            }
                        }
                        else if (genericDef == QueryableMethods.Where)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true) // seq.Where(true) ~ seq
                                {
                                    return arguments[0];
                                }
                                else // seq.Where(false) ~ empty sequence
                                {
                                    IEnumerable empty = (IEnumerable)EnumerableMethods.Empty.MakeGenericMethod(genericType).Invoke(null, null);
                                    return Expression.Constant(empty.AsQueryable());
                                }
                            }
                        }

                        if (firstArgAsMethodCall != null)
                        {
                            if (firstArgAsMethodCall.Method.DeclaringType == typeof(Queryable))
                            {
                                if (firstArgAsMethodCall.Method.IsGenericMethod)
                                {
                                    return combineQueryableMethods(node, firstArgAsMethodCall);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Expression reducedObject = Visit(node.Object);
                    ConstantExpression asConstExpr = reducedObject as ConstantExpression;
                    if (asConstExpr != null && reducedArguments.All(a => a is ConstantExpression))
                    {
                        object obj = asConstExpr.Value;
                        object value;
                        if (obj == null)
                        {
                            if (method.DeclaringType.IsNullable())
                            {
                                Type underlyingType = Nullable.GetUnderlyingType(method.DeclaringType);
                                if (method.Name == "GetValueOrDefault")
                                {
                                    if (method.GetParameters().Length == 0)
                                    {
                                        value = getDefaultValue(underlyingType);
                                    }
                                    else
                                    {
                                        value = ((ConstantExpression)reducedArguments.Single()).Value;
                                    }
                                }
                                else
                                {
                                    throw new InvalidOperationException("Unknown method " + method);
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Expression reduces to calling method " + method + " on a null reference.");
                            }
                        }
                        else
                        {
                            object[] parameters = reducedArguments.Cast<ConstantExpression>().Select(e => e.Value).ToArray();
                            value = method.Invoke(obj, parameters);
                        }
                        return Expression.Constant(value, node.Type);
                    }
                }
                return base.VisitMethodCall(node);
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Queryable<TElement> { Expression = expression, Provider = this };
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return QueryHelpers.CreateQuery(this, expression);
        }

        private TypeManager _typeManager;

        public QueryProvider(TypeManager typeManager)
        {
            _typeManager = typeManager;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        /*private class QueryReduceVisitor : ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(Queryable))
                {
                    if (node.Method.IsGenericMethod)
                    {
                        MethodInfo genericDef = node.Method.GetGenericMethodDefinition();
                        Type genericType = node.Method.GetGenericArguments()[0];
                        
                        Expression[] arguments = node.Arguments.Select(e => Visit(e)).ToArray();
                        MethodCallExpression firstArgAsMethodCall = arguments[0] as MethodCallExpression;
                        //LambdaExpression baseWhereExpression = extractLambda(arguments[1]);
                        if (genericDef == QueryableMethods.All)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            LambdaExpression negatedPredicate = Expression.Lambda(predicate.GetType().GetGenericArguments()[0], Expression.Not(predicate.Body), predicate.Parameters);
                            return Visit(Expression.Call(QueryableMethods.AnyMatchExpression.MakeGenericMethod(genericType), arguments[0], Expression.Quote(negatedPredicate)));
                        }
                        else if (genericDef == QueryableMethods.AnyMatchExpression)
                        {
                            LambdaExpression predicate = extractLambda(arguments[1]);
                            ConstantExpression bodyAsConst = predicate.Body as ConstantExpression;
                            if (bodyAsConst != null)
                            {
                                if ((bool)bodyAsConst.Value == true)
                                {
                                    return Visit(Expression.Call(QueryableMethods.Any.MakeGenericMethod(genericType), arguments[0]));
                                }
                            }
                        }
                        
                        if (firstArgAsMethodCall != null)
                        {
                            if (firstArgAsMethodCall.Method.DeclaringType == typeof(Queryable))
                            {
                                if (firstArgAsMethodCall.Method.IsGenericMethod)
                                {
                                    return combineQueryableMethods(node, firstArgAsMethodCall);
                                }
                            }
                        }
                    }
                }
                return base.VisitMethodCall(node);
            }
        }*/

        private static MethodCallExpression combineQueryableMethods(MethodCallExpression baseExpression, MethodCallExpression inputArg)
        {
            MethodInfo baseMethod = baseExpression.Method.GetGenericMethodDefinition();
            Type genericType = baseExpression.Method.GetGenericArguments()[0];
            if (inputArg.Method.GetGenericMethodDefinition() == QueryableMethods.Where)
            {
                LambdaExpression whereExpression = getPredicateLambda(inputArg);
                if (baseMethod == QueryableMethods.Where
                    || baseMethod == QueryableMethods.AnyMatchExpression
                    || baseMethod == QueryableMethods.All
                    || baseMethod == QueryableMethods.CountMatchExpression
                    || baseMethod == QueryableMethods.FirstMatchExpression
                    || baseMethod == QueryableMethods.FirstOrDefaultMatchExpression
                    || baseMethod == QueryableMethods.LastMatchExpression
                    || baseMethod == QueryableMethods.LastOrDefaultMatchExpression
                    || baseMethod == QueryableMethods.SingleMatchExpression
                    || baseMethod == QueryableMethods.SingleOrDefaultMatchExpression)
                {
                    LambdaExpression baseWhereExpression = getPredicateLambda(baseExpression);
                    LambdaExpression newLambda = andAlso(baseWhereExpression, whereExpression);
                    return Expression.Call(baseExpression.Method, inputArg.Arguments[0], Expression.Quote(newLambda));
                }
                else if (baseMethod == QueryableMethods.Any)
                {
                    return Expression.Call(QueryableMethods.AnyMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
                else if (baseMethod == QueryableMethods.Count)
                {
                    return Expression.Call(QueryableMethods.CountMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
                else if (baseMethod == QueryableMethods.First)
                {
                    return Expression.Call(QueryableMethods.FirstMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
                else if (baseMethod == QueryableMethods.FirstOrDefault)
                {
                    return Expression.Call(QueryableMethods.FirstOrDefaultMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
                else if (baseMethod == QueryableMethods.Last)
                {
                    return Expression.Call(QueryableMethods.LastMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
                else if (baseMethod == QueryableMethods.LastOrDefault)
                {
                    return Expression.Call(QueryableMethods.LastOrDefaultMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
                else if (baseMethod == QueryableMethods.Single)
                {
                    return Expression.Call(QueryableMethods.SingleMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
                else if (baseMethod == QueryableMethods.SingleOrDefault)
                {
                    return Expression.Call(QueryableMethods.SingleOrDefaultMatchExpression.MakeGenericMethod(genericType), inputArg.Arguments[0], Expression.Quote(whereExpression));
                }
            }
            return baseExpression;
        }

        private static LambdaExpression extractLambda(Expression lambdaArgument)
        {
            return (LambdaExpression)((UnaryExpression)lambdaArgument).Operand;
        }

        /*private static MethodCallExpression reduceQueryMethod(MethodCallExpression expression)
        {
            MethodInfo method = expression.Method;
            Debug.Assert(method.DeclaringType == typeof(Queryable), "Non-queryable method?");
            MethodInfo genericDef = method.GetGenericMethodDefinition();
            Type genericType = method.GetGenericArguments()[0];
            var arguments = expression.Arguments;
            Expression firstArg = arguments[0];
            MethodCallExpression firstArgAsMethodCall = firstArg as MethodCallExpression;
            if (firstArgAsMethodCall == null)
            {
                return expression;
            }
            else
            {
                firstArgAsMethodCall = reduceQueryMethod(firstArgAsMethodCall);
                if (firstArgAsMethodCall.Method.GetGenericMethodDefinition() == QueryableMethods.Where)
                {
                    LambdaExpression whereExpression = getPredicateLambda(firstArgAsMethodCall);
                    if (genericDef == QueryableMethods.Where 
                        || genericDef == QueryableMethods.AnyMatchExpression 
                        || genericDef == QueryableMethods.All 
                        || genericDef == QueryableMethods.CountMatchExpression
                        || genericDef == QueryableMethods.FirstMatchExpression
                        || genericDef == QueryableMethods.FirstOrDefaultMatchExpression
                        || genericDef == QueryableMethods.LastMatchExpression
                        || genericDef == QueryableMethods.LastOrDefaultMatchExpression
                        || genericDef == QueryableMethods.SingleMatchExpression
                        || genericDef == QueryableMethods.SingleOrDefaultMatchExpression)
                    {
                        LambdaExpression baseWhereExpression = getPredicateLambda(expression);
                        LambdaExpression newLambda = andAlso(baseWhereExpression, whereExpression);
                        return Expression.Call(method, firstArgAsMethodCall.Arguments[0], Expression.Quote(newLambda));
                    }
                    else if (genericDef == QueryableMethods.Any)
                    {
                        return Expression.Call(QueryableMethods.AnyMatchExpression.MakeGenericMethod(genericType),  firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.Count)
                    {
                        return Expression.Call(QueryableMethods.CountMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.First)
                    {
                        return Expression.Call(QueryableMethods.FirstMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.FirstOrDefault)
                    {
                        return Expression.Call(QueryableMethods.FirstOrDefaultMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.Last)
                    {
                        return Expression.Call(QueryableMethods.LastMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.LastOrDefault)
                    {
                        return Expression.Call(QueryableMethods.LastOrDefaultMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.Single)
                    {
                        return Expression.Call(QueryableMethods.SingleMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                    else if (genericDef == QueryableMethods.SingleOrDefault)
                    {
                        return Expression.Call(QueryableMethods.SingleOrDefaultMatchExpression.MakeGenericMethod(genericType), firstArgAsMethodCall.Arguments[0], Expression.Quote(whereExpression));
                    }
                }
            }
            throw new NotImplementedException();
        }*/

        private static LambdaExpression andAlso(LambdaExpression left, LambdaExpression right)
        {
            // need to detect whether they use the same
            // parameter instance; if not, they need fixing
            ParameterExpression param = left.Parameters[0];
            Type leftDelegateType = left.GetType().GetGenericArguments()[0];
            Type rightDelegateType = right.GetType().GetGenericArguments()[0];
            if (!leftDelegateType.Equals(rightDelegateType))
            {
                throw new ArgumentException("Delegate types do not match");
            }

            var visitor = new ParameterUpdateVisitor(right.Parameters[0], left.Parameters[0]);
            var body = visitor.Visit(right.Body);

            return Expression.Lambda(leftDelegateType, Expression.AndAlso(left.Body, body), left.Parameters[0]);
        }

        private class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _oldParameter)
                {
                    return _newParameter;
                }
                else
                {
                    return base.VisitParameter(node);
                }
            }
        }

        private static LambdaExpression getPredicateLambda(MethodCallExpression expression)
        {
            foreach (Expression arg in expression.Arguments)
            {
                if (arg.NodeType == ExpressionType.Quote)
                {
                    return (LambdaExpression)((UnaryExpression)arg).Operand;
                }
            }
            return null;
        }

        public object Execute(Expression expression)
        {
            IQueryable asQueryable;
            ReduceExpressionVisitor v = new ReduceExpressionVisitor();
            Expression reducedExpression = v.Visit(expression);
            switch (reducedExpression.NodeType)
            {
                case ExpressionType.Constant:
                    ConstantExpression constExpr = (ConstantExpression)reducedExpression;
                    asQueryable = constExpr.Value as IQueryable;
                    if (asQueryable != null && asQueryable.Provider == this)
                    {
                        return QueryHelpers.GenericCast(_typeManager.EnumerateValues(), _typeManager.Type);
                    }
                    else
                    {
                        return constExpr.Value;
                    }
                case ExpressionType.Call:
                    //QueryReduceVisitor queryVisitor = new QueryReduceVisitor();
                    MethodCallExpression methodCallExpression = (MethodCallExpression)reducedExpression;
                    ConstantExpression firstArg = methodCallExpression.Arguments[0] as ConstantExpression;
                    if (firstArg == null)
                    {
                        throw new InvalidOperationException();
                    }
                    asQueryable = firstArg.Value as IQueryable;
                    if (asQueryable != null && asQueryable.Provider == this)
                    {
                        LambdaExpression lambda = getPredicateLambda(methodCallExpression);
                        MethodInfo queryableMethod = methodCallExpression.Method.GetGenericMethodDefinition();
                        if (queryableMethod == QueryableMethods.Any)
                        {
                            return _typeManager.QueryAny();
                        }
                        else if (queryableMethod == QueryableMethods.AnyMatchExpression)
                        {
                            return _typeManager.QueryAny(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Count || queryableMethod == QueryableMethods.LongCount)
                        {
                            return _typeManager.QueryCount();
                        }
                        else if (queryableMethod == QueryableMethods.CountMatchExpression || queryableMethod == QueryableMethods.LongCountMatchExpression)
                        {
                            return _typeManager.QueryCount(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.First)
                        {
                            return _typeManager.QueryFirst();
                        }
                        else if (queryableMethod == QueryableMethods.FirstMatchExpression)
                        {
                            return _typeManager.QueryFirst(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.FirstOrDefault)
                        {
                            return _typeManager.QueryFirstOrDefault();
                        }
                        else if (queryableMethod == QueryableMethods.FirstOrDefaultMatchExpression)
                        {
                            return _typeManager.QueryFirstOrDefault(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Last)
                        {
                            return _typeManager.QueryLast();
                        }
                        else if (queryableMethod == QueryableMethods.LastMatchExpression)
                        {
                            return _typeManager.QueryLast(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.LastOrDefault)
                        {
                            return _typeManager.QueryLastOrDefault();
                        }
                        else if (queryableMethod == QueryableMethods.LastOrDefaultMatchExpression)
                        {
                            return _typeManager.QueryLastOrDefault(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Single)
                        {
                            return _typeManager.QuerySingle();
                        }
                        else if (queryableMethod == QueryableMethods.SingleMatchExpression)
                        {
                            return _typeManager.QuerySingle(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.SingleOrDefault)
                        {
                            return _typeManager.QuerySingleOrDefault();
                        }
                        else if (queryableMethod == QueryableMethods.SingleOrDefaultMatchExpression)
                        {
                            return _typeManager.QuerySingleOrDefault(lambda);
                        }
                        else if (queryableMethod == QueryableMethods.Where)
                        {
                            return _typeManager.QueryWhere(lambda);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
