using System.Text.Json;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// Every query is a strict json (quoted operators, extended json for ids, dates and the non-finite numbers), so it can be
/// sent to an API which reads its body with System.Text.Json, and MongoDB reads it as the same filter as before.
/// </summary>
public class QueryBuilderStrictJsonTests
{
	#region Fields
	
	private static readonly DateTime SampleDate = new(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc);
	
	#endregion
	
	#region Methods
	
	public static TheoryData<string, IQuery> Queries => new()
	{
		{ "comparison", QueryBuilder.Where(QueryBuilder.Equals("a", 1), QueryBuilder.NotEquals("b", "x"), QueryBuilder.Combine("c", QueryBuilder.GreaterThan(1), QueryBuilder.LessThanOrEqual(5))) },
		{ "in / nin", QueryBuilder.Where(QueryBuilder.Contains("a", new[] { 1, 2 }), QueryBuilder.NotContains("b", Array.Empty<string>())) },
		{ "logical", QueryBuilder.And(QueryBuilder.Or(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2)), QueryBuilder.Nor(QueryBuilder.Equals("c", 3))) },
		{ "not", QueryBuilder.Where(QueryBuilder.Not("a", 5), QueryBuilder.Not(QueryBuilder.GreaterThan("b", 1))) },
		{ "element", QueryBuilder.Where(QueryBuilder.Exists("a", true), QueryBuilder.TypeOf("b", BsonType.ObjectId)) },
		{ "regex", QueryBuilder.Regex("name", "^ja", RegexOptions.CaseInsensitivity) },
		{ "text", QueryBuilder.Where(QueryBuilder.Equals("membership_id", "m"), QueryBuilder.FullTextSearch("jane doe", "en", true, true)) },
		{ "elemMatch", QueryBuilder.ElemMatch("accounts", QueryBuilder.Equals("provider", "google"), QueryBuilder.Equals("user_id", "1")) },
		{ "object id", QueryBuilder.Where(QueryBuilder.Equals("_id", QueryBuilder.ObjectId("65a0f0c2e4b0a1b2c3d4e5f6"))) },
		{ "dates", QueryBuilder.Where(QueryBuilder.Equals("a", SampleDate), QueryBuilder.Equals("b", new DateTimeOffset(SampleDate)), QueryBuilder.GreaterThan("c", QueryBuilder.ISODate(SampleDate))) },
		{ "non-finite numbers", QueryBuilder.Where(QueryBuilder.Equals("a", double.NaN), QueryBuilder.Equals("b", float.PositiveInfinity)) },
		{ "where / select", QueryBuilder.WhereOut(QueryBuilder.Equals("a", 1)) },
		{ "projection", QueryBuilder.Select(new Dictionary<string, bool> { ["a"] = true }) }
	};
	
	[Theory]
	[MemberData(nameof(Queries))]
	public void Query_IsAStrictJsonReadByMongoDB(string name, IQuery query)
	{
		var json = query.ToString();
		
		var exception = Record.Exception(() => JsonDocument.Parse(json).Dispose());
		Assert.True(exception == null, $"{name}: not a strict json: {json}");
		Assert.NotNull(BsonDocument.Parse(json));
	}
	
	[Fact]
	public void ObjectIdAndDate_AreReadByMongoDBAsTheirTypes()
	{
		var query = QueryBuilder.Where(QueryBuilder.Equals("_id", QueryBuilder.ObjectId("65a0f0c2e4b0a1b2c3d4e5f6")), QueryBuilder.Equals("t", SampleDate));
		
		var document = BsonDocument.Parse(query.ToString());
		
		Assert.IsType<BsonObjectId>(document["_id"]);
		Assert.Equal(new BsonDateTime(SampleDate), document["t"]);
	}
	
	#endregion
}
