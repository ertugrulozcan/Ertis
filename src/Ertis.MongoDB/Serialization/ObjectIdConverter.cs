using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace Ertis.MongoDB.Serialization;

// ReSharper disable once UnusedType.Global
public class ObjectIdConverter : JsonConverter<ObjectId>
{
	#region Methods
	
	public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => ObjectId.Parse(reader.GetString());
	
	public override void Write(Utf8JsonWriter writer, ObjectId objectId, JsonSerializerOptions options) => writer.WriteStringValue(objectId.ToString());
	
	#endregion
}