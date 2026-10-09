using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;

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
	
	[Fact]
	public void Serialize_WithDefaultOptions_WritesTheNameInLowerCase()
	{
		var options = new JsonSerializerOptions { Converters = { new FieldInfoJsonConverter() } };
		var properties = new PropertiesModel { Properties = [new ArrayFieldInfo { Name = "tags", ItemSchema = new StringFieldInfo { Name = "$schema" } }] };
		
		var json = JsonSerializer.Serialize(properties, options);
		var roundTripped = JsonSerializer.Deserialize<PropertiesModel>(json, options)!;
		
		Assert.Equal("""{"properties":{"tags":{"type":"array","itemSchema":{"type":"string","name":"$schema"}}}}""", json);
		Assert.Equal("tags", Assert.Single(roundTripped.Properties).Name);
	}
	
	[Fact]
	public void Deserialize_ArrayFieldWithoutItemSchemaName_UsesTheSchemaName()
	{
		var fieldInfo = JsonSerializer.Deserialize<IFieldInfo>("""{ "name": "tags", "type": "array", "itemSchema": { "type": "string" } }""", Options);
		
		var array = Assert.IsType<ArrayFieldInfo>(fieldInfo);
		Assert.Equal("$schema", array.ItemSchema?.Name);
	}
	
	[Fact]
	public void Deserialize_ArrayFieldWithItemSchemaName_KeepsTheName()
	{
		var fieldInfo = JsonSerializer.Deserialize<IFieldInfo>("""{ "name": "tags", "type": "array", "itemSchema": { "name": "custom", "type": "string" } }""", Options);
		
		Assert.Equal("custom", Assert.IsType<ArrayFieldInfo>(fieldInfo).ItemSchema?.Name);
	}
	
	[Fact]
	public void Deserialize_ArrayOfArrays_ReadsEveryLevel()
	{
		const string json = """{ "properties": { "matrix": { "type": "array", "itemSchema": { "type": "array", "itemSchema": { "type": "array", "itemSchema": { "type": "integer" } } } } } }""";
		
		var properties = JsonSerializer.Deserialize<PropertiesModel>(json, Options)!;
		var roundTripped = JsonSerializer.Deserialize<PropertiesModel>(JsonSerializer.Serialize(properties, Options), Options)!;
		
		var level1 = Assert.IsType<ArrayFieldInfo>(Assert.Single(roundTripped.Properties));
		var level2 = Assert.IsType<ArrayFieldInfo>(level1.ItemSchema);
		var level3 = Assert.IsType<ArrayFieldInfo>(level2.ItemSchema);
		Assert.IsType<IntegerFieldInfo>(level3.ItemSchema);
		Assert.Equal("$schema", level3.ItemSchema?.Name);
	}
	
	[Fact]
	public void Validate_WithArrayOfArrays_ReportsTheNestedItemPath()
	{
		const string json = """{ "properties": { "matrix": { "type": "array", "itemSchema": { "type": "array", "itemSchema": { "type": "integer" } } } } }""";
		var properties = JsonSerializer.Deserialize<PropertiesModel>(json, Options)!;
		
		var result = SchemaValidation.Validate(TestSchema.Of(properties.Properties.ToArray()), """{ "matrix": [[1, 2], ["x"]] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal("test-schema.matrix[1][0]", Assert.Single(result.Errors).FieldPath);
	}
	
	#endregion
	
	#region Error Methods
	
	[Theory]
	[InlineData("""{ "type": "unknown" }""", "Unknown field type : 'unknown'")]
	[InlineData("""{ "type": "1" }""", "Unknown field type : '1'")]
	[InlineData("""{ "type": "String" }""", "Unknown field type : 'String'")]
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
	
	#region Helper Methods
	
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
