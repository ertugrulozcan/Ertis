using System.Text.Json;
using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;

// ReSharper disable UnusedType.Global
namespace Ertis.Schema.Serialization;

public class DynamicObjectJsonConverter : JsonConverter<DynamicObject>
{
	#region Methods
	
	public override bool CanConvert(Type typeToConvert)
	{
		return typeof(DynamicObject).IsAssignableFrom(typeToConvert);
	}
	
	public override DynamicObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}
		
		using var document = JsonDocument.ParseValue(ref reader);
		return DynamicObject.FromJsonElement(document.RootElement);
	}
	
	public override void Write(Utf8JsonWriter writer, DynamicObject dynamicObject, JsonSerializerOptions options)
	{
		writer.WriteRawValue(dynamicObject.ToJson(), skipInputValidation: false);
	}
	
	#endregion
}