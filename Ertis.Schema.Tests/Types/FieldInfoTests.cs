using Ertis.Schema.Exceptions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types;

public class FieldInfoTests
{
	#region Schema Methods
	
	[Theory]
	[InlineData("title")]
	[InlineData("first_name")]
	[InlineData("_id")]
	[InlineData("address2")]
	[InlineData("şehir")]
	[InlineData("first-name")]
	[InlineData("a-b-c")]
	public void ValidateSchema_WithValidName_Succeeds(string name)
	{
		var fieldInfo = new StringFieldInfo { Name = name };
		
		var isValid = fieldInfo.ValidateSchema(out var exception);
		
		Assert.True(isValid);
		Assert.Null(exception);
	}
	
	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("first name")]
	[InlineData("first\tname")]
	[InlineData("2nd")]
	[InlineData("first.name")]
	[InlineData("first$name")]
	[InlineData("-name")]
	[InlineData("name-")]
	[InlineData("-")]
	public void ValidateSchema_WithInvalidName_Fails(string name)
	{
		var fieldInfo = new StringFieldInfo { Name = name };
		
		var isValid = fieldInfo.ValidateSchema(out var exception);
		
		Assert.False(isValid);
		Assert.IsType<FieldValidationException>(exception);
	}
	
	[Fact]
	public void ValidateSchema_WithSeveralErrors_ReturnsTheFirstOne()
	{
		var fieldInfo = new StringFieldInfo { Name = "first name", MaxLength = 5 };
		
		fieldInfo.ValidateSchema(out var exception);
		
		Assert.Equal("The field name can not include any whitespace", exception?.Message);
	}
	
	[Fact]
	public void Create_ObjectWithAnInvalidPropertyName_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new ObjectFieldInfo([new StringFieldInfo { Name = "first name" }]) { Name = "person" });
		
		Assert.Equal("The field name can not include any whitespace", exception.Message);
	}
	
	[Fact]
	public void Create_ArrayItemSchemaWithTheLibraryName_Succeeds()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "tags", ItemSchema = new StringFieldInfo { Name = "$schema" } };
		
		Assert.True(fieldInfo.ValidateSchema(out var exception));
		Assert.Null(exception);
	}
	
	[Fact]
	public void Create_HiddenAndRequiredWithoutDefault_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new IntegerFieldInfo { Name = "age", IsHidden = true, IsRequired = true });
		
		Assert.Equal("A field with a default value of null cannot be both hidden and required.", exception.Message);
	}
	
	[Fact]
	public void Create_HiddenAndRequiredWithDefault_Succeeds()
	{
		var fieldInfo = new IntegerFieldInfo { Name = "age", DefaultValue = 18, IsHidden = true, IsRequired = true };
		
		Assert.True(fieldInfo.IsHidden);
		Assert.True(fieldInfo.IsRequired);
	}
	
	[Fact]
	public void Path_OfNestedField_IncludesParentNames()
	{
		var city = new StringFieldInfo { Name = "city" };
		var address = new ObjectFieldInfo([city]) { Name = "address" };
		
		Assert.Equal("address.city", city.Path);
		Assert.Same(address, city.Parent);
	}
	
	#endregion
	
	#region Data Validation Methods
	
	[Fact]
	public void Validate_RequiredFieldMissing_Fails()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title", IsRequired = true });
		
		var result = SchemaValidation.Validate(schema, "{}");
		
		Assert.False(result.IsValid);
		Assert.Equal(["title is required"], result.Messages);
	}
	
	[Fact]
	public void Validate_RequiredFieldNull_Fails()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title", IsRequired = true });
		
		var result = SchemaValidation.Validate(schema, """{ "title": null }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["title is required"], result.Messages);
	}
	
	[Fact]
	public void Validate_RequiredFieldMissingWithDefault_SucceedsAndSetsTheDefault()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title", IsRequired = true, DefaultValue = "untitled" });
		
		var result = SchemaValidation.Validate(schema, "{}");
		
		Assert.True(result.IsValid);
		Assert.Equal("untitled", result.Content.GetValue("title"));
	}
	
	[Fact]
	public void Validate_OptionalFieldMissing_Succeeds()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title" });
		
		var result = SchemaValidation.Validate(schema, "{}");
		
		Assert.True(result.IsValid);
		Assert.Empty(result.Errors);
	}
	
	[Fact]
	public void Validate_AdditionalPropertyWhenNotAllowed_Fails()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title" });
		
		var result = SchemaValidation.Validate(schema, """{ "title": "a", "extra": 1 }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Additional properties not allowed in this object schema. (extra)"], result.Messages);
	}
	
	[Fact]
	public void Validate_AdditionalPropertyWhenAllowed_Succeeds()
	{
		var schema = new TestSchema { Properties = [new StringFieldInfo { Name = "title" }], AllowAdditionalProperties = true };
		
		var result = SchemaValidation.Validate(schema, """{ "title": "a", "extra": 1 }""");
		
		Assert.True(result.IsValid);
		Assert.Equal(1L, result.Content.GetValue("extra"));
	}
	
	[Fact]
	public void Validate_SeveralInvalidFields_ReportsAllErrors()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "title", IsRequired = true },
			new BooleanFieldInfo { Name = "active" },
			new StringFieldInfo { Name = "code", MaxLength = 2 });
		
		var result = SchemaValidation.Validate(schema, """{ "active": "yes", "code": "abc" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(3, result.Errors.Count);
		Assert.Equal(["active", "code", "title"], result.Errors.Select(x => x.FieldName).Order());
	}
	
	#endregion
}
