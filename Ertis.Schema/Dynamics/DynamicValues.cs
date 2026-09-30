using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Ertis.Schema.Dynamics;

/// <summary>
/// Converts json and CLR values into the dynamic value model of the DynamicObject:
/// objects are Dictionary&lt;string, object?&gt;, arrays are object[] and the other values are strings, booleans, numbers or dates.
/// </summary>
internal static class DynamicValues
{
	#region Fields
	
	private static readonly JsonReaderOptions ReaderOptions = new()
	{
		AllowTrailingCommas = true,
		CommentHandling = JsonCommentHandling.Skip
	};
	
	/// <summary>
	/// The contract options of the CLR objects; property names are used as they are, like the [JsonPropertyName] attributes declare
	/// </summary>
	private static readonly JsonSerializerOptions ContractOptions = CreateContractOptions();
	
	private static readonly ConcurrentDictionary<JsonConverter, JsonSerializerOptions> ConverterOptions = new();
	
	#endregion
	
	#region Json Methods
	
	/// <summary>
	/// Parses the json object; a json which is not an object results in an empty dictionary
	/// </summary>
	internal static Dictionary<string, object?> FromJson(string json)
	{
		var buffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(json.Length));
		try
		{
			var length = Encoding.UTF8.GetBytes(json, buffer);
			var reader = new Utf8JsonReader(buffer.AsSpan(0, length), ReaderOptions);
			if (!reader.Read())
			{
				throw new JsonException("The json is empty");
			}
			
			var value = ReadValue(ref reader);
			
			// Nothing is allowed after the json value (throws JsonException on an extra token)
			reader.Read();
			
			return value as Dictionary<string, object?> ?? new Dictionary<string, object?>();
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}
	
	/// <summary>
	/// Reads the json value at the current token of the reader, the reader is left on the last token of the value
	/// </summary>
	internal static object? ReadValue(ref Utf8JsonReader reader)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.StartObject:
			{
				var dictionary = new Dictionary<string, object?>();
				while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
				{
					var propertyName = reader.GetString()!;
					reader.Read();
					dictionary[propertyName] = ReadValue(ref reader);
				}
				
				return dictionary;
			}
			case JsonTokenType.StartArray:
			{
				var items = new List<object?>();
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					items.Add(ReadValue(ref reader));
				}
				
				return items.ToArray();
			}
			case JsonTokenType.String:
				// The strings stay strings (no date detection); the schema converts its date fields
				return reader.GetString();
			case JsonTokenType.Number:
				return ReadNumber(ref reader);
			case JsonTokenType.True:
				return true;
			case JsonTokenType.False:
				return false;
			default:
				return null;
		}
	}
	
	private static object ReadNumber(ref Utf8JsonReader reader)
	{
		if (reader.TryGetInt64(out var longValue))
		{
			return longValue;
		}
		
		return TryGetLargeInteger(reader.ValueSpan, out var decimalValue) ? decimalValue : reader.GetDouble();
	}
	
	/// <summary>
	/// Gets an integer out of the Int64 range as decimal (the numbers with a fraction or an exponent are not integers)
	/// </summary>
	private static bool TryGetLargeInteger(ReadOnlySpan<byte> rawNumber, out decimal value)
	{
		if (rawNumber.IndexOfAny((byte) '.', (byte) 'e', (byte) 'E') >= 0)
		{
			value = 0;
			return false;
		}
		
		return decimal.TryParse(Encoding.UTF8.GetString(rawNumber), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
	}
	
	internal static object? FromJsonElement(JsonElement element)
	{
		switch (element.ValueKind)
		{
			case JsonValueKind.Object:
			{
				var dictionary = new Dictionary<string, object?>();
				foreach (var property in element.EnumerateObject())
				{
					dictionary[property.Name] = FromJsonElement(property.Value);
				}
				
				return dictionary;
			}
			case JsonValueKind.Array:
			{
				var array = new object?[element.GetArrayLength()];
				var index = 0;
				foreach (var item in element.EnumerateArray())
				{
					array[index++] = FromJsonElement(item);
				}
				
				return array;
			}
			case JsonValueKind.String:
				// The strings stay strings (no date detection); the schema converts its date fields
				return element.GetString();
			case JsonValueKind.Number:
				return ReadNumber(element);
			case JsonValueKind.True:
				return true;
			case JsonValueKind.False:
				return false;
			default:
				return null;
		}
	}
	
	private static object ReadNumber(JsonElement element)
	{
		if (element.TryGetInt64(out var longValue))
		{
			return longValue;
		}
		
		return TryGetLargeInteger(Encoding.UTF8.GetBytes(element.GetRawText()), out var decimalValue) ? decimalValue : element.GetDouble();
	}
	
	#endregion
	
	#region CLR Object Methods
	
	/// <summary>
	/// Converts a CLR object (dictionary, expando, anonymous object, POCO etc.) into a dictionary
	/// </summary>
	internal static Dictionary<string, object?> FromObject(object obj)
	{
		if (FromValue(obj) is Dictionary<string, object?> dictionary)
		{
			return dictionary;
		}
		
		throw new ArgumentException($"The value of type '{obj.GetType().Name}' can not be converted to a dynamic object", nameof(obj));
	}
	
	/// <summary>
	/// Converts a CLR value into the dynamic value model, keeping the primitive CLR types (numbers, dates) as they are
	/// </summary>
	internal static object? FromValue(object? value)
	{
		switch (value)
		{
			case null:
				return null;
			case string or bool or DateTime or DateTimeOffset or decimal or double or float or long or int or short or sbyte or byte or ulong or uint or ushort:
				return value;
			case nint nintValue:
				return (long) nintValue;
			case nuint nuintValue:
				return (ulong) nuintValue;
			case BigInteger bigInteger:
				return bigInteger >= (BigInteger) decimal.MinValue && bigInteger <= (BigInteger) decimal.MaxValue ? (decimal) bigInteger : (double) bigInteger;
			case char charValue:
				return charValue.ToString();
			case Guid guid:
				return guid.ToString();
			case TimeSpan timeSpan:
				return timeSpan.ToString("c", CultureInfo.InvariantCulture);
			case Uri uri:
				return uri.OriginalString;
			case Enum enumValue:
				return Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumValue.GetType()), CultureInfo.InvariantCulture);
			case byte[] bytes:
				return Convert.ToBase64String(bytes);
			case DynamicObject dynamicObject:
				return FromValue(dynamicObject.ToDictionary());
			case JsonElement jsonElement:
				return FromJsonElement(jsonElement);
			case JsonNode jsonNode:
				return FromJsonElement(JsonSerializer.SerializeToElement(jsonNode));
			case IDictionary<string, object?> genericDictionary:
			{
				var dictionary = new Dictionary<string, object?>(genericDictionary.Count);
				foreach (var (key, item) in genericDictionary)
				{
					dictionary[key] = FromValue(item);
				}
				
				return dictionary;
			}
			case IDictionary nonGenericDictionary:
			{
				var dictionary = new Dictionary<string, object?>(nonGenericDictionary.Count);
				foreach (DictionaryEntry entry in nonGenericDictionary)
				{
					dictionary[Convert.ToString(entry.Key, CultureInfo.InvariantCulture) ?? string.Empty] = FromValue(entry.Value);
				}
				
				return dictionary;
			}
			case IEnumerable enumerable:
				return enumerable.Cast<object?>().Select(FromValue).ToArray();
			default:
				return FromComplexValue(value);
		}
	}
	
	private static object? FromComplexValue(object value)
	{
		var type = value.GetType();
		
		// The types which declare a string conversion (e.g. MongoDB ObjectId) are written as strings
		var typeConverter = TypeDescriptor.GetConverter(type);
		if (typeConverter.GetType() != typeof(TypeConverter) && typeConverter is not ComponentConverter and not ReferenceConverter && typeConverter.CanConvertTo(typeof(string)))
		{
			return typeConverter.ConvertToInvariantString(value);
		}
		
		var typeInfo = ContractOptions.GetTypeInfo(type);
		if (typeInfo.Kind != JsonTypeInfoKind.Object)
		{
			return FromJsonElement(JsonSerializer.SerializeToElement(value, type, ContractOptions));
		}
		
		var dictionary = new Dictionary<string, object?>();
		foreach (var propertyInfo in typeInfo.Properties)
		{
			if (propertyInfo.Get == null)
			{
				continue;
			}
			
			var propertyValue = propertyInfo.Get(value);
			if (propertyInfo.ShouldSerialize != null && !propertyInfo.ShouldSerialize(value, propertyValue))
			{
				continue;
			}
			
			dictionary[propertyInfo.Name] = propertyInfo.CustomConverter != null
				? FromJsonElement(JsonSerializer.SerializeToElement(propertyValue, propertyInfo.PropertyType, GetConverterOptions(propertyInfo.CustomConverter)))
				: FromValue(propertyValue);
		}
		
		return dictionary;
	}
	
	private static JsonSerializerOptions CreateContractOptions()
	{
		var options = new JsonSerializerOptions
		{
			IncludeFields = true,
			TypeInfoResolver = new DefaultJsonTypeInfoResolver()
		};
		
		options.MakeReadOnly();
		return options;
	}
	
	private static JsonSerializerOptions GetConverterOptions(JsonConverter converter)
	{
		return ConverterOptions.GetOrAdd(converter, x =>
		{
			var options = new JsonSerializerOptions(ContractOptions);
			options.Converters.Add(x);
			options.MakeReadOnly();
			return options;
		});
	}
	
	#endregion
	
	#region Copy Methods
	
	/// <summary>
	/// Copies the dictionaries and arrays of the value (the other values are immutable)
	/// </summary>
	internal static object? DeepCopy(object? value)
	{
		switch (value)
		{
			case IDictionary<string, object?> dictionary:
			{
				var copy = new Dictionary<string, object?>(dictionary.Count);
				foreach (var (key, item) in dictionary)
				{
					copy[key] = DeepCopy(item);
				}
				
				return copy;
			}
			case object?[] array:
				return array.Select(DeepCopy).ToArray();
			default:
				return value;
		}
	}
	
	#endregion
	
	#region Conversion Methods
	
	/// <summary>
	/// Formats the value culture invariant (dates in ISO 8601)
	/// </summary>
	internal static string? ToInvariantString(object? value)
	{
		return value switch
		{
			null => null,
			string text => text,
			DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
			DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),
			IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
			_ => value.ToString()
		};
	}
	
	/// <summary>
	/// Converts a scalar value to the type, culture invariant
	/// </summary>
	internal static object? ConvertTo(object value, Type type)
	{
		var targetType = Nullable.GetUnderlyingType(type) ?? type;
		if (targetType.IsInstanceOfType(value))
		{
			return value;
		}
		
		if (targetType == typeof(string))
		{
			return ToInvariantString(value);
		}
		
		if (targetType.IsEnum)
		{
			return Enum.Parse(targetType, ToInvariantString(value) ?? string.Empty, ignoreCase: false);
		}
		
		if (targetType == typeof(DateTime) && value is string dateTimeString)
		{
			return DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}
		
		if (targetType == typeof(DateTimeOffset))
		{
			return value switch
			{
				string text => DateTimeOffset.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
				DateTime dateTime => new DateTimeOffset(dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime),
				_ => throw new InvalidCastException($"'{value.GetType().Name}' can not be converted to DateTimeOffset")
			};
		}
		
		if (targetType == typeof(Guid) && value is string guidString)
		{
			return Guid.Parse(guidString);
		}
		
		return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
	}
	
	#endregion
}
