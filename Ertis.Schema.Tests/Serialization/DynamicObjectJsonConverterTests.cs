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
	
	[Fact]
	public void RoundTrip_PreservesDateTimeStrings()
	{
		const string json = """{"id":"1","document":{"startsAt":"2026-01-31T10:00:00.000Z"}}""";
		
		var model = JsonSerializer.Deserialize<EventModel>(json, Options);
		
		Assert.Equal(json, JsonSerializer.Serialize(model, Options));
	}
	
	[Fact]
	public void Deserialize_DynamicObjectInTheMiddleOfAModel_ReadsTheFollowingProperties()
	{
		const string json = """{ "id": "1", "document": { "a": { "b": [1, { "c": 2 }] } }, "after": "x", "items": [{ "n": 1 }, null, { "n": 2 }] }""";
		
		var model = JsonSerializer.Deserialize<WideModel>(json, Options)!;
		
		Assert.Equal(2L, model.Document?.GetValue("a.b[1].c"));
		Assert.Equal("x", model.After);
		Assert.NotNull(model.Items);
		Assert.Equal(3, model.Items.Count);
		Assert.Equal(1L, model.Items[0]?.GetValue("n"));
		Assert.Null(model.Items[1]);
		Assert.Equal(2L, model.Items[2]?.GetValue("n"));
	}
	
	[Theory]
	[InlineData("5")]
	[InlineData("\"text\"")]
	[InlineData("[1, 2]")]
	[InlineData("true")]
	public void Deserialize_NonObjectValue_ReadsAnEmptyObject(string jsonValue)
	{
		var model = JsonSerializer.Deserialize<WideModel>($$"""{ "document": {{jsonValue}}, "after": "x" }""", Options)!;
		
		Assert.NotNull(model.Document);
		Assert.Empty(model.Document.ToDictionary());
		Assert.Equal("x", model.After);
	}
	
	[Fact]
	public void Deserialize_EscapedStrings_AreUnescaped()
	{
		var model = JsonSerializer.Deserialize<EventModel>("""{ "id": "1", "document": { "text": "line\nbreak \u015Eule \"q\"" } }""", Options)!;
		
		Assert.Equal("line\nbreak Şule \"q\"", model.Document?.GetValue("text"));
	}
	
	[Fact]
	public void Serialize_ListOfDynamicObjects_WritesEachObject()
	{
		var model = new WideModel { Items = [DynamicObject.Parse("""{ "n": 1 }"""), null] };
		
		var json = JsonSerializer.Serialize(model, Options);
		
		Assert.Equal("""{"id":null,"document":null,"after":null,"items":[{"n":1},null]}""", json);
	}
	
	#endregion
	
	#region Without Registration
	
	// The converter is declared on the DynamicObject type: it is used with any options, also without being registered
	
	[Fact]
	public void Serialize_WithoutRegisteredConverter_WritesTheObject()
	{
		var payload = new { user = DynamicObject.Parse("""{ "username": "jane", "age": 30 }"""), items = new[] { DynamicObject.Parse("""{ "n": 1 }""") } };
		
		var json = JsonSerializer.Serialize(payload);
		
		Assert.Equal("""{"user":{"username":"jane","age":30},"items":[{"n":1}]}""", json);
	}
	
	[Fact]
	public void Deserialize_WithoutRegisteredConverter_ReadsTheObject()
	{
		var model = JsonSerializer.Deserialize<EventModel>("""{ "id": "1", "document": { "name": "Jane", "address": { "city": "Istanbul" } } }""")!;
		
		Assert.NotNull(model.Document);
		Assert.Equal("Istanbul", model.Document.GetValue("address.city"));
	}
	
	[Fact]
	public void ToJson_WithDynamicObjectSetAsAValue_WritesItsFields()
	{
		var dynamicObject = DynamicObject.Parse("""{ "name": "Jane" }""");
		dynamicObject.SetValue("address", DynamicObject.Parse("""{ "city": "Istanbul" }"""), createIfNotExist: true);
		
		Assert.Equal("""{"name":"Jane","address":{"city":"Istanbul"}}""", dynamicObject.ToJson());
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
	
	public sealed class WideModel
	{
		[JsonPropertyName("id")]
		public string? Id { get; init; }
		
		[JsonPropertyName("document")]
		public DynamicObject? Document { get; init; }
		
		[JsonPropertyName("after")]
		public string? After { get; init; }
		
		[JsonPropertyName("items")]
		public List<DynamicObject?>? Items { get; init; }
	}
	
	#endregion
}
