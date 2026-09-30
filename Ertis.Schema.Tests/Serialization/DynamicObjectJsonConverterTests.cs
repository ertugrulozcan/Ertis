using System.Text.Json;
using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Serialization;

namespace Ertis.Schema.Tests.Serialization;

public class DynamicObjectJsonConverterTests
{
	#region Fields
	
	private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
	{
		Converters = { new DynamicObjectJsonConverter() }
	};
	
	#endregion
	
	#region Methods
	
	[Fact]
	public void Deserialize_ReadsTheObject()
	{
		const string json = """{ "id": "1", "document": { "name": "Jane", "age": 30, "address": { "city": "Istanbul" }, "tags": ["a"] } }""";
		
		var model = JsonSerializer.Deserialize<EventModel>(json, Options)!;
		
		Assert.NotNull(model.Document);
		Assert.Equal("Jane", model.Document.GetValue("name"));
		Assert.Equal(30L, model.Document.GetValue("age"));
		Assert.Equal("Istanbul", model.Document.GetValue("address.city"));
	}
	
	[Fact]
	public void Deserialize_Null_ReturnsNull()
	{
		var model = JsonSerializer.Deserialize<EventModel>("""{ "id": "1", "document": null }""", Options)!;
		
		Assert.Null(model.Document);
	}
	
	[Fact]
	public void Serialize_WritesTheObjectInline()
	{
		var model = new EventModel { Id = "1", Document = DynamicObject.Parse("""{ "name": "Jane", "address": { "city": "Istanbul" } }""") };
		
		var json = JsonSerializer.Serialize(model, Options);
		
		Assert.Equal("""{"id":"1","document":{"name":"Jane","address":{"city":"Istanbul"}}}""", json);
	}
	
	[Fact]
	public void RoundTrip_PreservesTheJson()
	{
		const string json = """{"id":"1","document":{"name":"Jane","age":30,"ratio":0.5,"active":true,"nothing":null,"address":{"city":"Istanbul"},"tags":["a","b"],"phones":[{"number":"555"}]}}""";
		
		var model = JsonSerializer.Deserialize<EventModel>(json, Options);
		
		Assert.Equal(json, JsonSerializer.Serialize(model, Options));
	}
	
	[Fact(Skip = "Bug (backlog #9): date-time strings are read as DateTime and written back in another format")]
	public void RoundTrip_PreservesDateTimeStrings()
	{
		const string json = """{"id":"1","document":{"startsAt":"2026-01-31T10:00:00.000Z"}}""";
		
		var model = JsonSerializer.Deserialize<EventModel>(json, Options);
		
		Assert.Equal(json, JsonSerializer.Serialize(model, Options));
	}
	
	#endregion
	
	#region Test Types
	
	public sealed class EventModel
	{
		[JsonPropertyName("id")]
		public required string Id { get; init; }
		
		[JsonPropertyName("document")]
		public DynamicObject? Document { get; init; }
	}
	
	#endregion
}
