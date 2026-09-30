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
	
	[Theory]
	[InlineData(-0.5)]
	[InlineData(1.5)]
	public void Validate_ParsedDecimalOutOfTheBounds_Fails(double value)
	{
		var fieldInfo = new FloatFieldInfo { Name = "ratio", Minimum = 0, Maximum = 1 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "ratio": {{value.ToString(System.Globalization.CultureInfo.InvariantCulture)}} }""");
		
		Assert.False(result.IsValid);
		Assert.Single(result.Errors);
	}
	
	[Theory]
	[InlineData(0.5, true)]
	[InlineData(1.0, false)]
	[InlineData(0.0, false)]
	public void Validate_ParsedDecimalWithExclusiveBounds_ValidatesTheRange(double value, bool expected)
	{
		var fieldInfo = new FloatFieldInfo { Name = "ratio", ExclusiveMinimum = 0, ExclusiveMaximum = 1 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "ratio": {{value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)}} }""");
		
		Assert.Equal(expected, result.IsValid);
	}
	
	[Theory]
	[InlineData(1.5f, false)]
	[InlineData(0.5f, true)]
	public void Validate_SingleValue_EnforcesTheBounds(float value, bool expected)
	{
		var content = DynamicObject.Create(new Dictionary<string, object?>());
		content.SetValue("ratio", value, true);
		
		var result = SchemaValidation.Validate(TestSchema.Of(new FloatFieldInfo { Name = "ratio", Maximum = 1 }), content);
		
		Assert.Equal(expected, result.IsValid);
	}
	
	[Fact]
	public void Validate_DecimalValue_FailsWithTypeMismatchOnly()
	{
		var content = DynamicObject.Create(new Dictionary<string, object?>());
		content.SetValue("ratio", 1.5m, true);
		
		var result = SchemaValidation.Validate(TestSchema.Of(new FloatFieldInfo { Name = "ratio", Maximum = 1 }), content);
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. 'ratio' is must be 'Nullable`1'"], result.Messages);
	}
	
	[Fact]
	public void Validate_OutOfTheBounds_WritesTheBoundCultureInvariant()
	{
		using var _ = new CultureScope("tr-TR");
		var fieldInfo = new FloatFieldInfo { Name = "ratio", Maximum = 1.5 };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "ratio": 2.5 }""");
		
		Assert.Equal(["The 'ratio' value can not be greater than 1.5"], result.Messages);
	}
	
	[Theory]
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
