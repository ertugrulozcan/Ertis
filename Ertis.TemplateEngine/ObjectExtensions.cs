using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Ertis.TemplateEngine;

/// <summary>
/// Converts the template data (dictionaries, expando objects, anonymous objects, POCOs) into dictionaries without a json round trip,
/// keeping the CLR values as they are (numbers, dates, booleans); the POCO property names come from the System.Text.Json contract ([JsonPropertyName])
/// </summary>
internal static class ObjectExtensions
{
	#region Fields
	
	private static readonly JsonSerializerOptions ContractOptions = CreateContractOptions();
	
	private static readonly ConcurrentDictionary<JsonConverter, JsonSerializerOptions> ConverterOptions = new();
	
	#endregion
	
	#region Methods
	
	public static IDictionary<string, object?> ToDictionary(this object model)
	{
		if (ToValue(model) is Dictionary<string, object?> dictionary)
		{
			return dictionary;
		}
		
		throw new ArgumentException($"The template data of type '{model.GetType().Name}' is not an object", nameof(model));
	}
	
	private static object? ToValue(object? value)
	{
		switch (value)
		{
			case null:
				return null;
			case string or bool or char or DateTime or DateTimeOffset or DateOnly or TimeOnly or TimeSpan or Guid or Uri or decimal or double or float or long or int or short or sbyte or byte or ulong or uint or ushort:
				return value;
			case Enum enumValue:
				// Written as its number (a [JsonConverter] on the property writes its name)
				return Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumValue.GetType()), CultureInfo.InvariantCulture);
			case JsonElement jsonElement:
				return FromJsonElement(jsonElement);
			case JsonNode jsonNode:
				return FromJsonElement(JsonSerializer.SerializeToElement(jsonNode));
			case IDictionary<string, object?> genericDictionary:
			{
				var dictionary = new Dictionary<string, object?>(genericDictionary.Count);
				foreach (var (key, item) in genericDictionary)
				{
					dictionary[key] = ToValue(item);
				}
				
				return dictionary;
			}
			case IDictionary nonGenericDictionary:
			{
				var dictionary = new Dictionary<string, object?>(nonGenericDictionary.Count);
				foreach (DictionaryEntry entry in nonGenericDictionary)
				{
					dictionary[Convert.ToString(entry.Key, CultureInfo.InvariantCulture) ?? string.Empty] = ToValue(entry.Value);
				}
				
				return dictionary;
			}
			case IEnumerable enumerable:
				return enumerable.Cast<object?>().Select(ToValue).ToArray();
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
				: ToValue(propertyValue);
		}
		
		return dictionary;
	}
	
	private static object? FromJsonElement(JsonElement element)
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
				return element.EnumerateArray().Select(FromJsonElement).ToArray();
			case JsonValueKind.String:
				return element.GetString();
			case JsonValueKind.Number:
				return element.TryGetInt64(out var longValue) ? longValue : element.GetDouble();
			case JsonValueKind.True:
				return true;
			case JsonValueKind.False:
				return false;
			default:
				return null;
		}
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
}
