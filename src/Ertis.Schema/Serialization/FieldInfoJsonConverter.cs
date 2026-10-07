using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;
using Ertis.Schema.Types.CustomTypes;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Serialization;

public class FieldInfoJsonConverter : JsonConverter<IFieldInfo>
{
	#region Methods
	
	public override IFieldInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		try
		{
			var tempReader = reader;
			if (tempReader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}
			
			var depth = 1;
			while (tempReader.Read())
			{
				// ReSharper disable once ConvertIfStatementToSwitchStatement
				if (tempReader.TokenType == JsonTokenType.PropertyName)
				{
					var propertyName = tempReader.GetString();
					if (propertyName == "type" && depth == 1)
					{
						tempReader.Read();
						var fieldTypeName = tempReader.GetString();
						if (!string.IsNullOrEmpty(fieldTypeName))
						{
							// Only the exact type names (Enum.TryParse accepts the numeric values too)
							if (Enum.TryParse(fieldTypeName, out FieldType fieldType) && fieldType.ToString() == fieldTypeName)
							{
								var type = fieldType switch
								{
									FieldType.@object => typeof(ObjectFieldInfo),
									FieldType.@string => typeof(StringFieldInfo),
									FieldType.integer => typeof(IntegerFieldInfo),
									FieldType.@float => typeof(FloatFieldInfo),
									FieldType.boolean => typeof(BooleanFieldInfo),
									FieldType.array => typeof(ArrayFieldInfo),
									FieldType.@enum => typeof(EnumFieldInfo),
									FieldType.@const => typeof(ConstantFieldInfo),
									FieldType.tags => typeof(TagsFieldInfo),
									FieldType.json => typeof(JsonFieldInfo),
									FieldType.date => typeof(DateFieldInfo),
									FieldType.datetime => typeof(DateTimeFieldInfo),
									FieldType.longtext => typeof(LongTextFieldInfo),
									FieldType.richtext => typeof(RichTextFieldInfo),
									FieldType.email => typeof(EmailAddressFieldInfo),
									FieldType.uri => typeof(UriFieldInfo),
									FieldType.hostname => typeof(HostNameFieldInfo),
									FieldType.color => typeof(ColorFieldInfo),
									FieldType.location => typeof(LocationFieldInfo),
									FieldType.reference => typeof(ReferenceFieldInfo),
									FieldType.code => typeof(CodeFieldInfo),
									FieldType.image => typeof(ImageFieldInfo),
									FieldType.video => typeof(VideoFieldInfo),
									
									_ => throw new SchemaValidationException($"Unknown field type : '{fieldTypeName}'")
								};
								
								if (type == typeof(ArrayFieldInfo))
								{
									return ReadArrayFieldInfo(ref reader, options);
								}
								
								return (IFieldInfo) JsonSerializer.Deserialize(ref reader, type, options)!;
							}
							else
							{
								throw new SchemaValidationException($"Unknown field type : '{fieldTypeName}'");
							}
						}
						else
						{
							throw new SchemaValidationException("Field info type missing");
						}
					}
				}
				else if (tempReader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
				{
					depth++;
				}
				else if (tempReader.TokenType is JsonTokenType.EndObject or JsonTokenType.EndArray)
				{
					depth--;
				}
			}
			
			throw new SchemaValidationException("Field info type missing");
		}
		catch (Exception ex) when (ex is not FieldValidationException and not SchemaValidationException)
		{
			if (ex.InnerException is FieldValidationException or SchemaValidationException)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
			}
			
			throw new SchemaValidationException($"FieldInfo cannot be deserialized: {ex.Message}", ex);
		}
	}
	
	/// <summary>
	/// The item schema of an array is named by the library ('$schema') when the json has no name for it
	/// (the item schema is read by this converter too, so the arrays of arrays are named on every level)
	/// </summary>
	private static IFieldInfo ReadArrayFieldInfo(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		var node = JsonNode.Parse(ref reader)!.AsObject();
		if (node["itemSchema"] is JsonObject itemSchema && !itemSchema.ContainsKey("name"))
		{
			itemSchema["name"] = "$schema";
		}
		
		return node.Deserialize<ArrayFieldInfo>(options)!;
	}
	
	public override void Write(Utf8JsonWriter writer, IFieldInfo fieldInfo, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, fieldInfo, fieldInfo.GetType(), options);
	}
	
	#endregion
}