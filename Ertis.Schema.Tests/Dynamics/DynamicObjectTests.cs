using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Tests.TestHelpers;

namespace Ertis.Schema.Tests.Dynamics;

public class DynamicObjectTests
{
	#region Constants
	
	private const string SAMPLE_JSON =
		"""
		{
			"name": "Jane",
			"age": 30,
			"ratio": 0.5,
			"active": true,
			"nothing": null,
			"address": { "city": "Istanbul", "geo": { "zip": 34000 } },
			"tags": ["a", "b"],
			"phones": [{ "number": "555" }, { "number": "556" }]
		}
		""";
	
	#endregion
	
	#region Parse Methods
	
	[Fact]
	public void Parse_MapsJsonValuesToClrTypes()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.IsType<string>(dynamicObject.GetValue("name"));
		Assert.IsType<long>(dynamicObject.GetValue("age"));
		Assert.IsType<double>(dynamicObject.GetValue("ratio"));
		Assert.IsType<bool>(dynamicObject.GetValue("active"));
		Assert.Null(dynamicObject.GetValue("nothing"));
		Assert.IsType<Dictionary<string, object?>>(dynamicObject.GetValue("address"));
		Assert.IsType<object[]>(dynamicObject.GetValue("tags"));
	}
	
	[Fact]
	public void Create_WithNestedDictionary_KeepsTheStructure()
	{
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?>
		{
			["name"] = "Jane",
			["address"] = new Dictionary<string, object?> { ["city"] = "Istanbul" }
		});
		
		Assert.Equal("Istanbul", dynamicObject.GetValue("address.city"));
	}
	
	[Fact]
	public void Constructor_WithPlainObject_ReadsItsProperties()
	{
		var dynamicObject = new DynamicObject(new { Name = "Jane", Address = new { City = "Istanbul" } });
		
		Assert.Equal("Jane", dynamicObject.GetValue("Name"));
		Assert.Equal("Istanbul", dynamicObject.GetValue("Address.City"));
	}
	
	[Fact]
	public void Constructor_WithDynamicObject_SharesTheSameProperties()
	{
		var original = DynamicObject.Parse("""{ "name": "Jane" }""");
		
		var copy = new DynamicObject(original);
		copy.SetValue("name", "John");
		
		Assert.Equal("John", original.GetValue("name"));
	}
	
	#endregion
	
	#region GetValue Methods
	
	[Theory]
	[InlineData("name", "Jane")]
	[InlineData("address.city", "Istanbul")]
	[InlineData("address.geo.zip", 34000L)]
	[InlineData("tags[1]", "b")]
	[InlineData("phones[1].number", "556")]
	public void GetValue_WithPath_ReturnsTheValue(string path, object expected)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal(expected, dynamicObject.GetValue(path));
		Assert.Equal(expected, dynamicObject[path]);
	}
	
	[Theory]
	[InlineData("missing")]
	[InlineData("address.missing")]
	[InlineData("name.first")]
	public void GetValue_WithUndefinedPath_ThrowsUndefinedFieldException(string path)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<UndefinedFieldException>(() => dynamicObject.GetValue(path));
	}
	
	[Theory]
	[InlineData("tags[5]", "Out of range (length: 2, index: 5)")]
	[InlineData("tags[x]", "Array index is not valid integer ('x')")]
	[InlineData("name[0]", "Indexed node is not an array")]
	public void GetValue_WithInvalidIndexer_ThrowsInvalidOperationException(string path, string expectedMessage)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		var exception = Assert.Throws<InvalidOperationException>(() => dynamicObject.GetValue(path));
		
		Assert.Equal(expectedMessage, exception.Message);
	}
	
	[Fact]
	public void GetValue_WithEmptyPath_ThrowsArgumentException()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<ArgumentException>(() => dynamicObject.GetValue(string.Empty));
	}
	
	[Fact]
	public void GetValue_WithDefaultForUndefinedPath_ReturnsTheDefault()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal("none", dynamicObject.GetValue("missing", (object) "none"));
		Assert.Equal(7, dynamicObject.GetValue("missing", 7));
	}
	
	[Fact]
	public void GetValueOfT_ConvertsTheValue()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal(30, dynamicObject.GetValue<int>("age"));
		Assert.Equal("30", dynamicObject.GetValue<string>("age"));
		Assert.Equal(DayOfWeek.Monday, DynamicObject.Parse("""{ "day": "Monday" }""").GetValue<DayOfWeek>("day"));
		Assert.Equal(["a", "b"], dynamicObject.GetValue<string[]>("tags")!);
		Assert.Null(dynamicObject.GetValue<string>("nothing"));
	}
	
	[Fact]
	public void GetValueOfT_WithObjectValue_DeserializesIt()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		var address = dynamicObject.GetValue<Address>("address");
		
		Assert.NotNull(address);
		Assert.Equal("Istanbul", address.City);
	}
	
	[Fact(Skip = "Bug (finding #12): GetValue<T[]> of object items converts the items but returns an object[], the cast to T[] throws InvalidCastException")]
	public void GetValueOfT_WithObjectArray_DeserializesTheItems()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		var phones = dynamicObject.GetValue<Phone[]>("phones");
		
		Assert.NotNull(phones);
		Assert.Equal(["555", "556"], phones.Select(x => x.Number));
	}
	
	[Fact(Skip = "Bug (finding #12): GetValue<T[]> of primitive items returns an object[], the cast to T[] throws InvalidCastException")]
	public void GetValueOfT_WithPrimitiveArray_ConvertsTheItems()
	{
		var dynamicObject = DynamicObject.Parse("""{ "numbers": [1, 2] }""");
		
		Assert.Equal([1L, 2L], dynamicObject.GetValue<long[]>("numbers")!);
	}
	
	[Fact(Skip = "Bug (finding #8): GetValue<string> formats numbers with the current culture ('0,5' under tr-TR)")]
	public void GetValueOfT_StringOfDouble_IsCultureInvariant()
	{
		using var _ = new CultureScope("tr-TR");
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal("0.5", dynamicObject.GetValue<string>("ratio"));
	}
	
	[Theory]
	[InlineData("name", true)]
	[InlineData("address.geo.zip", true)]
	[InlineData("missing", false)]
	[InlineData("address.missing", false)]
	public void TryGetValue_ReportsWhetherThePathExists(string path, bool expected)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal(expected, dynamicObject.TryGetValue(path, out _));
		Assert.Equal(expected, dynamicObject.TryGetValue(path, out _, out var exception));
		Assert.Equal(expected, exception == null);
		Assert.Equal(expected, dynamicObject.ContainsProperty(path));
	}
	
	#endregion
	
	#region SetValue Methods
	
	[Theory]
	[InlineData("name", "John")]
	[InlineData("address.city", "Ankara")]
	[InlineData("tags[0]", "z")]
	[InlineData("phones[0].number", "999")]
	public void SetValue_WithExistingPath_ReplacesTheValue(string path, string value)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.SetValue(path, value);
		
		Assert.Equal(value, dynamicObject.GetValue(path));
	}
	
	[Fact]
	public void SetValue_WithUndefinedPath_Throws()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<UndefinedFieldException>(() => dynamicObject.SetValue("missing", 1));
		Assert.False(dynamicObject.TrySetValue("missing", 1, out var exception));
		Assert.IsType<UndefinedFieldException>(exception);
	}
	
	[Theory]
	[InlineData("missing")]
	[InlineData("address.zip")]
	public void SetValue_WithCreateIfNotExist_AddsTheProperty(string path)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.SetValue(path, 1, createIfNotExist: true);
		
		Assert.Equal(1, dynamicObject.GetValue(path));
	}
	
	[Fact]
	public void SetValue_WithCreateIfNotExistAndMissingParent_AddsTheParentAndTheProperty()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.SetValue("contact.email", "jane@example.com", createIfNotExist: true);
		dynamicObject.SetValue("a.b.c", 1, createIfNotExist: true);
		
		Assert.Equal("jane@example.com", dynamicObject.GetValue("contact.email"));
		Assert.Equal(1, dynamicObject.GetValue("a.b.c"));
	}
	
	[Theory]
	[InlineData("name")]
	[InlineData("address.city")]
	public void RemoveProperty_RemovesTheProperty(string path)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.RemoveProperty(path);
		
		Assert.False(dynamicObject.ContainsProperty(path));
	}
	
	[Fact]
	public void RemoveProperty_WithUndefinedPath_DoesNothing()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.RemoveProperty("missing.path");
		
		Assert.Equal("Jane", dynamicObject.GetValue("name"));
	}
	
	#endregion
	
	#region Conversion Methods
	
	[Fact]
	public void ToJson_WritesAllProperties()
	{
		var dynamicObject = DynamicObject.Parse("""{ "name": "Jane", "age": 30, "address": { "city": "Istanbul" }, "tags": ["a"], "nothing": null }""");
		
		Assert.Equal("""{"name":"Jane","age":30,"address":{"city":"Istanbul"},"tags":["a"],"nothing":null}""", dynamicObject.ToJson());
	}
	
	[Fact]
	public void Deserialize_MapsToTheType()
	{
		var dynamicObject = DynamicObject.Parse("""{ "name": "Jane", "address": { "city": "Istanbul" } }""");
		
		var person = dynamicObject.Deserialize<Person>();
		
		Assert.NotNull(person);
		Assert.Equal("Jane", person.Name);
		Assert.Equal("Istanbul", person.Address?.City);
		Assert.Equal(person, dynamicObject.Deserialize(typeof(Person)));
	}
	
	[Fact]
	public void Clone_CreatesAnIndependentCopy()
	{
		var original = DynamicObject.Parse(SAMPLE_JSON);
		
		var clone = (DynamicObject) original.Clone();
		clone.SetValue("address.city", "Ankara");
		
		Assert.Equal("Istanbul", original.GetValue("address.city"));
		Assert.Equal("Ankara", clone.GetValue("address.city"));
	}
	
	[Fact]
	public void ToDynamic_ReturnsAnExpandoObject()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		var expando = dynamicObject.ToDynamic();
		
		Assert.Equal("Jane", (string) expando.name);
		Assert.Equal("Istanbul", (string) expando.address.city);
	}
	
	[Fact]
	public void Merge_CombinesNestedObjects()
	{
		var first = DynamicObject.Parse("""{ "name": "Jane", "address": { "city": "Istanbul", "zip": 34000 }, "keep": 1 }""");
		var second = DynamicObject.Parse("""{ "name": "John", "address": { "city": "Ankara" }, "added": 2 }""");
		
		var merged = first.Merge(second);
		
		Assert.Equal("John", merged.GetValue("name"));
		Assert.Equal("Ankara", merged.GetValue("address.city"));
		Assert.Equal(34000L, merged.GetValue("address.zip"));
		Assert.Equal(1L, merged.GetValue("keep"));
		Assert.Equal(2L, merged.GetValue("added"));
	}
	
	#endregion
	
	#region Test Types
	
	public sealed record Address([property: JsonPropertyName("city")] string City);
	
	public sealed record Phone([property: JsonPropertyName("number")] string Number);
	
	public sealed record Person([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("address")] Address? Address);
	
	#endregion
}
