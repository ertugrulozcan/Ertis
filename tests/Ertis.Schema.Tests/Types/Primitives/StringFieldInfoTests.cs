using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Tests.Types.Primitives;

public class StringFieldInfoTests
{
	#region Schema Methods
	
	[Fact]
	public void Create_WithNegativeMinLength_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new StringFieldInfo { Name = "title", MinLength = -1 });
		
		Assert.Equal("MinLength can not be less than zero", exception.Message);
	}
	
	[Fact]
	public void Create_WithMinLengthGreaterThanMaxLength_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new StringFieldInfo { Name = "title", MaxLength = 2, MinLength = 3 });
		
		Assert.Equal("MinLength can not be greater than MaxLength", exception.Message);
	}
	
	[Fact]
	public void Create_WithWhitespaceInFormatPatternSegment_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new StringFieldInfo { Name = "fullname", FormatPattern = "{first name}" });
		
		Assert.Equal("The 'formatPattern' segments can not be contains whitespace", exception.Message);
	}
	
	[Fact]
	public void Create_HiddenAndRequiredWithFormatPattern_Succeeds()
	{
		var fieldInfo = new StringFieldInfo { Name = "fullname", FormatPattern = "{firstname}", IsHidden = true, IsRequired = true };
		
		Assert.True(fieldInfo.IsHidden);
	}
	
	#endregion
	
	#region Data Validation Methods
	
	[Theory]
	[InlineData("abc", true)]
	[InlineData("abcde", true)]
	[InlineData("abcdef", false)]
	public void Validate_WithMaxLength_ValidatesTheLength(string value, bool expected)
	{
		var schema = new ObjectFieldInfo([new StringFieldInfo { Name = "title", MaxLength = 5 }])
		{
			Name = "root"
		};
		
		var content = DynamicObject.Create(new Dictionary<string, object?> { ["title"] = value });
		var validationContext = new FieldValidationContext(content);
		
		var isValid = schema.ValidateData(content, validationContext);
		
		Assert.Equal(expected, isValid);
		Assert.Equal(expected, validationContext.Errors.Count == 0);
	}
	
	[Theory]
	[InlineData("ab", false)]
	[InlineData("abc", true)]
	public void Validate_WithMinLength_ValidatesTheLength(string value, bool expected)
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "title", MinLength = 3 }, $$"""{ "title": "{{value}}" }""");
		
		Assert.Equal(expected, result.IsValid);
	}
	
	[Fact]
	public void Validate_RequiredWithWhitespaceOnly_Fails()
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "title", IsRequired = true }, """{ "title": "   " }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["title is required"], result.Messages);
	}
	
	[Theory]
	[InlineData("5")]
	[InlineData("true")]
	[InlineData("{}")]
	[InlineData("[\"a\"]")]
	public void Validate_WithNonStringValue_FailsWithTypeMismatch(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "title" }, $$"""{ "title": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Contains("Type mismatch error. 'title' is must be 'string'", result.Messages);
	}
	
	[Theory]
	[InlineData("ABC-123", true)]
	[InlineData("abc-123", false)]
	public void Validate_WithRegexPattern_ValidatesTheValue(string value, bool expected)
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "code", RegexPattern = "^[A-Z]{3}-[0-9]{3}$" }, $$"""{ "code": "{{value}}" }""");
		
		Assert.Equal(expected, result.IsValid);
	}
	
	[Fact]
	public void Validate_ManyFieldsWithDifferentRegexPatterns_ValidatesEachWithItsOwnPattern()
	{
		var fields = Enumerable.Range(0, 30).Select(i => new StringFieldInfo { Name = $"code{i}", RegexPattern = $"^{i}-[a-z]+$" }).ToArray<Ertis.Schema.Types.IFieldInfo>();
		var schema = TestSchema.Of(fields);
		var validJson = "{" + string.Join(",", Enumerable.Range(0, 30).Select(i => $"\"code{i}\": \"{i}-abc\"")) + "}";
		var invalidJson = "{" + string.Join(",", Enumerable.Range(0, 30).Select(i => $"\"code{i}\": \"{(i + 1) % 30}-abc\"")) + "}";
		
		Assert.True(SchemaValidation.Validate(schema, validJson).IsValid);
		Assert.Equal(30, SchemaValidation.Validate(schema, invalidJson).Errors.Count);
	}
	
	[Theory]
	[InlineData("[", null, "regexPattern")]
	[InlineData(null, "(", "restrictRegexPattern")]
	public void Create_WithInvalidRegexPattern_Throws(string? regexPattern, string? restrictRegexPattern, string propertyName)
	{
		var exception = Assert.Throws<FieldValidationException>(() => new StringFieldInfo { Name = "code", RegexPattern = regexPattern, RestrictRegexPattern = restrictRegexPattern });
		
		Assert.StartsWith($"The '{propertyName}' is not a valid regular expression.", exception.Message);
	}
	
	[Fact]
	public void Deserialize_WithInvalidRegexPattern_ThrowsTheFieldValidationException()
	{
		var options = new System.Text.Json.JsonSerializerOptions { Converters = { new Ertis.Schema.Serialization.FieldInfoJsonConverter() } };
		
		Assert.Throws<FieldValidationException>(() => System.Text.Json.JsonSerializer.Deserialize<Ertis.Schema.Types.IFieldInfo>("""{ "name": "code", "type": "string", "regexPattern": "[" }""", options));
	}
	
	[Fact]
	public void Validate_WithRegexPatternAndEmptyValue_SkipsThePattern()
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "code", RegexPattern = "^[A-Z]+$" }, """{ "code": "" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("hello", true)]
	[InlineData("<script>", false)]
	public void Validate_WithRestrictRegexPattern_RejectsMatchingValues(string value, bool expected)
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "text", RestrictRegexPattern = "[<>]" }, $$"""{ "text": "{{value}}" }""");
		
		Assert.Equal(expected, result.IsValid);
	}
	
	[Fact]
	public void Validate_WithFormatPattern_FillsTheValueFromOtherFields()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "firstname" },
			new StringFieldInfo { Name = "lastname" },
			new StringFieldInfo { Name = "fullname", FormatPattern = "{firstname} {lastname}" });
		
		var result = SchemaValidation.Validate(schema, """{ "firstname": "Jane", "lastname": "Doe" }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("Jane Doe", result.Content.GetValue("fullname"));
	}
	
	[Fact]
	public void Validate_WithFormatPattern_OverridesTheSentValue()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "firstname" },
			new StringFieldInfo { Name = "fullname", FormatPattern = "{firstname}!" });
		
		var result = SchemaValidation.Validate(schema, """{ "firstname": "Jane", "fullname": "Other" }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("Jane!", result.Content.GetValue("fullname"));
	}
	
	[Fact]
	public void Validate_WithDateOnlyString_Succeeds()
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "code" }, """{ "code": "2026-01-01" }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("2026-01-01", result.Content.GetValue("code"));
	}
	
	[Theory]
	[InlineData("2026-01-01T10:00:00Z")]
	[InlineData("2026-01-01T10:00:00+03:00")]
	[InlineData("2026-01-01T10:00:00")]
	public void Validate_WithDateTimeLikeString_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new StringFieldInfo { Name = "code" }, $$"""{ "code": "{{value}}" }""");
		
		Assert.True(result.IsValid);
		Assert.Equal(value, result.Content.GetValue("code"));
	}
	
	#endregion
}
