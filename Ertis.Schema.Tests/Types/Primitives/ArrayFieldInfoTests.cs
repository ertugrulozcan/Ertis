using Ertis.Schema.Exceptions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types.Primitives;

public class ArrayFieldInfoTests
{
	#region Schema Methods
	
	[Theory]
	[InlineData("tags")]
	public void ValidateSchema_WithoutItemSchema_Fails(string name)
	{
		var fieldInfo = new ArrayFieldInfo { Name = name };
		
		var isValid = fieldInfo.ValidateSchema(out var exception);
		
		Assert.False(isValid);
		Assert.Equal("Item schema is required for array ('tags')", exception?.Message);
	}
	
	[Fact]
	public void Create_WithNegativeMinCount_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem(), MinCount = -1 });
		
		Assert.Equal("The 'minCount' value can not be less than zero ('tags')", exception.Message);
	}
	
	[Fact]
	public void Create_WithUniqueByOnPrimitiveItems_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem(), UniqueBy = ["code"] });
		
		Assert.Equal("The 'uniqueBy' feature only can be used with object-type items. Use the 'uniqueItems' feature for the primitive types. ('tags')", exception.Message);
	}
	
	#endregion
	
	#region Data Validation Methods
	
	[Fact]
	public void Validate_WithValidItems_Succeeds()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem() };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "tags": ["a", "b"] }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact]
	public void Validate_WithEmptyArray_Succeeds()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem() };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "tags": [] }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact]
	public void Validate_WithNonArrayValue_FailsWithTypeMismatch()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem() };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "tags": "a" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. 'tags' is must be 'array'"], result.Messages);
	}
	
	[Fact]
	public void Validate_WithAnInvalidItem_Fails()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem() };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "tags": ["a", 5] }""");
		
		Assert.False(result.IsValid);
		Assert.Single(result.Errors);
		Assert.StartsWith("Type mismatch error.", result.Errors[0].Message);
	}
	
	[Theory]
	[InlineData("[\"a\"]", "Array length can not be less than 2")]
	[InlineData("[\"a\", \"b\", \"c\", \"d\"]", "Array length can not be greater than 3")]
	public void Validate_WithCountOutOfTheBounds_Fails(string jsonValue, string expectedMessage)
	{
		var fieldInfo = new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem(), MinCount = 2, MaxCount = 3 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "tags": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal([expectedMessage], result.Messages);
	}
	
	[Fact]
	public void Validate_WithDuplicateItemsWhenUniqueItems_Fails()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "tags", ItemSchema = StringItem(), UniqueItems = true };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "tags": ["a", "a"] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Array items must be unique"], result.Messages);
	}
	
	[Fact]
	public void Validate_WithObjectItems_ValidatesEachItem()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "phones", ItemSchema = PhoneItem() };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "555" }, { "number": 5 }, { "label": "home" }] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(2, result.Errors.Count);
		Assert.Contains("number is required", result.Messages);
	}
	
	[Fact]
	public void Validate_WithDuplicateItemsByUniqueBy_Fails()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "phones", ItemSchema = PhoneItem(), UniqueBy = ["number"] };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "555" }, { "number": "555" }] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Array items must be unique by the 'number' field"], result.Messages);
	}
	
	[Fact]
	public void Validate_WithDistinctItemsByUniqueBy_Succeeds()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "phones", ItemSchema = PhoneItem(), UniqueBy = ["number"] };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "555" }, { "number": "556" }] }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact]
	public void Validate_WithDuplicateItemsByANestedUniqueByPath_Fails()
	{
		var fieldInfo = new ArrayFieldInfo
		{
			Name = "people",
			UniqueBy = ["contact.email"],
			ItemSchema = new ObjectFieldInfo([new ObjectFieldInfo([new StringFieldInfo { Name = "email" }]) { Name = "contact" }]) { Name = "$schema" }
		};
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "people": [{ "contact": { "email": "a@x.com" } }, { "contact": { "email": "a@x.com" } }] }""");
		
		Assert.Equal(["Array items must be unique by the 'contact.email' field"], result.Messages);
	}
	
	[Fact]
	public void Validate_UniqueBy_IgnoresTheItemsWithoutTheField()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "phones", ItemSchema = PhoneItem(), UniqueBy = ["label"] };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "1" }, { "number": "2" }, { "number": "3", "label": null }] }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact]
	public void Validate_UniqueBy_IsCaseSensitive()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "phones", ItemSchema = PhoneItem(), UniqueBy = ["label"] };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "1", "label": "Home" }, { "number": "2", "label": "home" }] }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact]
	public void Validate_WithSeveralUniqueByPaths_ReportsEachViolatedPath()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "phones", ItemSchema = PhoneItem(), UniqueBy = ["number", "label"] };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "1", "label": "home" }, { "number": "2", "label": "home" }] }""");
		
		Assert.Equal(["Array items must be unique by the 'label' field"], result.Messages);
	}
	
	[Fact]
	public void Validate_UniqueByWithAPrimitiveItem_FailsWithTypeMismatch()
	{
		var fieldInfo = new ArrayFieldInfo { Name = "phones", ItemSchema = PhoneItem(), UniqueBy = ["number"] };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "phones": [{ "number": "1" }, "2"] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. '$schema' is must be 'object'"], result.Messages);
	}
	
	private static StringFieldInfo StringItem()
	{
		return new StringFieldInfo { Name = "$schema" };
	}
	
	private static IFieldInfo PhoneItem()
	{
		return new ObjectFieldInfo([
			new StringFieldInfo { Name = "number", IsRequired = true },
			new StringFieldInfo { Name = "label" }
		])
		{
			Name = "$schema"
		};
	}
	
	#endregion
}
