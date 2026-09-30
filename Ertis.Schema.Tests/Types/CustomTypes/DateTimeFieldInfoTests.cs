using Ertis.Schema.Tests.TestHelpers;
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
	
	[Fact(Skip = "Bug (backlog #9): date values are stored as DateTimeKind.Unspecified (the MongoDB driver treats them as server-local time)")]
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
	
	[Theory(Skip = "Bug (backlog #9): date-time values lose their kind/offset (culture ToString + DateTime.TryParse -> Unspecified), so they shift by the server offset when stored")]
	[InlineData("2026-01-31T10:00:00.000Z", "2026-01-31T10:00:00.0000000Z")]
	[InlineData("2026-01-31T10:00:00+03:00", "2026-01-31T07:00:00.0000000Z")]
	public void Validate_DateTime_ConvertsTheValueToUtc(string value, string expectedUtc)
	{
		var result = SchemaValidation.ValidateField(new DateTimeFieldInfo { Name = "startsAt" }, $$"""{ "startsAt": "{{value}}" }""");
		
		var dateTime = Assert.IsType<DateTime>(result.Content.GetValue("startsAt"));
		Assert.Equal(DateTimeKind.Utc, dateTime.Kind);
		Assert.Equal(expectedUtc, dateTime.ToString("O"));
	}
	
	#endregion
}
