using Ertis.MongoDB.Queries.Tests.TestHelpers;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// The values written into the queries (strings, numbers, booleans, dates, ids)
/// </summary>
public class QueryBuilderValueTests
{
	#region Constants
	
	private static readonly DateTime SampleDate = new(2026, 1, 31, 10, 0, 0, 123, DateTimeKind.Utc);
	
	#endregion
	
	#region Where Methods
	
	[Fact]
	public void Where_String_WritesAQuotedString()
	{
		var query = QueryBuilder.Where("name", "Jane");
		
		Assert.Equal("""{ "name": "Jane" }""", query.ToString());
		QueryAssert.SingleField("name", "Jane", query);
	}
	
	[Theory]
	[InlineData(5, "5")]
	[InlineData(-7, "-7")]
	public void Where_Int_WritesTheNumber(int value, string expected)
	{
		Assert.Equal($$"""{ "n": {{expected}} }""", QueryBuilder.Where("n", value).ToString());
	}
	
	[Fact]
	public void Where_Long_WritesTheNumber()
	{
		var query = QueryBuilder.Where("n", 9007199254740993L);
		
		Assert.Equal("""{ "n": 9007199254740993 }""", query.ToString());
		QueryAssert.SingleField("n", 9007199254740993L, query);
	}
	
	[Fact]
	public void Where_DoubleAndFloat_WriteCultureInvariantNumbers()
	{
		using var _ = new CultureScope("tr-TR");
		
		Assert.Equal("""{ "d": 1.5 }""", QueryBuilder.Where("d", 1.5).ToString());
		Assert.Equal("""{ "f": 0.25 }""", QueryBuilder.Where("f", 0.25f).ToString());
		Assert.Equal("""{ "a": 1234.5 }""", QueryBuilder.Equals("a", 1234.5m).ToString());
	}
	
	[Theory]
	[InlineData(true, "true")]
	[InlineData(false, "false")]
	public void Where_Bool_WritesALowerCaseLiteral(bool value, string expected)
	{
		Assert.Equal($$"""{ "b": {{expected}} }""", QueryBuilder.Where("b", value).ToString());
	}
	
	[Fact]
	public void Where_DateTime_WritesADate()
	{
		// Extended json: MongoDB reads it as a date (a plain ISO string would be compared as a string)
		var query = QueryBuilder.Where("t", SampleDate);
		
		Assert.Equal("""{ "t": { "$date": "2026-01-31T10:00:00.123Z" } }""", query.ToString());
		QueryAssert.SingleField("t", new BsonDateTime(SampleDate), query);
	}
	
	[Fact]
	public void Where_Char_WritesAQuotedString()
	{
		QueryAssert.SingleField("c", "a", QueryBuilder.Where("c", 'a'));
	}
	
	[Fact]
	public void Equals_Null_WritesNull()
	{
		Assert.Equal("""{ "a": null }""", QueryBuilder.Equals<string?>("a", null).ToString());
	}
	
	[Fact]
	public void Equals_NaN_WritesTheShellLiteral()
	{
		QueryAssert.SingleField("a", double.NaN, QueryBuilder.Equals("a", double.NaN));
	}
	
	[Fact]
	public void Equals_NestedFieldPath_WritesTheDottedPath()
	{
		QueryAssert.SingleField("address.city", "Istanbul", QueryBuilder.Equals("address.city", "Istanbul"));
	}
	
	[Fact]
	public void Equals_EnumAndGuid_WriteStrings()
	{
		var enumQuery = QueryBuilder.Equals("a", DayOfWeek.Monday);
		var guidQuery = QueryBuilder.Equals("a", Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"));
		
		Assert.Equal("""{ "a": "Monday" }""", enumQuery.ToString());
		QueryAssert.SingleField("a", "0f8fad5b-d9cb-469f-a165-70867728950e", guidQuery);
	}
	
	[Fact]
	public void Equals_LocalDateTime_WritesTheUtcTime()
	{
		var local = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Local);
		
		var query = QueryBuilder.Equals("a", local);
		
		Assert.Equal($$"""{ "a": { "$date": "{{local.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fff}}Z" } }""", query.ToString());
	}
	
	[Fact]
	public void Equals_UnspecifiedDateTime_IsTreatedAsUtc()
	{
		Assert.Equal("""{ "a": { "$date": "2026-01-31T10:00:00.000Z" } }""", QueryBuilder.Equals("a", new DateTime(2026, 1, 31, 10, 0, 0)).ToString());
	}
	
	[Fact]
	public void Equals_DateTimeOffset_WritesTheUtcTime()
	{
		var query = QueryBuilder.Equals("a", new DateTimeOffset(2026, 1, 31, 10, 0, 0, TimeSpan.FromHours(3)));
		
		Assert.Equal("""{ "a": { "$date": "2026-01-31T07:00:00.000Z" } }""", query.ToString());
	}
	
	[Fact]
	public void Equals_Infinity_WritesTheInvariantLiteral()
	{
		using var _ = new CultureScope("tr-TR");
		
		QueryAssert.SingleField("a", double.PositiveInfinity, QueryBuilder.Equals("a", double.PositiveInfinity));
	}
	
	/// <summary>
	/// Json numbers can't express NaN and the infinities: they are written in extended json
	/// </summary>
	[Fact]
	public void Equals_NaNAndNegativeInfinity_WriteExtendedJson()
	{
		Assert.Equal("""{ "a": { "$numberDouble": "NaN" } }""", QueryBuilder.Equals("a", double.NaN).ToString());
		QueryAssert.SingleField("a", double.NaN, QueryBuilder.Equals("a", double.NaN));
		QueryAssert.SingleField("a", double.NegativeInfinity, QueryBuilder.Equals("a", float.NegativeInfinity));
	}
	
	#endregion
	
	#region ObjectId & ISODate Methods
	
	[Fact]
	public void ObjectId_WritesExtendedJson()
	{
		var query = QueryBuilder.Equals("_id", QueryBuilder.ObjectId("65a0f0c2e4b0a1b2c3d4e5f6"));
		
		Assert.Equal("""{ "$oid": "65a0f0c2e4b0a1b2c3d4e5f6" }""", QueryBuilder.ObjectId("65a0f0c2e4b0a1b2c3d4e5f6").ToString());
		Assert.Equal("_id", QueryBuilder.ObjectId("65a0f0c2e4b0a1b2c3d4e5f6").Field);
		QueryAssert.SingleField("_id", new BsonObjectId(global::MongoDB.Bson.ObjectId.Parse("65a0f0c2e4b0a1b2c3d4e5f6")), query);
	}
	
	[Fact]
	public void ISODate_WritesExtendedJson()
	{
		var query = QueryBuilder.Equals("t", QueryBuilder.ISODate(new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc)));
		
		Assert.Equal("""{ "t": { "$date": "2026-01-31T10:00:00Z" } }""", query.ToString());
		QueryAssert.SingleField("t", new BsonDateTime(new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc)), query);
	}
	
	[Fact]
	public void ISODate_KeepsTheMilliseconds()
	{
		QueryAssert.SingleField("t", new BsonDateTime(SampleDate), QueryBuilder.Equals("t", QueryBuilder.ISODate(SampleDate)));
	}
	
	[Fact]
	public void ISODate_OfALocalDate_WritesTheUtcTime()
	{
		var local = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Local);
		
		QueryAssert.SingleField("t", new BsonDateTime(local.ToUniversalTime()), QueryBuilder.Equals("t", QueryBuilder.ISODate(local)));
	}
	
	#endregion
	
	#region Other Value Types
	
	// The values were written with their ToString (culture dependent, unquoted): an invalid json, e.g. { "day": 31.01.2026 }
	
	[Fact]
	public void Equals_DateOnly_WritesTheUtcMidnight()
	{
		using var _ = new CultureScope("tr-TR");
		var query = QueryBuilder.Equals("day", new DateOnly(2026, 1, 31));
		
		Assert.Equal("""{ "day": { "$date": "2026-01-31T00:00:00.000Z" } }""", query.ToString());
		QueryAssert.SingleField("day", new BsonDateTime(new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc)), query);
	}
	
	[Fact]
	public void Equals_TimeOnlyAndTimeSpan_WriteInvariantStrings()
	{
		using var _ = new CultureScope("tr-TR");
		
		QueryAssert.SingleField("time", "10:30:15.5", QueryBuilder.Equals("time", new TimeOnly(10, 30, 15, 500)));
		QueryAssert.SingleField("time", "10:30:00", QueryBuilder.Equals("time", new TimeOnly(10, 30)));
		QueryAssert.SingleField("duration", "1.02:03:04", QueryBuilder.Equals("duration", new TimeSpan(1, 2, 3, 4)));
	}
	
	[Fact]
	public void Equals_Uri_WritesTheOriginalStringEscaped()
	{
		QueryAssert.SingleField("url", "https://example.com/a?b=\"c\"", QueryBuilder.Equals("url", new Uri("https://example.com/a?b=\"c\"", UriKind.Absolute)));
	}
	
	[Fact]
	public void Equals_LargeAndHalfNumbers_WriteTheNumbers()
	{
		QueryAssert.SingleField("a", 12345, QueryBuilder.Equals("a", (Int128) 12345));
		QueryAssert.SingleField("b", 1.5, QueryBuilder.Equals("b", (Half) 1.5));
		QueryAssert.SingleField("c", 42, QueryBuilder.Equals("c", new System.Numerics.BigInteger(42)));
	}
	
	[Fact]
	public void Equals_ConvertibleWithoutBaseType_WritesItsInvariantString()
	{
		// e.g. MongoDB.Bson.ObjectId (Ertis.MongoDB converts an id string of an _id field to an ObjectId)
		QueryAssert.SingleField("_id", "65a0f0c2e4b0a1b2c3d4e5f6", QueryBuilder.Equals("_id", global::MongoDB.Bson.ObjectId.Parse("65a0f0c2e4b0a1b2c3d4e5f6")));
	}
	
	[Fact]
	public void Equals_Array_WritesEachItem()
	{
		var date = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
		
		QueryAssert.SingleField("tags", new BsonArray { "a", "b" }, QueryBuilder.Equals("tags", new[] { "a", "b" }));
		QueryAssert.SingleField("dates", new BsonArray { new BsonDateTime(date) }, QueryBuilder.Equals("dates", new List<DateTime> { date }));
	}
	
	[Fact]
	public void Equals_Object_WritesTheJsonDocument()
	{
		QueryAssert.SingleField("address", new BsonDocument { { "city", "Istanbul" }, { "zip", 34000 } }, QueryBuilder.Equals("address", new { city = "Istanbul", zip = 34000 }));
	}
	
	#endregion
}
