using Ertis.MongoDB.Queries.Tests.TestHelpers;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// A field repeated in a Where or a Combine: the keys of an object must be distinct (MongoDB rejects a repeated key),
/// so the operators of the field are merged, or the queries are combined with $and.
/// </summary>
public class QueryBuilderRepeatedFieldTests
{
	#region Methods
	
	[Fact]
	public void Where_RangeOnAField_MergesTheOperators()
	{
		var query = QueryBuilder.Where(QueryBuilder.GreaterThan("age", 18), QueryBuilder.LessThan("age", 65));
		
		Assert.Equal("""{ "age": { "$gt": 18, "$lt": 65 } }""", query.ToString());
		QueryAssert.Filter("""{ "age": { "$gt": 18, "$lt": 65 } }""", query);
	}
	
	[Fact]
	public void Where_RepeatedFieldAmongOthers_KeepsTheOrderOfTheFirstOccurrence()
	{
		var query = QueryBuilder.Where(
			QueryBuilder.GreaterThanOrEqual("age", 18),
			QueryBuilder.Equals("membership_id", "m"),
			QueryBuilder.LessThan("age", 65),
			QueryBuilder.NotEquals("age", 30));
		
		Assert.Equal("""{ "age": { "$gte": 18, "$lt": 65, "$ne": 30 }, "membership_id": "m" }""", query.ToString());
	}
	
	[Theory]
	[MemberData(nameof(UnmergeableQueries))]
	public void Where_UnmergeableRepeatedKeys_AreCombinedWithAnd(string name, IQuery[] queries, string expectedFilter)
	{
		var query = QueryBuilder.Where(queries);
		
		QueryAssert.Filter(expectedFilter, query);
		Assert.True(QueryAssert.Parse(query).Contains("$and"), name);
	}
	
	public static TheoryData<string, IQuery[], string> UnmergeableQueries => new()
	{
		{ "two values", [QueryBuilder.Equals("a", 1), QueryBuilder.Equals("a", 2)], """{ "$and": [ { "a": 1 }, { "a": 2 } ] }""" },
		{ "same operator", [QueryBuilder.GreaterThan("a", 1), QueryBuilder.GreaterThan("a", 5)], """{ "$and": [ { "a": { "$gt": 1 } }, { "a": { "$gt": 5 } } ] }""" },
		{ "value and operator", [QueryBuilder.Equals("a", 1), QueryBuilder.GreaterThan("a", 0)], """{ "$and": [ { "a": 1 }, { "a": { "$gt": 0 } } ] }""" },
		{ "two $or", [QueryBuilder.Or(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 1)), QueryBuilder.Or(QueryBuilder.Equals("c", 1), QueryBuilder.Equals("d", 1))], """{ "$and": [ { "$or": [ { "a": 1 }, { "b": 1 } ] }, { "$or": [ { "c": 1 }, { "d": 1 } ] } ] }""" },
		{ "a field of a flattened $and", [QueryBuilder.And(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2)), QueryBuilder.Equals("a", 3)], """{ "$and": [ { "a": 1, "b": 2 }, { "a": 3 } ] }""" }
	};
	
	[Fact]
	public void WhereOut_RepeatedField_IsWrappedInWhere()
	{
		var query = QueryBuilder.WhereOut(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("a", 2));
		
		QueryAssert.Filter("""{ "where": { "$and": [ { "a": 1 }, { "a": 2 } ] } }""", query);
	}
	
	[Fact]
	public void Combine_RangeOnAField_MergesTheOperators()
	{
		var query = QueryBuilder.Combine(QueryBuilder.GreaterThan("age", 18), QueryBuilder.LessThan("age", 65));
		
		QueryAssert.Filter("""{ "age": { "$gt": 18, "$lt": 65 } }""", query);
	}
	
	[Fact]
	public void Where_DistinctFields_AreWrittenAsBefore()
	{
		Assert.Equal("""{ "a": 1, "b": { "$gt": 2 } }""", QueryBuilder.Where(QueryBuilder.Equals("a", 1), QueryBuilder.GreaterThan("b", 2)).ToString());
	}
	
	#endregion
}
