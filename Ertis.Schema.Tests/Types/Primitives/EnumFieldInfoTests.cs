using Ertis.Schema.Exceptions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types.Primitives;

public class EnumFieldInfoTests
{
	#region Schema Methods
	
	[Fact]
	public void Create_WithEmptyItems_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new EnumFieldInfo { Name = "country", Items = [] });
		
		Assert.Equal("Enum items can not be empty", exception.Message);
	}
	
	[Fact]
	public void Create_WithDuplicateItems_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new EnumFieldInfo
		{
			Name = "country",
			Items = [Item("tr"), Item("tr")]
		});
		
		Assert.Equal("Enum items must be unique", exception.Message);
	}
	
	[Fact]
	public void ValidateSchema_WithoutItems_ReturnsFalse()
	{
		var fieldInfo = new EnumFieldInfo { Name = "country" };
		
		var isValid = fieldInfo.ValidateSchema(out var exception);
		
		Assert.False(isValid);
		Assert.Equal("Enum items can not be empty", exception?.Message);
	}
	
	#endregion
	
	#region Data Validation Methods
	
	[Theory]
	[InlineData("tr")]
	[InlineData("de")]
	public void Validate_WithAnItemValue_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(CreateField(), $$"""{ "country": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("\"xx\"")]
	[InlineData("\"TR\"")]
	[InlineData("5")]
	public void Validate_WithAnUnknownValue_Fails(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(CreateField(), $$"""{ "country": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["The value does not exist in the enum items. The 'country' value must be one of them ['tr', 'de']"], result.Messages);
	}
	
	[Fact]
	public void Validate_WithObjectValue_Fails()
	{
		var result = SchemaValidation.ValidateField(CreateField(), """{ "country": { "code": "tr" } }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Enum value is must be primitive type (country)"], result.Messages);
	}
	
	[Fact]
	public void Validate_MultipleWithItemValues_Succeeds()
	{
		var result = SchemaValidation.ValidateField(CreateField(isMultiple: true), """{ "country": ["tr", "de"] }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact]
	public void Validate_MultipleWithAnUnknownValue_Fails()
	{
		var result = SchemaValidation.ValidateField(CreateField(isMultiple: true), """{ "country": ["tr", "xx"] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["The value does not exist in the enum items. The 'country' value must be one of them ['tr', 'de']"], result.Messages);
	}
	
	[Fact]
	public void Validate_MultipleWithDuplicateValues_Fails()
	{
		var result = SchemaValidation.ValidateField(CreateField(isMultiple: true), """{ "country": ["tr", "de", "tr"] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["The 'country' values must be unique"], result.Messages);
	}
	
	[Fact]
	public void Validate_MultipleWithSingleValue_Fails()
	{
		var result = SchemaValidation.ValidateField(CreateField(isMultiple: true), """{ "country": "tr" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Enum value is must be array type (country)"], result.Messages);
	}
	
	private static EnumFieldInfo CreateField(bool isMultiple = false)
	{
		return new EnumFieldInfo
		{
			Name = "country",
			Items = [Item("tr"), Item("de")],
			IsMultiple = isMultiple
		};
	}
	
	private static EnumFieldInfo.EnumItem Item(string value)
	{
		return new EnumFieldInfo.EnumItem { DisplayName = value.ToUpperInvariant(), Value = value };
	}
	
	#endregion
}
