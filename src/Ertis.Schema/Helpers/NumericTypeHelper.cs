namespace Ertis.Schema.Helpers;

internal static class NumericTypeHelper
{
	#region Methods
	
	internal static bool? IsAssignableTo(this Type type1, Type type2, bool allowNullableTypes = true)
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
			
			return size1 <= size2;
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
			
			return size1 <= size2;
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
	
	/// <summary>
	/// Gets the value of an integral number that fits into Int64
	/// </summary>
	internal static bool TryGetInt64(object? obj, out long value)
	{
		switch (obj)
		{
			case long longValue:
				value = longValue;
				return true;
			case int intValue:
				value = intValue;
				return true;
			case short shortValue:
				value = shortValue;
				return true;
			case sbyte sbyteValue:
				value = sbyteValue;
				return true;
			case byte byteValue:
				value = byteValue;
				return true;
			case ushort ushortValue:
				value = ushortValue;
				return true;
			case uint uintValue:
				value = uintValue;
				return true;
			case nint nintValue:
				value = nintValue;
				return true;
			default:
				value = 0;
				return false;
		}
	}
	
	/// <summary>
	/// Gets the value of a number that is assignable to Double (every integral and binary floating point number, not decimal)
	/// </summary>
	internal static bool TryGetDouble(object? obj, out double value)
	{
		switch (obj)
		{
			case double doubleValue:
				value = doubleValue;
				return true;
			case float floatValue:
				value = floatValue;
				return true;
			case ulong ulongValue:
				value = ulongValue;
				return true;
			case nuint nuintValue:
				value = nuintValue;
				return true;
			default:
			{
				if (TryGetInt64(obj, out var longValue))
				{
					value = longValue;
					return true;
				}
				
				value = 0;
				return false;
			}
		}
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
		// The sizes are the value bits (signed types: n - 1, unsigned types: n), so a type is assignable to another one with an equal or greater size
		if (type == typeof(byte))
		{
			return 8;
		}
		else if (type == typeof(sbyte))
		{
			return 7;
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
		else if (type == typeof(uint))
		{
			return 32;
		}
		else if (type == typeof(long) || type == typeof(nint))
		{
			return 63;
		}
		else if (type == typeof(ulong) || type == typeof(nuint))
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