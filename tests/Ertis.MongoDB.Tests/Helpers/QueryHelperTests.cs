using Ertis.MongoDB.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Ertis.MongoDB.Tests.Helpers;

public class QueryHelperTests
{
	#region Constants
	
	private const string Id1 = "65a1b2c3d4e5f6a7b8c9d0e1";
	private const string Id2 = "65a1b2c3d4e5f6a7b8c9d0e2";
	
	#endregion
	
	#region Methods
	
	private static BsonDocument EnsureQuery(string query)
	{
		return BsonDocument.Parse(QueryHelper.EnsureObjectIdsAndISODates(query));
	}
	
	#endregion
	
	#region Input Methods
	
	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void EnsureObjectIdsAndISODates_WithAnEmptyQuery_ReturnsIt(string? query)
	{
		Assert.Equal(query, QueryHelper.EnsureObjectIdsAndISODates(query!));
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_WithAnInvalidJson_ReturnsItAsItIs()
	{
		Assert.Equal("{ invalid", QueryHelper.EnsureObjectIdsAndISODates("{ invalid"));
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_KeepsTheOtherValues()
	{
		var query = EnsureQuery("""{ "name": "Jane", "age": { "$gt": 18 }, "ratio": 1.5, "active": true, "none": null, "tags": { "$in": ["a", "b"] } }""");
		
		Assert.Equal(BsonDocument.Parse("""{ "name": "Jane", "age": { "$gt": 18 }, "ratio": 1.5, "active": true, "none": null, "tags": { "$in": ["a", "b"] } }"""), query);
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_WithTheShellSyntax_ReadsIt()
	{
		var query = EnsureQuery($$"""{ _id: "{{Id1}}", age: { $gt: 18 } }""");
		
		Assert.Equal(new BsonDocument { { "_id", ObjectId.Parse(Id1) }, { "age", new BsonDocument("$gt", 18) } }, query);
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_WithShellConstructors_KeepsThem()
	{
		var query = EnsureQuery($$"""{ "_id": ObjectId("{{Id1}}"), "created_at": { "$gt": ISODate("2026-01-31T10:00:00Z") } }""");
		
		Assert.Equal(ObjectId.Parse(Id1), query["_id"]);
		Assert.Equal(new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc), query["created_at"]["$gt"].ToUniversalTime());
	}
	
	/// <summary>
	/// The extended json of a BsonDocument (e.g. ToJson() of a filter built with the driver)
	/// </summary>
	[Fact]
	public void EnsureObjectIdsAndISODates_WithTheExtendedJson_KeepsTheValues()
	{
		var filter = new BsonDocument { { "_id", ObjectId.Parse(Id1) }, { "created_at", new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc) } };
		
		Assert.Equal(filter, EnsureQuery(filter.ToJson()));
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_WithAnArray_ConvertsTheStages()
	{
		var stages = BsonSerializer.Deserialize<BsonArray>(QueryHelper.EnsureObjectIdsAndISODates($$"""[{ "$match": { "_id": "{{Id1}}" } }, { "$limit": 5 }]"""));
		
		Assert.Equal(ObjectId.Parse(Id1), stages[0]["$match"]["_id"]);
		Assert.Equal(5, stages[1]["$limit"].AsInt32);
	}
	
	#endregion
	
	#region ObjectId Methods
	
	[Fact]
	public void EnsureObjectIdsAndISODates_ConvertsTheIdValues()
	{
		Assert.Equal(ObjectId.Parse(Id1), EnsureQuery($$"""{ "_id": "{{Id1}}" }""")["_id"]);
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_ConvertsTheIdOperatorValues()
	{
		var query = EnsureQuery($$"""{ "_id": { "$in": ["{{Id1}}", "{{Id2}}"] } }""");
		
		Assert.Equal(new BsonArray { ObjectId.Parse(Id1), ObjectId.Parse(Id2) }, query["_id"]["$in"]);
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_ConvertsTheNestedIdValues()
	{
		Assert.Equal(ObjectId.Parse(Id1), EnsureQuery($$"""{ "owner": { "_id": "{{Id1}}" } }""")["owner"]["_id"]);
		Assert.Equal(ObjectId.Parse(Id1), EnsureQuery($$"""{ "$and": [{ "_id": "{{Id1}}" }] }""")["$and"][0]["_id"]);
	}
	
	[Theory]
	[InlineData("""{ "_id": "not-an-object-id" }""")]
	[InlineData("""{ "owner_id": "65a1b2c3d4e5f6a7b8c9d0e1" }""")]
	[InlineData("""{ "name": "65a1b2c3d4e5f6a7b8c9d0e1" }""")]
	[InlineData("""{ "_id": 5 }""")]
	public void EnsureObjectIdsAndISODates_KeepsTheOtherIdLikeValues(string json)
	{
		Assert.Equal(BsonDocument.Parse(json), EnsureQuery(json));
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_ConvertsTheIdOperatorValuesInLogicalOperators()
	{
		var query = EnsureQuery($$"""{ "$or": [{ "_id": { "$ne": "{{Id1}}" } }, { "owner._id": { "$in": ["{{Id2}}"] } }] }""");
		
		Assert.Equal(ObjectId.Parse(Id1), query["$or"][0]["_id"]["$ne"]);
		Assert.Equal(ObjectId.Parse(Id2), query["$or"][1]["owner._id"]["$in"][0]);
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_ConvertsTheDottedIdFields()
	{
		Assert.Equal(ObjectId.Parse(Id1), EnsureQuery($$"""{ "owner._id": "{{Id1}}" }""")["owner._id"]);
	}
	
	#endregion
	
	#region Date Methods
	
	[Fact]
	public void EnsureObjectIdsAndISODates_ConvertsTheUtcDates()
	{
		var value = EnsureQuery("""{ "created_at": { "$gt": "2026-01-31T10:00:00Z" } }""")["created_at"]["$gt"];
		
		Assert.True(value.IsValidDateTime);
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_KeepsTheInstantOfTheDates()
	{
		var query = EnsureQuery("""{ "a": "2026-01-31T10:00:00Z", "b": "2026-01-31T13:00:00+03:00" }""");
		
		var expected = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc);
		Assert.Equal(expected, query["a"].ToUniversalTime());
		Assert.Equal(expected, query["b"].ToUniversalTime());
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_KeepsTheMilliseconds()
	{
		var value = EnsureQuery("""{ "a": "2026-01-31T10:00:00.123Z" }""")["a"];
		
		Assert.Equal(new DateTime(2026, 1, 31, 10, 0, 0, 123, DateTimeKind.Utc), value.ToUniversalTime());
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_ConvertsTheDatesWithoutTime()
	{
		var value = EnsureQuery("""{ "a": "2026-01-31" }""")["a"];
		
		Assert.Equal(new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc), value.ToUniversalTime());
	}
	
	[Theory]
	[InlineData("Jane")]
	[InlineData("2026")]
	[InlineData("")]
	public void EnsureObjectIdsAndISODates_KeepsTheNonDateStrings(string value)
	{
		var json = new BsonDocument("a", value).ToJson();
		
		Assert.Equal(BsonDocument.Parse(json), EnsureQuery(json));
	}
	
	[Theory]
	[InlineData("1.2")]
	[InlineData("10:00")]
	[InlineData("1/2/2026")]
	[InlineData("31 January 2026")]
	public void EnsureObjectIdsAndISODates_KeepsTheStringsWhichAreNotIsoDates(string value)
	{
		var json = new BsonDocument("a", value).ToJson();
		
		Assert.Equal(BsonDocument.Parse(json), EnsureQuery(json));
	}
	
	#endregion
	
	#region Helper Methods
	
	[Fact]
	public void ObjectIdHelper_ConvertsOnlyTheIds()
	{
		var query = BsonDocument.Parse(QueryHelper.EnsureQuery($$"""{ "_id": "{{Id1}}", "a": "2026-01-31T10:00:00Z" }""", convertObjectIds: true, convertDates: false));
		
		Assert.Equal(ObjectId.Parse(Id1), query["_id"]);
		Assert.Equal("2026-01-31T10:00:00Z", query["a"]);
	}
	
	[Fact]
	public void ISODateHelper_ConvertsOnlyTheDates()
	{
		var query = BsonDocument.Parse(ISODateHelper.EnsureDatetimeFieldsToISODate($$"""{ "_id": "{{Id1}}", "a": "2026-01-31T10:00:00Z" }"""));
		
		Assert.Equal(Id1, query["_id"]);
		Assert.Equal(new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc), query["a"].ToUniversalTime());
	}
	
	[Theory]
	[InlineData("2026-01-31T10:00:00", "2026-01-31T10:00:00Z")]
	[InlineData("2026-01-31T10:00Z", "2026-01-31T10:00:00Z")]
	[InlineData("2026-01-31T10:00:00.1234567+01:00", "2026-01-31T09:00:00.1234567Z")]
	public void ISODateHelper_TryParseDateTime_ReturnsUtc(string value, string expected)
	{
		Assert.True(ISODateHelper.TryParseDateTime(value, out var dateTime));
		Assert.Equal(DateTimeKind.Utc, dateTime.Kind);
		Assert.Equal(DateTime.Parse(expected, null, System.Globalization.DateTimeStyles.AdjustToUniversal), dateTime);
	}
	
	[Fact]
	public void EnsureObjectIdsAndISODates_WritesTheShellSyntax()
	{
		Assert.Equal($$"""{ "_id" : ObjectId("{{Id1}}"), "a" : ISODate("2026-01-31T10:00:00.123Z") }""", QueryHelper.EnsureObjectIdsAndISODates($$"""{ "_id": "{{Id1}}", "a": "2026-01-31T10:00:00.123Z" }"""));
	}
	
	[Theory]
	[InlineData("{} extra")]
	[InlineData("   ")]
	public void EnsureObjectIdsAndISODates_WithTrailingOrNoContent_ReturnsItAsItIs(string query)
	{
		Assert.Equal(query, QueryHelper.EnsureObjectIdsAndISODates(query));
	}
	
	#endregion
}
