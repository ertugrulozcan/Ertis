using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;

using LegacyFieldInfoCollectionJsonConverter = Ertis.Schema.Serialization.Legacy.FieldInfoCollectionJsonConverter;

namespace Ertis.Schema.Tests.Serialization;

public class FieldInfoJsonConverterTests
{
	#region Fields
	
	/// <summary>
	/// The options the consumers use (e.g. ErtisAuth registers FieldInfoJsonConverter and DynamicObjectJsonConverter)
	/// </summary>
	private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
	{
		Converters = { new FieldInfoJsonConverter(), new DynamicObjectJsonConverter() }
	};
	
	#endregion
	
	#region Round Trip Methods
	
	public static TheoryData<string> FieldNames()
	{
		return new TheoryData<string>(SampleFields.All().Select(x => x.Name));
	}
	
	[Theory]
	[MemberData(nameof(FieldNames))]
	public void RoundTrip_PreservesTheFieldTypeAndJson(string fieldName)
	{
		var properties = new PropertiesModel { Properties = SampleFields.All() };
		var json = JsonSerializer.Serialize(properties, Options);
		
		var deserialized = JsonSerializer.Deserialize<PropertiesModel>(json, Options)!;
		var original = properties.Properties.Single(x => x.Name == fieldName);
		var roundTripped = deserialized.Properties.Single(x => x.Name == fieldName);
		
		Assert.IsType(original.GetType(), roundTripped);
		Assert.Equal(FieldJson(json, fieldName), FieldJson(JsonSerializer.Serialize(deserialized, Options), fieldName));
	}
	
	[Fact]
	public void Serialize_WritesPropertiesAsAnObjectKeyedByName()
	{
		var properties = new PropertiesModel { Properties = [new StringFieldInfo { Name = "title", MaxLength = 5 }] };
		
		var json = JsonSerializer.Serialize(properties, Options);
		
		Assert.Equal("""{"properties":{"title":{"type":"string","maxLength":5}}}""", json);
	}
	
	[Fact]
	public void Serialize_ArrayItemSchema_HasTheSchemaName()
	{
		var properties = new PropertiesModel { Properties = [new ArrayFieldInfo { Name = "tags", ItemSchema = new StringFieldInfo { Name = "$schema" } }] };
		
		var json = JsonSerializer.Serialize(properties, Options);
		
		Assert.Equal("""{"properties":{"tags":{"type":"array","itemSchema":{"type":"string","name":"$schema"}}}}""", json);
	}
	
	[Fact]
	public void Deserialize_ArrayWithoutItemSchemaName_UsesTheSchemaName()
	{
		const string json = """{ "properties": { "tags": { "type": "array", "itemSchema": { "type": "string" } } } }""";
		
		var properties = JsonSerializer.Deserialize<PropertiesModel>(json, Options)!;
		
		var array = Assert.IsType<ArrayFieldInfo>(Assert.Single(properties.Properties));
		Assert.Equal("$schema", array.ItemSchema?.Name);
		Assert.Same(array, array.ItemSchema?.Parent);
	}
	
	[Fact]
	public void Deserialize_NestedObject_SetsTheParents()
	{
		const string json = """{ "properties": { "address": { "type": "object", "properties": { "city": { "type": "string" } } } } }""";
		
		var properties = JsonSerializer.Deserialize<PropertiesModel>(json, Options)!;
		
		var address = Assert.IsType<ObjectFieldInfo>(Assert.Single(properties.Properties));
		var city = Assert.Single(address.Properties);
		Assert.Equal("address.city", city.Path);
	}
	
	#endregion
	
	#region Error Methods
	
	[Theory]
	[InlineData("""{ "type": "unknown" }""", "Unknown field type : 'unknown'")]
	[InlineData("""{ "maxLength": 5 }""", "Field info type missing")]
	public void Deserialize_WithInvalidType_ThrowsSchemaValidationException(string fieldJson, string expectedMessage)
	{
		var json = $$"""{ "properties": { "title": {{fieldJson}} } }""";
		
		var exception = Assert.Throws<SchemaValidationException>(() => JsonSerializer.Deserialize<PropertiesModel>(json, Options));
		
		Assert.Equal(expectedMessage, exception.Message);
	}
	
	[Fact]
	public void Deserialize_WithInvalidFieldRules_ThrowsTheFieldValidationException()
	{
		const string json = """{ "properties": { "title": { "type": "string", "minLength": 5, "maxLength": 2 } } }""";
		
		var exception = Assert.Throws<FieldValidationException>(() => JsonSerializer.Deserialize<PropertiesModel>(json, Options));
		
		Assert.Equal("MinLength can not be greater than MaxLength", exception.Message);
	}
	
	[Fact]
	public void Deserialize_PropertiesAsArray_ThrowsJsonException()
	{
		const string json = """{ "properties": [{ "name": "title", "type": "string" }] }""";
		
		var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PropertiesModel>(json, Options));
		
		Assert.Equal("Expected StartObject but got StartArray.", exception.Message);
	}
	
	[Fact]
	public void Serialize_WithDuplicateNames_ThrowsJsonException()
	{
		var properties = new PropertiesModel { Properties = [new StringFieldInfo { Name = "title" }, new IntegerFieldInfo { Name = "title" }] };
		
		var exception = Assert.Throws<JsonException>(() => JsonSerializer.Serialize(properties, Options));
		
		Assert.Equal("Duplicate field name: 'title'.", exception.Message);
	}
	
	#endregion
	
	#region Legacy Equivalence Methods
	
	public static TheoryData<string> NonArrayFieldNames()
	{
		return new TheoryData<string>(SampleFields.All().Where(x => x.Type != FieldType.array).Select(x => x.Name));
	}
	
	public static TheoryData<string> ArrayFieldNames()
	{
		return new TheoryData<string>(SampleFields.All().Where(x => x.Type == FieldType.array).Select(x => x.Name));
	}
	
	[Theory]
	[MemberData(nameof(NonArrayFieldNames))]
	public void LegacyDeserialize_OfSystemTextJsonOutput_ProducesTheSameField(string fieldName)
	{
		AssertLegacyReadsTheSameField(fieldName);
	}
	
	[Theory(Skip = "Bug (finding #13): the legacy (Newtonsoft) converter can not read any array field, the ItemSchema setter throws NullReferenceException")]
	[MemberData(nameof(ArrayFieldNames))]
	public void LegacyDeserialize_OfSystemTextJsonOutput_ProducesTheSameArrayField(string fieldName)
	{
		AssertLegacyReadsTheSameField(fieldName);
	}
	
	[Theory]
	[MemberData(nameof(FieldNames))]
	public void Deserialize_OfLegacyOutput_ProducesTheSameField(string fieldName)
	{
		var properties = SampleFields.All();
		var json = JsonSerializer.Serialize(new PropertiesModel { Properties = properties }, Options);
		var legacyPropertiesJson = LegacyFieldInfoCollectionJsonConverter.Serialize(properties);
		
		var deserialized = JsonSerializer.Deserialize<PropertiesModel>($$"""{ "properties": {{legacyPropertiesJson}} }""", Options)!;
		var roundTrippedJson = JsonSerializer.Serialize(deserialized, Options);
		
		Assert.Equal(FieldJson(json, fieldName), FieldJson(roundTrippedJson, fieldName));
	}
	
	#endregion
	
	#region Helper Methods
	
	private static void AssertLegacyReadsTheSameField(string fieldName)
	{
		var properties = SampleFields.All().Where(x => x.Name == fieldName).ToArray();
		var json = JsonSerializer.Serialize(new PropertiesModel { Properties = properties }, Options);
		var propertiesJson = JsonNode.Parse(json)!["properties"]!.ToJsonString();
		
		var legacyProperties = LegacyFieldInfoCollectionJsonConverter.Deserialize(propertiesJson)!.ToArray();
		var legacyJson = JsonSerializer.Serialize(new PropertiesModel { Properties = legacyProperties }, Options);
		
		Assert.Equal(FieldJson(json, fieldName), FieldJson(legacyJson, fieldName));
	}
	
	private static string FieldJson(string json, string fieldName)
	{
		return JsonNode.Parse(json)!["properties"]![fieldName]!.ToJsonString();
	}
	
	#endregion
	
	#region Test Types
	
	public sealed class PropertiesModel
	{
		[JsonPropertyName("properties")]
		[JsonConverter(typeof(FieldInfoCollectionJsonConverterFactory))]
		public required IReadOnlyCollection<IFieldInfo> Properties { get; init; }
	}
	
	#endregion
}
