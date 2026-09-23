using System.Text.Json;
using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;

// ReSharper disable UnusedType.Global
namespace Ertis.Schema.Serialization;

public class DynamicObjectJsonConverter : JsonConverter<DynamicObject>
{
	#region Methods
	
	public override DynamicObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var json = reader.GetString();
		return string.IsNullOrEmpty(json) ? null : DynamicObject.Parse(json);
	}
	
	public override void Write(Utf8JsonWriter writer, DynamicObject dynamicObject, JsonSerializerOptions options)
	{
		writer.WriteStringValue(dynamicObject.ToJson());
	}
	
	#endregion
}