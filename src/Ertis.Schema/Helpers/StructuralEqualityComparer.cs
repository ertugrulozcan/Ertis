using System.Globalization;

namespace Ertis.Schema.Helpers;

/// <summary>
/// Compares the values of the dynamic value model by their content:
/// objects by their properties (regardless of the order), arrays item by item and numbers by their values (1 equals 1.0)
/// </summary>
internal sealed class StructuralEqualityComparer : IEqualityComparer<object?>
{
	#region Fields
	
	internal static readonly StructuralEqualityComparer Instance = new();
	
	#endregion
	
	#region Methods
	
	public new bool Equals(object? x, object? y)
	{
		if (ReferenceEquals(x, y))
		{
			return true;
		}
		
		if (x == null || y == null)
		{
			return false;
		}
		
		if (TryGetNumber(x, out var number1) && TryGetNumber(y, out var number2))
		{
			return number1.Equals(number2);
		}
		
		switch (x, y)
		{
			case (IDictionary<string, object?> dictionary1, IDictionary<string, object?> dictionary2):
			{
				if (dictionary1.Count != dictionary2.Count)
				{
					return false;
				}
				
				foreach (var (key, value) in dictionary1)
				{
					if (!dictionary2.TryGetValue(key, out var otherValue) || !this.Equals(value, otherValue))
					{
						return false;
					}
				}
				
				return true;
			}
			case (object?[] array1, object?[] array2):
			{
				if (array1.Length != array2.Length)
				{
					return false;
				}
				
				for (var i = 0; i < array1.Length; i++)
				{
					if (!this.Equals(array1[i], array2[i]))
					{
						return false;
					}
				}
				
				return true;
			}
			default:
				return x.Equals(y);
		}
	}
	
	public int GetHashCode(object? obj)
	{
		switch (obj)
		{
			case null:
				return 0;
			case IDictionary<string, object?> dictionary:
			{
				// Order independent
				var hashCode = dictionary.Count;
				foreach (var (key, value) in dictionary)
				{
					hashCode ^= HashCode.Combine(key, this.GetHashCode(value));
				}
				
				return hashCode;
			}
			case object?[] array:
			{
				var hashCode = new HashCode();
				foreach (var item in array)
				{
					hashCode.Add(this.GetHashCode(item));
				}
				
				return hashCode.ToHashCode();
			}
			default:
				return TryGetNumber(obj, out var number) ? number.GetHashCode() : obj.GetHashCode();
		}
	}
	
	/// <summary>
	/// Gets the value of a number as decimal (exact for the integers, unlike double) or as double when it is out of the decimal range
	/// </summary>
	private static bool TryGetNumber(object value, out object number)
	{
		switch (value)
		{
			case decimal decimalValue:
				number = decimalValue;
				return true;
			case double or float:
			{
				var doubleValue = value is double exactValue ? exactValue : (float) value;
				number = double.IsFinite(doubleValue) && Math.Abs(doubleValue) < 7.9e28 ? (decimal) doubleValue : doubleValue;
				return true;
			}
			case long or int or short or sbyte or byte or ulong or uint or ushort:
				number = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
				return true;
			default:
				number = value;
				return false;
		}
	}
	
	#endregion
}
