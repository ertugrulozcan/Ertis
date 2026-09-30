using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types.Primitives;

public class IntegerFieldInfoTests
{
	#region Schema Methods
	
	[Fact]
	public void Create_WithMinimumGreaterThanMaximum_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new IntegerFieldInfo { Name = "age", Maximum = 10, Minimum = 20 });
		
		Assert.Equal("The 'minimum' value can not be greater than the 'maximum' value (age)", exception.Message);
	}
	
	[Fact]
	public void Create_WithExclusiveMinimumGreaterThanExclusiveMaximum_Throws()
	{
		var exception = Assert.Throws<FieldValidationException>(() => new IntegerFieldInfo { Name = "age", ExclusiveMaximum = 10, ExclusiveMinimum = 20 });
		
		Assert.Equal("The 'exclusiveMinimum' value can not be greater than the 'exclusiveMaximum' value (age)", exception.Message);
	}
	
	[Theory]
	[InlineData(0)]
	[InlineData(-2)]
	public void Create_WithNonPositiveMultipleOf_Throws(int multipleOf)
	{
		var exception = Assert.Throws<FieldValidationException>(() => new IntegerFieldInfo { Name = "count", MultipleOf = multipleOf });
		
		Assert.Equal("The 'multipleOf' value can not be less than or equal zero (count)", exception.Message);
	}
	
	#endregion
	
	#region Data Validation Methods
	
	[Theory]
	[InlineData("0")]
	[InlineData("-5")]
	[InlineData("9007199254740993")]
	public void Validate_WithInteger_Succeeds(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new IntegerFieldInfo { Name = "count" }, $$"""{ "count": {{jsonValue}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("1.5")]
	[InlineData("\"5\"")]
	[InlineData("true")]
	public void Validate_WithNonInteger_FailsWithTypeMismatch(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new IntegerFieldInfo { Name = "count" }, $$"""{ "count": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. 'count' is must be 'Nullable`1'"], result.Messages);
	}
	
	[Fact]
	public void Validate_ParsedIntegerValue_IsAnInt64()
	{
		var content = DynamicObject.Parse("""{ "count": 5 }""");
		
		Assert.IsType<long>(content.GetValue("count"));
	}
	
	[Theory]
	[InlineData("{ \"minimum\": 0 }", -1)]
	[InlineData("{ \"maximum\": 100 }", 101)]
	[InlineData("{ \"exclusiveMinimum\": 0 }", -1)]
	[InlineData("{ \"exclusiveMaximum\": 10 }", 10)]
	[InlineData("{ \"multipleOf\": 5 }", 7)]
	public void Validate_ParsedValueOutOfTheBounds_Fails(string rule, long value)
	{
		var result = SchemaValidation.ValidateField(CreateField(rule), $$"""{ "count": {{value}} }""");
		
		Assert.False(result.IsValid);
	}
	
	[Theory]
	[InlineData(0)]
	public void Validate_ValueEqualToExclusiveMinimum_Fails(int value)
	{
		var fieldInfo = new IntegerFieldInfo { Name = "count", ExclusiveMinimum = 0 };
		var content = DynamicObject.Create(new Dictionary<string, object?>());
		content.SetValue("count", value, true);
		
		var result = SchemaValidation.Validate(TestSchema.Of(fieldInfo), content);
		
		Assert.False(result.IsValid);
	}
	
	[Theory]
	[InlineData("{ \"minimum\": 0 }", 0)]
	[InlineData("{ \"maximum\": 100 }", 100)]
	[InlineData("{ \"exclusiveMinimum\": 0 }", 1)]
	[InlineData("{ \"exclusiveMaximum\": 10 }", 9)]
	[InlineData("{ \"multipleOf\": 5 }", 10)]
	public void Validate_ValueInTheBounds_Succeeds(string rule, long value)
	{
		var result = SchemaValidation.ValidateField(CreateField(rule), $$"""{ "count": {{value}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("{ \"minimum\": 0 }", -1, "The 'count' value can not be less than 0")]
	[InlineData("{ \"maximum\": 100 }", 101, "The 'count' value can not be greater than 100")]
	[InlineData("{ \"exclusiveMaximum\": 10 }", 10, "The 'count' value can not be greater than or equal 10")]
	[InlineData("{ \"multipleOf\": 5 }", 7, "The 'count' value must be an exact multiple of the 5")]
	public void Validate_Int32ValueOutOfTheBounds_Fails(string rule, int value, string expectedMessage)
	{
		var content = DynamicObject.Create(new Dictionary<string, object?>());
		content.SetValue("count", value, true);
		
		var result = SchemaValidation.Validate(TestSchema.Of(CreateField(rule)), content);
		
		Assert.False(result.IsValid);
		Assert.Equal([expectedMessage], result.Messages);
	}
	
	public static TheoryData<object, bool> ClrValues()
	{
		return new TheoryData<object, bool>
		{
			{ 101L, false }, { 101, false }, { (short) 101, false }, { (byte) 101, false }, { (sbyte) 101, false },
			{ (ushort) 101, false }, { 101u, false },
			{ 100L, true }, { 100, true }, { (short) 100, true }, { 100u, true }
		};
	}
	
	[Theory]
	[MemberData(nameof(ClrValues))]
	public void Validate_IntegralClrValue_EnforcesTheBounds(object value, bool expected)
	{
		var content = DynamicObject.Create(new Dictionary<string, object?>());
		content.SetValue("count", value, true);
		
		var result = SchemaValidation.Validate(TestSchema.Of(new IntegerFieldInfo { Name = "count", Maximum = 100 }), content);
		
		Assert.True(expected == result.IsValid, $"{value.GetType().Name}: {string.Join(" | ", result.Messages)}");
	}
	
	[Fact]
	public void Validate_UInt64Value_FailsWithTypeMismatchOnly()
	{
		var content = DynamicObject.Create(new Dictionary<string, object?>());
		content.SetValue("count", 101UL, true);
		
		var result = SchemaValidation.Validate(TestSchema.Of(new IntegerFieldInfo { Name = "count", Maximum = 100 }), content);
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. 'count' is must be 'Nullable`1'"], result.Messages);
	}
	
	private static IntegerFieldInfo CreateField(string rule)
	{
		var bounds = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(rule)!;
		return new IntegerFieldInfo
		{
			Name = "count",
			Minimum = bounds.TryGetValue("minimum", out var minimum) ? minimum : null,
			Maximum = bounds.TryGetValue("maximum", out var maximum) ? maximum : null,
			ExclusiveMinimum = bounds.TryGetValue("exclusiveMinimum", out var exclusiveMinimum) ? exclusiveMinimum : null,
			ExclusiveMaximum = bounds.TryGetValue("exclusiveMaximum", out var exclusiveMaximum) ? exclusiveMaximum : null,
			MultipleOf = bounds.TryGetValue("multipleOf", out var multipleOf) ? multipleOf : null
		};
	}
	
	#endregion
}
