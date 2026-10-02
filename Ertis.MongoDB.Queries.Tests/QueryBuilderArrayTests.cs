using Ertis.MongoDB.Queries.Tests.TestHelpers;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// The array query operators ($elemMatch)
/// </summary>
public class QueryBuilderArrayTests
{
	#region ElemMatch Methods
	
	[Fact]
	public void ElemMatch_WithFieldConditions_MatchesOneElement()
	{
		var query = QueryBuilder.ElemMatch("connected_accounts", QueryBuilder.Equals("provider", "google"), QueryBuilder.Equals("user_id", "123"));
		
		Assert.Equal("""{ "connected_accounts": { "$elemMatch": { "provider": "google", "user_id": "123" } } }""", query.ToString());
		QueryAssert.Filter("""{ "connected_accounts": { "$elemMatch": { "provider": "google", "user_id": "123" } } }""", query);
	}
	
	[Fact]
	public void ElemMatch_WithOperators_MatchesOnePrimitiveElement()
	{
		var query = QueryBuilder.ElemMatch("scores", QueryBuilder.GreaterThanOrEqual(80), QueryBuilder.LessThan(85));
		
		QueryAssert.Filter("""{ "scores": { "$elemMatch": { "$gte": 80, "$lt": 85 } } }""", query);
	}
	
	[Fact]
	public void ElemMatch_OfAnEnumerable_WritesLikeTheParams()
	{
		var queries = new List<IQuery> { QueryBuilder.Equals("provider", "google"), QueryBuilder.Equals("user_id", "123") };
		
		Assert.Equal(QueryBuilder.ElemMatch("accounts", queries.ToArray()).ToString(), QueryBuilder.ElemMatch("accounts", queries).ToString());
		Assert.Equal(QueryBuilder.ElemMatch(queries.ToArray()).ToString(), QueryBuilder.ElemMatch(queries).ToString());
	}
	
	[Fact]
	public void ElemMatch_WithALogicalOperator_WritesItInsideTheElementQuery()
	{
		var query = QueryBuilder.ElemMatch("accounts",
			QueryBuilder.Or(QueryBuilder.Equals("provider", "google"), QueryBuilder.Equals("provider", "apple")),
			QueryBuilder.Equals("active", true));
		
		QueryAssert.Filter("""{ "accounts": { "$elemMatch": { "$or": [{ "provider": "google" }, { "provider": "apple" }], "active": true } } }""", query);
	}
	
	[Fact]
	public void ElemMatch_Fragment_CombinesWithOtherOperatorsOfTheField()
	{
		var query = QueryBuilder.Where("scores", new[] { QueryBuilder.ElemMatch(QueryBuilder.GreaterThan(90)), QueryBuilder.Exists(true) });
		
		QueryAssert.Filter("""{ "scores": { "$elemMatch": { "$gt": 90 }, "$exists": true } }""", query);
	}
	
	[Fact]
	public void ElemMatch_InALoginLikeQuery_KeepsTheValuesEscaped()
	{
		const string malicious = """x" }, "user_id": { "$ne": "x""";
		var query = QueryBuilder.And(
			QueryBuilder.Equals("membership_id", "m1"),
			QueryBuilder.ElemMatch("connected_accounts", QueryBuilder.Equals("provider", "google"), QueryBuilder.Equals("user_id", malicious)));
		
		var expected = new BsonDocument
		{
			{ "membership_id", "m1" },
			{ "connected_accounts", new BsonDocument("$elemMatch", new BsonDocument { { "provider", "google" }, { "user_id", malicious } }) }
		};
		Assert.Equal(expected, QueryAssert.Parse(query));
	}
	
	#endregion
}
