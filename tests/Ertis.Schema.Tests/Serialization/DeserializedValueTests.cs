using System.Text.Json;
using Ertis.Schema.Serialization;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types;
using Ertis.Schema.Types.CustomTypes;

namespace Ertis.Schema.Tests.Serialization;

/// <summary>
/// The object-typed values of a deserialized schema (default values, constants, query parameters) are converted into the value model
/// (dictionaries, arrays, primitives) instead of staying JsonElement
/// </summary>
public class DeserializedValueTests
{
	#region Fields
	
	private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
	{
		Converters = { new FieldInfoJsonConverter() }
	};
	
	#endregion
	
	#region Methods
	
	public static TheoryData<string, string, Type, string> DeserializedFields()
	{
		return new TheoryData<string, string, Type, string>
		{
			{ """{ "name": "country", "type": "enum", "defaultValue": "tr", "items": [{ "displayName": "TR", "value": "tr" }] }""", "country", typeof(string), "\"tr\"" },
			{ """{ "name": "kind", "type": "const", "valueType": "string", "value": "member" }""", "kind", typeof(string), "\"member\"" },
			{ """{ "name": "data", "type": "json", "defaultValue": { "a": 1, "b": [1, "x"] } }""", "data", typeof(Dictionary<string, object?>), """{"a":1,"b":[1,"x"]}""" },
			{ """{ "name": "data", "type": "json", "defaultValue": [1, { "a": true }] }""", "data", typeof(object[]), """[1,{"a":true}]""" },
			{ """{ "name": "tags", "type": "tags", "defaultValue": ["a"] }""", "tags", typeof(string[]), """["a"]""" },
			{ """{ "name": "age", "type": "integer", "defaultValue": 18 }""", "age", typeof(long), "18" }
		};
	}
	
	[Theory]
	[MemberData(nameof(DeserializedFields))]
	public void Validate_WithDeserializedField_SetsAValueOfTheValueModel(string fieldJson, string fieldName, Type expectedType, string expectedJson)
	{
		var fieldInfo = JsonSerializer.Deserialize<IFieldInfo>(fieldJson, Options)!;
		
		var isSchemaValid = fieldInfo.ValidateSchema(out var schemaException);
		var result = SchemaValidation.ValidateField(fieldInfo, "{}");
		
		Assert.True(isSchemaValid, schemaException?.Message);
		Assert.True(result.IsValid);
		Assert.IsType(expectedType, result.Content.GetValue(fieldName));
		Assert.Equal($$"""{"{{fieldName}}":{{expectedJson}}}""", result.Content.ToJson());
	}
	
	[Theory]
	[InlineData("""{ "name": "data", "type": "json", "defaultValue": { "a": 1, "b": [1, "x"] } }""", "defaultValue")]
	[InlineData("""{ "name": "country", "type": "enum", "defaultValue": "tr", "items": [{ "displayName": "TR", "value": "tr" }] }""", "defaultValue")]
	[InlineData("""{ "name": "kind", "type": "const", "valueType": "string", "value": "member" }""", "value")]
	public void Serialize_OfDeserializedField_WritesTheSameValue(string fieldJson, string valueProperty)
	{
		var fieldInfo = JsonSerializer.Deserialize<IFieldInfo>(fieldJson, Options)!;
		
		var json = JsonSerializer.Serialize(fieldInfo, Options);
		
		var expected = JsonDocument.Parse(fieldJson).RootElement.GetProperty(valueProperty);
		var actual = JsonDocument.Parse(json).RootElement.GetProperty(valueProperty);
		Assert.True(JsonElement.DeepEquals(expected, actual), json);
	}
	
	[Fact]
	public void Deserialize_QueryParameterValues_AreConverted()
	{
		var parameter = JsonSerializer.Deserialize<CollectionReferenceParameter>("""{ "name": "status", "value": { "in": ["a", "b"] }, "defaultValue": 5 }""", Options)!;
		
		var value = Assert.IsType<Dictionary<string, object?>>(parameter.Value);
		Assert.Equal(new object[] { "a", "b" }, value["in"]);
		Assert.Equal(5L, parameter.DefaultValue);
	}
	
	[Fact]
	public void Create_WithClrDefaultValue_KeepsTheValue()
	{
		var defaultValue = new Dictionary<string, object?> { ["a"] = 1 };
		
		var fieldInfo = new JsonFieldInfo { Name = "data", DefaultValue = defaultValue };
		
		Assert.Same(defaultValue, fieldInfo.DefaultValue);
	}
	
	#endregion
}
