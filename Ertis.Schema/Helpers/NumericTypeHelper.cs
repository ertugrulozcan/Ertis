namespace Ertis.Schema.Helpers;

internal static class NumericTypeHelper
{
    #region Methods
    
    extension(Type type1)
    {
        internal bool? IsAssignableTo(Type type2, bool allowNullableTypes = true)
        {
            if (allowNullableTypes)
            {
                var type1UnderlyingType = Nullable.GetUnderlyingType(type1);
                if (type1UnderlyingType != null)
                {
                    type1 = type1UnderlyingType;
                }
                
                var type2UnderlyingType = Nullable.GetUnderlyingType(type2);
                if (type2UnderlyingType != null)
                {
                    type2 = type2UnderlyingType;
                }
            }
            
            if (!type1.IsNumericType() || !type2.IsNumericType())
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
                // ReSharper disable once DuplicatedStatements
                return false;
            }
            
            return false;
        }
        
        private bool IsNumericType()
        {
            return type1.IsIntegralNumericType() || type1.IsFloatingPointNumericType();
        }
        
        private bool IsIntegralNumericType()
        {
            return
                type1 == typeof(byte) ||
                type1 == typeof(sbyte) ||
                type1 == typeof(short) ||
                type1 == typeof(ushort) ||
                type1 == typeof(int) ||
                type1 == typeof(uint) ||
                type1 == typeof(nint) ||
                type1 == typeof(nuint) ||
                type1 == typeof(long) ||
                type1 == typeof(ulong);
        }
        
        private bool IsFloatingPointNumericType()
        {
            return
                type1 == typeof(float) ||
                type1 == typeof(double) ||
                type1 == typeof(decimal);
        }
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