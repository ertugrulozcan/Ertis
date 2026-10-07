using Ertis.Schema.Dynamics;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Types.CustomTypes;

namespace Ertis.Schema.Tests.Types.CustomTypes;

public class DateTimeFieldInfoTests
{
	#region Date Methods
	
	[Fact]
	public void Validate_DateInFormat_Succeeds()
	{
		var result = SchemaValidation.ValidateField(new DateFieldInfo { Name = "birthday" }, """{ "birthday": "2026-01-31" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("31.01.2026")]
	[InlineData("2026-13-01")]
	[InlineData("2026/01/31")]
	[InlineData("tomorrow")]
	public void Validate_DateNotInFormat_Fails(string value)
	{
		var result = SchemaValidation.ValidateField(new DateFieldInfo { Name = "birthday" }, $$"""{ "birthday": "{{value}}" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Datetime is not valid. Datetime values must be 'yyyy-MM-dd' format."], result.Messages);
	}
	
	[Fact]
	public void Validate_Date_ConvertsTheValueToDateTime()
	{
		var result = SchemaValidation.ValidateField(new DateFieldInfo { Name = "birthday" }, """{ "birthday": "2026-01-31" }""");
		
		var value = Assert.IsType<DateTime>(result.Content.GetValue("birthday"));
		Assert.Equal(new DateTime(2026, 1, 31), value.Date);
	}
	
	[Fact]
	public void Validate_Date_ConvertsTheValueToUtcMidnight()
	{
		var result = SchemaValidation.ValidateField(new DateFieldInfo { Name = "birthday" }, """{ "birthday": "2026-01-31" }""");
		
		var value = Assert.IsType<DateTime>(result.Content.GetValue("birthday"));
		Assert.Equal(DateTimeKind.Utc, value.Kind);
		Assert.Equal(new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc), value);
	}
	
	[Theory]
	[InlineData("2025-12-31", false)]
	[InlineData("2026-01-01", true)]
	[InlineData("2026-12-31", true)]
	[InlineData("2027-01-01", false)]
	public void Validate_DateWithMinAndMaxValue_ValidatesTheRange(string value, bool expected)
	{
		var fieldInfo = new DateFieldInfo { Name = "birthday", MinValue = new DateTime(2026, 1, 1), MaxValue = new DateTime(2026, 12, 31) };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "birthday": "{{value}}" }""");
		
		Assert.Equal(expected, result.IsValid);
	}
	
	#endregion
	
	#region DateTime Methods
	
	[Theory]
	[InlineData("2026-01-31T10:00:00.000Z")]
	[InlineData("2026-01-31T10:00:00Z")]
	[InlineData("2026-01-31T10:00:00+03:00")]
	public void Validate_IsoDateTime_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new DateTimeFieldInfo { Name = "startsAt" }, $$"""{ "startsAt": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("31.01.2026 10:00")]
	[InlineData("2026-01-31")]
	[InlineData("now")]
	public void Validate_DateTimeNotInFormat_Fails(string value)
	{
		var result = SchemaValidation.ValidateField(new DateTimeFieldInfo { Name = "startsAt" }, $$"""{ "startsAt": "{{value}}" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Datetime is not valid. Datetime values must be 'yyyy-MM-ddTHH:mm:ss.fffZ' format."], result.Messages);
	}
	
	[Theory]
	[InlineData("2026-01-31T10:00:00.000Z", "2026-01-31T10:00:00.0000000Z")]
	[InlineData("2026-01-31T10:00:00Z", "2026-01-31T10:00:00.0000000Z")]
	[InlineData("2026-01-31T10:00:00.1234567Z", "2026-01-31T10:00:00.1234567Z")]
	[InlineData("2026-01-31T10:00:00+03:00", "2026-01-31T07:00:00.0000000Z")]
	[InlineData("2026-01-31T10:00:00.500-02:30", "2026-01-31T12:30:00.5000000Z")]
	[InlineData("2026-01-31T10:00:00", "2026-01-31T10:00:00.0000000Z")]
	public void Validate_DateTime_ConvertsTheValueToUtc(string value, string expectedUtc)
	{
		var result = SchemaValidation.ValidateField(new DateTimeFieldInfo { Name = "startsAt" }, $$"""{ "startsAt": "{{value}}" }""");
		
		var dateTime = Assert.IsType<DateTime>(result.Content.GetValue("startsAt"));
		Assert.Equal(DateTimeKind.Utc, dateTime.Kind);
		Assert.Equal(expectedUtc, dateTime.ToString("O"));
	}
	
	[Theory]
	[InlineData(DateTimeKind.Utc)]
	[InlineData(DateTimeKind.Unspecified)]
	public void Validate_DateTimeValue_IsNormalizedToUtc(DateTimeKind kind)
	{
		var content = DynamicObject.Create(new Dictionary<string, object?> { ["startsAt"] = new DateTime(2026, 1, 31, 10, 0, 0, kind) });
		
		var result = SchemaValidation.Validate(TestSchema.Of(new DateTimeFieldInfo { Name = "startsAt" }), content);
		
		Assert.True(result.IsValid);
		Assert.Equal(new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc), result.Content.GetValue("startsAt"));
		Assert.Equal(DateTimeKind.Utc, ((DateTime) result.Content.GetValue("startsAt")!).Kind);
	}
	
	[Fact]
	public void Validate_LocalDateTimeValue_IsConvertedToUtc()
	{
		var local = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Local);
		var content = DynamicObject.Create(new Dictionary<string, object?> { ["startsAt"] = local });
		
		var result = SchemaValidation.Validate(TestSchema.Of(new DateTimeFieldInfo { Name = "startsAt" }), content);
		
		Assert.Equal(local.ToUniversalTime(), result.Content.GetValue("startsAt"));
	}
	
	[Theory]
	[InlineData("2026-01-31T23:59:59Z", true)]
	[InlineData("2026-02-01T00:00:00Z", false)]
	[InlineData("2026-02-01T02:00:00+03:00", true)]
	public void Validate_DateTimeWithUnspecifiedMaxValue_ComparesInUtc(string value, bool expected)
	{
		var fieldInfo = new DateTimeFieldInfo { Name = "startsAt", MaxValue = new DateTime(2026, 1, 31, 23, 59, 59) };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "startsAt": "{{value}}" }""");
		
		Assert.Equal(expected, result.IsValid);
	}
	
	[Fact]
	public void Validate_DateOutOfTheRange_WritesTheBoundCultureInvariant()
	{
		using var _ = new CultureScope("tr-TR");
		var fieldInfo = new DateFieldInfo { Name = "birthday", MaxValue = new DateTime(2026, 12, 31) };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "birthday": "2027-01-01" }""");
		
		Assert.Equal(["Date can not be greater than 2026-12-31"], result.Messages);
	}
	
	[Fact]
	public void Validate_DateTimeLikeStringInAnAdditionalProperty_StaysAString()
	{
		var schema = new TestSchema { Properties = [new DateTimeFieldInfo { Name = "startsAt" }], AllowAdditionalProperties = true };
		
		var result = SchemaValidation.Validate(schema, """{ "startsAt": "2026-01-31T10:00:00Z", "note": "2026-01-31T10:00:00Z" }""");
		
		Assert.IsType<DateTime>(result.Content.GetValue("startsAt"));
		Assert.Equal("2026-01-31T10:00:00Z", result.Content.GetValue("note"));
	}
	
	[Fact]
	public void Validate_FormatPatternWithNumberAndDate_IsCultureInvariant()
	{
		using var _ = new CultureScope("tr-TR");
		var schema = TestSchema.Of(
			new FloatFieldInfo { Name = "ratio" },
			new StringFieldInfo { Name = "code" },
			new StringFieldInfo { Name = "label", FormatPattern = "{code}-{ratio}" });
		
		var result = SchemaValidation.Validate(schema, """{ "ratio": 0.5, "code": "A" }""");
		
		Assert.Equal("A-0.5", result.Content.GetValue("label"));
	}
	
	#endregion
}
