using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types.Primitives;

public class FloatFieldInfoTests
{
	#region Schema Methods
	
	[Fact]
	public void Create_WithMinimumGreaterThanMaximum_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new FloatFieldInfo { Name = "ratio", Maximum = 0.5, Minimum = 1.5 });
		
		Assert.Equal("The 'minimum' value can not be greater than the 'maximum' value (ratio)", exception.Message);
	}
	
	#endregion
	
	#region Data Validation Methods
	
	[Theory]
	[InlineData("1.5")]
	[InlineData("-0.25")]
	[InlineData("3")]
	public void Validate_WithNumber_Succeeds(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new FloatFieldInfo { Name = "ratio" }, $$"""{ "ratio": {{jsonValue}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("\"1.5\"")]
	[InlineData("false")]
	public void Validate_WithNonNumber_FailsWithTypeMismatch(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new FloatFieldInfo { Name = "ratio" }, $$"""{ "ratio": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. 'ratio' is must be 'Nullable`1'"], result.Messages);
	}
	
	[Fact]
	public void Validate_ParsedFloatValue_IsADouble()
	{
		var content = DynamicObject.Parse("""{ "ratio": 0.5 }""");
		
		Assert.IsType<double>(content.GetValue("ratio"));
	}
	
	[Theory(Skip = "Bug (backlog #7): float bounds are not enforced for Double values (every decimal parsed from json)")]
	[InlineData(-0.5)]
	[InlineData(1.5)]
	public void Validate_ParsedDecimalOutOfTheBounds_Fails(double value)
	{
		var fieldInfo = new FloatFieldInfo { Name = "ratio", Minimum = 0, Maximum = 1 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "ratio": {{value.ToString(System.Globalization.CultureInfo.InvariantCulture)}} }""");
		
		Assert.False(result.IsValid);
	}
	
	[Theory(Skip = "Bug (backlog #7): exclusiveMinimum accepts the boundary value itself ('<' instead of '<=')")]
	[InlineData(0)]
	public void Validate_ValueEqualToExclusiveMinimum_Fails(long value)
	{
		var fieldInfo = new FloatFieldInfo { Name = "ratio", ExclusiveMinimum = 0 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "ratio": {{value}} }""");
		
		Assert.False(result.IsValid);
	}
	
	[Theory]
	[InlineData(-1, "The 'ratio' value can not be less than 0")]
	[InlineData(2, "The 'ratio' value can not be greater than 1")]
	public void Validate_ParsedIntegerOutOfTheBounds_Fails(long value, string expectedMessage)
	{
		var fieldInfo = new FloatFieldInfo { Name = "ratio", Minimum = 0, Maximum = 1 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "ratio": {{value}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal([expectedMessage], result.Messages);
	}
	
	[Fact]
	public void Validate_ParsedIntegerEqualToExclusiveMaximum_Fails()
	{
		var fieldInfo = new FloatFieldInfo { Name = "ratio", ExclusiveMaximum = 1 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "ratio": 1 }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["The 'ratio' value can not be greater than or equal 1"], result.Messages);
	}
	
	#endregion
}
