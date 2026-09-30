using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.CustomTypes;

namespace Ertis.Schema.Tests.Types.CustomTypes;

public class TagsFieldInfoTests
{
	#region Data Validation Methods
	
	[Theory]
	[InlineData("[\"a\", \"b\"]")]
	[InlineData("[]")]
	public void Validate_WithValidTags_Succeeds(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new TagsFieldInfo { Name = "tags" }, $$"""{ "tags": {{jsonValue}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("[\"a\", \"a\"]", "Tags items must be unique")]
	[InlineData("[\"a\", \"\"]", "Tags items can not be blank")]
	public void Validate_WithInvalidItems_Fails(string jsonValue, string expectedMessage)
	{
		var result = SchemaValidation.ValidateField(new TagsFieldInfo { Name = "tags" }, $$"""{ "tags": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal([expectedMessage], result.Messages);
	}
	
	[Theory]
	[InlineData("[\"a\"]", "Tags item count can not be less than 2")]
	[InlineData("[\"a\", \"b\", \"c\", \"d\"]", "Tags item count can not be greater than 3")]
	[InlineData("[\"a\", \"toolong\"]", "The length of tag items can not be greater than 5 character")]
	public void Validate_WithRulesViolated_Fails(string jsonValue, string expectedMessage)
	{
		var fieldInfo = new TagsFieldInfo { Name = "tags", MinCount = 2, MaxCount = 3, MaxLength = 5 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "tags": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal([expectedMessage], result.Messages);
	}
	
	[Theory]
	[InlineData("\"a\"")]
	[InlineData("[1, 2]")]
	public void Validate_WithNonStringArray_FailsWithTypeMismatch(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new TagsFieldInfo { Name = "tags" }, $$"""{ "tags": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Contains("Type mismatch error. 'tags' is must be 'string array'", result.Messages);
	}
	
	[Theory]
	[InlineData(-1, null, "The 'minLength' value can not be less than zero ('tags')")]
	[InlineData(5, 2, "The 'minLength' value can not be greater than the 'maxLength' value ('tags')")]
	public void Create_WithInvalidLengthRule_Throws(int minLength, int? maxLength, string expectedMessage)
	{
		var exception = Assert.Throws<Ertis.Schema.Exceptions.FieldValidationException>(() => new TagsFieldInfo { Name = "tags", MaxLength = maxLength, MinLength = minLength });
		
		Assert.Equal(expectedMessage, exception.Message);
	}
	
	#endregion
}
