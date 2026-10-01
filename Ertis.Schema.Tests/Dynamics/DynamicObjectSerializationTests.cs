using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Schema.Tests.Dynamics;

/// <summary>
/// The json output of DynamicObject (ToJson / Serialize) and the typed deserialization (Deserialize)
/// </summary>
public class DynamicObjectSerializationTests
{
	#region ToJson Methods
	
	[Fact]
	public void ToJson_KeepsThePropertyOrderAndStructure()
	{
		var dynamicObject = DynamicObject.Parse("""{ "b": 1, "a": { "d": [], "c": {} }, "list": [{ "x": 1 }, [1, [2]], null] }""");
		
		Assert.Equal("""{"b":1,"a":{"d":[],"c":{}},"list":[{"x":1},[1,[2]],null]}""", dynamicObject.ToJson());
		Assert.Equal(dynamicObject.ToJson(), dynamicObject.Serialize());
	}
	
	[Fact]
	public void ToJson_OfEmptyObject_WritesAnEmptyObject()
	{
		Assert.Equal("{}", DynamicObject.Parse("{}").ToJson());
	}
	
	[Theory]
	[InlineData("Şule Çağ", "\\u015Eule \\u00C7a\\u011F")]
	[InlineData("<b>&'", "\\u003Cb\\u003E\\u0026\\u0027")]
	[InlineData("quote \" back \\ slash", "quote \\u0022 back \\\\ slash")]
	[InlineData("line\nbreak\ttab", "line\\nbreak\\ttab")]
	public void ToJson_EscapesTheStrings(string value, string expectedJsonString)
	{
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?> { ["text"] = value });
		
		Assert.Equal($$"""{"text":"{{expectedJsonString}}"}""", dynamicObject.ToJson());
	}
	
	[Fact]
	public void ToJson_WritesTheNumbers()
	{
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?>
		{
			["long"] = 9007199254740993L,
			["int"] = 5,
			["double"] = 0.1,
			["large"] = 1e20,
			["decimal"] = 0.50m,
			["negative"] = -2.5,
			["bool"] = true
		});
		
		Assert.Equal("""{"long":9007199254740993,"int":5,"double":0.1,"large":1E+20,"decimal":0.50,"negative":-2.5,"bool":true}""", dynamicObject.ToJson());
	}
	
	[Fact]
	public void ToJson_WritesTheDatesInIso8601()
	{
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?>
		{
			["utc"] = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc),
			["fraction"] = new DateTime(2026, 1, 31, 10, 0, 0, 123, DateTimeKind.Utc),
			["unspecified"] = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Unspecified),
			["offset"] = new DateTimeOffset(2026, 1, 31, 10, 0, 0, TimeSpan.FromHours(3))
		});
		
		Assert.Equal("""{"utc":"2026-01-31T10:00:00Z","fraction":"2026-01-31T10:00:00.123Z","unspecified":"2026-01-31T10:00:00","offset":"2026-01-31T10:00:00+03:00"}""", dynamicObject.ToJson());
	}
	
	[Fact]
	public void ToJson_WritesLocalDatesWithTheLocalOffset()
	{
		var local = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Local);
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?> { ["local"] = local });
		
		Assert.Equal($$"""{"local":"{{local.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture)}}"}""", dynamicObject.ToJson());
	}
	
	[Fact]
	public void ToJson_WritesTheRawValuesSetWithSetValue()
	{
		var dynamicObject = DynamicObject.Parse("{}");
		dynamicObject.SetValue("guid", Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), createIfNotExist: true);
		dynamicObject.SetValue("day", DayOfWeek.Monday, createIfNotExist: true);
		dynamicObject.SetValue("poco", new RawPoco { Name = "x", Count = 2 }, createIfNotExist: true);
		dynamicObject.SetValue("list", new List<string> { "a" }, createIfNotExist: true);
		
		Assert.Equal("""{"guid":"0f8fad5b-d9cb-469f-a165-70867728950e","day":1,"poco":{"Name":"x","Count":2},"list":["a"]}""", dynamicObject.ToJson());
	}
	
	#endregion
	
	#region Deserialize Methods
	
	[Fact]
	public void Deserialize_MapsTheTypedProperties()
	{
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?>
		{
			["name"] = "Jane",
			["age"] = 30L,
			["ratio"] = 0.5,
			["createdAt"] = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc),
			["status"] = 2L,
			["tags"] = new object[] { "a", "b" },
			["address"] = new Dictionary<string, object?> { ["city"] = "Istanbul" },
			["nickname"] = null,
			["extra"] = "ignored"
		});
		
		var model = dynamicObject.Deserialize<TypedModel>();
		
		Assert.NotNull(model);
		Assert.Equal("Jane", model.Name);
		Assert.Equal(30, model.Age);
		Assert.Equal(0.5m, model.Ratio);
		Assert.Equal(new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc), model.CreatedAt);
		Assert.Equal(DateTimeKind.Utc, model.CreatedAt.Kind);
		Assert.Equal(DayOfWeek.Tuesday, model.Status);
		Assert.Equal(["a", "b"], model.Tags!);
		Assert.Equal("Istanbul", model.Address?.City);
		Assert.Null(model.Nickname);
	}
	
	[Fact]
	public void Deserialize_IsCaseSensitive()
	{
		var dynamicObject = DynamicObject.Parse("""{ "Name": "Jane", "name2": "x" }""");
		
		var model = dynamicObject.Deserialize<TypedModel>();
		
		Assert.NotNull(model);
		Assert.Null(model.Name);
	}
	
	[Fact]
	public void Deserialize_WithoutAttributes_UsesThePropertyNames()
	{
		var dynamicObject = DynamicObject.Parse("""{ "Name": "x", "count": 2, "Count": 3 }""");
		
		var model = dynamicObject.Deserialize<RawPoco>();
		
		Assert.NotNull(model);
		Assert.Equal("x", model.Name);
		Assert.Equal(3, model.Count);
	}
	
	[Fact]
	public void Deserialize_UnspecifiedDateTime_StaysUnspecified()
	{
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?> { ["createdAt"] = new DateTime(2026, 1, 31, 10, 0, 0) });
		
		var model = dynamicObject.Deserialize<TypedModel>();
		
		Assert.Equal(DateTimeKind.Unspecified, model?.CreatedAt.Kind);
	}
	
	[Fact]
	public void Deserialize_EnumFromString_Throws()
	{
		var dynamicObject = DynamicObject.Parse("""{ "status": "Monday" }""");
		
		// ReSharper disable once ConvertClosureToMethodGroup
		Assert.Throws<JsonException>(() => dynamicObject.Deserialize<TypedModel>());
	}
	
	[Fact]
	public void Deserialize_ToDictionary_ReturnsJsonElements()
	{
		var dynamicObject = DynamicObject.Parse("""{ "a": 1, "b": { "c": "x" } }""");
		
		var dictionary = dynamicObject.Deserialize<Dictionary<string, object>>();
		
		Assert.NotNull(dictionary);
		Assert.IsType<JsonElement>(dictionary["a"]);
		Assert.Equal(JsonValueKind.Object, ((JsonElement) dictionary["b"]).ValueKind);
	}
	
	[Fact]
	public void DeserializeByType_MapsTheType()
	{
		var dynamicObject = DynamicObject.Parse("""{ "name": "Jane" }""");
		
		var model = dynamicObject.Deserialize(typeof(TypedModel));
		
		Assert.Equal("Jane", Assert.IsType<TypedModel>(model).Name);
	}
	
	#endregion
	
	#region Test Types
	
	public sealed class TypedModel
	{
		[JsonPropertyName("name")]
		public string? Name { get; set; }
		
		[JsonPropertyName("age")]
		public int Age { get; set; }
		
		[JsonPropertyName("ratio")]
		public decimal Ratio { get; set; }
		
		[JsonPropertyName("createdAt")]
		public DateTime CreatedAt { get; set; }
		
		[JsonPropertyName("status")]
		public DayOfWeek Status { get; set; }
		
		[JsonPropertyName("tags")]
		public string[]? Tags { get; set; }
		
		[JsonPropertyName("address")]
		public TypedAddress? Address { get; set; }
		
		[JsonPropertyName("nickname")]
		public string? Nickname { get; set; }
	}
	
	public sealed class TypedAddress
	{
		[JsonPropertyName("city")]
		public string? City { get; set; }
	}
	
	public sealed class RawPoco
	{
		public string? Name { get; set; }
		
		public int Count { get; set; }
	}
	
	#endregion
}
