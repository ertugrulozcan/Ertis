using Ertis.Schema.Exceptions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types.Primitives;

public class ObjectFieldInfoTests
{
	#region Schema Methods
	
	[Fact]
	public void Create_WithDuplicatePropertyNames_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new ObjectFieldInfo([
			new StringFieldInfo { Name = "city" },
			new StringFieldInfo { Name = "city" }
		])
		{
			Name = "address"
		});
		
		Assert.Equal("Duplicate property declaration in a field info. Property names are must be unique.", exception.Message);
	}
	
	#endregion
	
	#region Data Validation Methods
	
	[Fact]
	public void Validate_WithValidNestedObject_Succeeds()
	{
		var result = SchemaValidation.ValidateField(CreateAddress(), """{ "address": { "city": "Istanbul", "zip": 34000 } }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("Istanbul", result.Content.GetValue("address.city"));
	}
	
	[Theory]
	[InlineData("\"Istanbul\"")]
	[InlineData("5")]
	[InlineData("[1]")]
	public void Validate_WithNonObjectValue_FailsWithTypeMismatch(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(CreateAddress(), $$"""{ "address": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. 'address' is must be 'object'"], result.Messages);
	}
	
	[Fact]
	public void Validate_WithNestedRequiredFieldMissing_Fails()
	{
		var result = SchemaValidation.ValidateField(CreateAddress(), """{ "address": { "zip": 34000 } }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["city is required"], result.Messages);
		Assert.Equal("test-schema.address.city", result.Errors[0].FieldPath);
	}
	
	[Fact]
	public void Validate_WithNestedAdditionalProperty_Fails()
	{
		var result = SchemaValidation.ValidateField(CreateAddress(), """{ "address": { "city": "Istanbul", "street": "x" } }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Additional properties not allowed in this object schema. (street)"], result.Messages);
	}
	
	[Fact]
	public void Validate_WithNestedTypeMismatch_Fails()
	{
		var result = SchemaValidation.ValidateField(CreateAddress(), """{ "address": { "city": "Istanbul", "zip": "34000" } }""");
		
		Assert.False(result.IsValid);
		Assert.Single(result.Errors);
		Assert.Equal("zip", result.Errors[0].FieldName);
	}
	
	[Fact]
	public void Validate_WithDeeplyNestedObject_ValidatesEveryLevel()
	{
		var fieldInfo = new ObjectFieldInfo([
			new ObjectFieldInfo([
				new ObjectFieldInfo([new IntegerFieldInfo { Name = "value", IsRequired = true }]) { Name = "level3" }
			])
			{
				Name = "level2"
			}
		])
		{
			Name = "level1"
		};
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "level1": { "level2": { "level3": {} } } }""");
		
		Assert.False(result.IsValid);
		Assert.Equal("test-schema.level1.level2.level3.value", Assert.Single(result.Errors).FieldPath);
	}
	
	[Fact]
	public void Validate_WithNestedDefaultValue_SetsTheDefault()
	{
		var fieldInfo = new ObjectFieldInfo([
			new StringFieldInfo { Name = "city", IsRequired = true },
			new StringFieldInfo { Name = "country", DefaultValue = "TR" }
		])
		{
			Name = "address"
		};
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "address": { "city": "Istanbul" } }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("TR", result.Content.GetValue("address.country"));
	}
	
	[Fact]
	public void Validate_WithNestedFormatPattern_FillsTheValue()
	{
		var fieldInfo = new ObjectFieldInfo([
			new StringFieldInfo { Name = "city" },
			new StringFieldInfo { Name = "label", FormatPattern = "{address.city}" }
		])
		{
			Name = "address"
		};
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "address": { "city": "Istanbul" } }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("Istanbul", result.Content.GetValue("address.label"));
	}
	
	[Theory]
	[InlineData("{}")]
	[InlineData("""{ "address": null }""")]
	public void Validate_WithoutTheParentObject_DoesNotApplyTheNestedDefault(string json)
	{
		var fieldInfo = new ObjectFieldInfo([new StringFieldInfo { Name = "country", DefaultValue = "TR" }]) { Name = "address" };
		
		var result = SchemaValidation.ValidateField(fieldInfo, json);
		
		Assert.True(result.IsValid);
		Assert.False(result.Content.ContainsProperty("address.country"));
	}
	
	[Fact]
	public void Validate_WithDeeplyNestedDefault_SetsTheDefault()
	{
		var fieldInfo = new ObjectFieldInfo([
			new ObjectFieldInfo([new StringFieldInfo { Name = "code", DefaultValue = "34" }]) { Name = "region" }
		])
		{
			Name = "address"
		};
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "address": { "region": {} } }""");
		
		Assert.Equal("34", result.Content.GetValue("address.region.code"));
	}
	
	[Fact]
	public void Validate_WithNestedDate_ConvertsTheValueToDateTime()
	{
		var fieldInfo = new ObjectFieldInfo([new Ertis.Schema.Types.CustomTypes.DateFieldInfo { Name = "since" }]) { Name = "membership" };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "membership": { "since": "2026-01-31" } }""");
		
		Assert.True(result.IsValid);
		Assert.IsType<DateTime>(result.Content.GetValue("membership.since"));
	}
	
	/// <summary>
	/// The post validation steps are applied to the objects only, the array items are out of scope for now
	/// </summary>
	[Fact]
	public void Validate_ArrayItemDefault_IsNotApplied()
	{
		var fieldInfo = new ArrayFieldInfo
		{
			Name = "phones",
			ItemSchema = new ObjectFieldInfo([new StringFieldInfo { Name = "number" }, new StringFieldInfo { Name = "label", DefaultValue = "home" }]) { Name = "$schema" }
		};
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "1" }] }""");
		
		Assert.True(result.IsValid);
		Assert.False(result.Content.ContainsProperty("phones[0].label"));
	}
	
	private static ObjectFieldInfo CreateAddress()
	{
		return new ObjectFieldInfo([
			new StringFieldInfo { Name = "city", IsRequired = true },
			new IntegerFieldInfo { Name = "zip" }
		])
		{
			Name = "address"
		};
	}
	
	#endregion
}
