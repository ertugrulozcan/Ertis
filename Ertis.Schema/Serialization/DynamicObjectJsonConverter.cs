using Ertis.Schema.Dynamics.Legacy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Serialization;

public class DynamicObjectJsonConverter : JsonConverter<DynamicObject>
{
	#region Methods
	
	public override void WriteJson(JsonWriter writer, DynamicObject value, JsonSerializer serializer)
	{
		var jToken = JToken.Parse(value.ToJson());
		jToken.WriteTo(writer);
	}
	
	public override DynamicObject ReadJson(JsonReader reader, Type objectType, DynamicObject existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		var jObject = JObject.Load(reader);
		var json = jObject.ToString(Formatting.None);
		return DynamicObject.Parse(json);
	}
	
	#endregion
}