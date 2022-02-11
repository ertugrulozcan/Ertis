using System;

namespace Ertis.Schema.Helpers
{
    internal static class NumericTypeHelper
    {
        #region Methods

        internal static bool? IsAssignableTo(this Type type1, Type type2)
        {
            if (!IsNumericType(type1) || !IsNumericType(type2))
            {
                return null;
            }
            
            if (type1.IsIntegralNumericType() && type2.IsIntegralNumericType())
            {
                var size1 = SizeOf(type1);
                var size2 = SizeOf(type2);
                if (size1 == null || size2 == null)
                {
                    return null;
                }

                return size1 < size2;
            }
            else if (type1.IsFloatingPointNumericType() && type2.IsFloatingPointNumericType())
            {
                if (type1 == typeof(decimal) || type2 == typeof(decimal))
                {
                    return false;
                }
                
                var size1 = SizeOf(type1);
                var size2 = SizeOf(type2);
                if (size1 == null || size2 == null)
                {
                    return null;
                }

                return size1 < size2;
            }
            else if (type1.IsIntegralNumericType() && type2.IsFloatingPointNumericType())
            {
                return true;
            }
            else if (type1.IsFloatingPointNumericType() && type2.IsIntegralNumericType())
            {
                return false;
            }
            
            return false;
        }

        private static bool IsNumericType(this Type type)
        {
            return
                IsIntegralNumericType(type) ||
                IsFloatingPointNumericType(type);
        }
        
        private static bool IsIntegralNumericType(this Type type)
        {
            return
                type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(nint) ||
                type == typeof(nuint) ||
                type == typeof(long) ||
                type == typeof(ulong);
        }
        
        private static bool IsFloatingPointNumericType(this Type type)
        {
            return
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal);
        }

        private static int? SizeOf(Type type)
        {
            if (type == typeof(byte))
            {
                return 7;
            }
            else if (type == typeof(sbyte))
            {
                return 8;
            }
            else if (type == typeof(short))
            {
                return 15;
            }
            else if (type == typeof(ushort))
            {
                return 16;
            }
            else if (type == typeof(int))
            {
                return 31;
            }
            else if (type == typeof(long))
            {
                return 63;
            }
            else if (type == typeof(ulong))
            {
                return 64;
            }
            else if (type == typeof(float))
            {
                return 32;
            }
            else if (type == typeof(double))
            {
                return 64;
            }
            else if (type == typeof(decimal))
            {
                return 128;
            }
            else
            {
                return null;
            }
        }
        
        #endregion
    }
}