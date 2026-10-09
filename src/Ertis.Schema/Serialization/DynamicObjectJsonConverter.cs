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
		
		return DynamicObject.Read(ref reader);
	}
	
	public override void Write(Utf8JsonWriter writer, DynamicObject dynamicObject, JsonSerializerOptions options)
	{
		// ToJson is written by the serializer, so it is a valid json and doesn't need a validation
		writer.WriteRawValue(dynamicObject.ToJson(), skipInputValidation: true);
	}
	
	#endregion
}