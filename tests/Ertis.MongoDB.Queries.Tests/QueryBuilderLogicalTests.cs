using Ertis.MongoDB.Queries.Tests.TestHelpers;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// The logical operators ($and, $or, $nor) and the query wrappers (Where, WhereOut, Select, Combine)
/// </summary>
public class QueryBuilderLogicalTests
{
	#region Logical Operator Methods
	
	[Fact]
	public void And_OfFieldExpressions_MergesThemIntoOneObject()
	{
		var query = QueryBuilder.And(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2));
		
		Assert.Equal("""{ "a": 1, "b": 2 }""", query.ToString());
	}
	
	[Fact]
	public void And_WithANonFieldQuery_WritesTheAndOperator()
	{
		var query = QueryBuilder.And(QueryBuilder.Equals("a", 1), QueryBuilder.Or(QueryBuilder.Equals("b", 2), QueryBuilder.Equals("c", 3)));
		
		Assert.Equal("""{ "$and": [ { "a": 1 }, { "$or": [ { "b": 2 }, { "c": 3 } ] } ] }""", query.ToString());
		QueryAssert.Filter("""{ "$and": [{ "a": 1 }, { "$or": [{ "b": 2 }, { "c": 3 }] }] }""", query);
	}
	
	[Fact]
	public void And_OnTheSameField_KeepsBothConditions()
	{
		var query = QueryBuilder.And(QueryBuilder.GreaterThan("age", 18), QueryBuilder.LessThan("age", 65));
		
		var document = QueryAssert.Parse(query);
		
		Assert.Equal(1, document.Contains("$and") ? document["$and"].AsBsonArray.Count(x => x.AsBsonDocument.Contains("age") && x["age"].AsBsonDocument.Contains("$gt")) : 0);
		Assert.Equal(1, document.Contains("$and") ? document["$and"].AsBsonArray.Count(x => x.AsBsonDocument.Contains("age") && x["age"].AsBsonDocument.Contains("$lt")) : 0);
	}
	
	[Fact]
	public void Or_WritesTheOrOperator()
	{
		var query = QueryBuilder.Or(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2));
		
		Assert.Equal("""{ "$or": [ { "a": 1 }, { "b": 2 } ] }""", query.ToString());
		Assert.Equal(query.ToString(), QueryBuilder.Or(new List<IQuery> { QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2) }).ToString());
	}
	
	[Fact]
	public void Nor_WritesTheNorOperator()
	{
		var query = QueryBuilder.Nor(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2));
		
		Assert.Equal("""{ "$nor": [ { "a": 1 }, { "b": 2 } ] }""", query.ToString());
		Assert.Equal(query.ToString(), QueryBuilder.Nor(new List<IQuery> { QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2) }).ToString());
	}
	
	[Fact]
	public void And_OfAnEnumerable_WritesLikeTheParams()
	{
		var queries = new List<IQuery> { QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2) };
		
		Assert.Equal(QueryBuilder.And(queries.ToArray()).ToString(), QueryBuilder.And(queries).ToString());
	}
	
	#endregion
	
	#region Wrapper Methods
	
	[Fact]
	public void Where_OfQueries_CombinesThemWithoutAWrapper()
	{
		Assert.Equal("""{ "a": 1, "b": "x" }""", QueryBuilder.Where(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", "x")).ToString());
		Assert.Equal("""{ "a": 1 }""", QueryBuilder.Where(new List<IQuery> { QueryBuilder.Equals("a", 1) }).ToString());
	}
	
	[Fact]
	public void WhereOut_WrapsTheQueryWithWhere()
	{
		Assert.Equal("""{ "where": { "name": "Jane" } }""", QueryBuilder.WhereOut("name", "Jane").ToString());
		Assert.Equal("""{ "where": { "a": 1, "b": "x" } }""", QueryBuilder.WhereOut(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", "x")).ToString());
		Assert.Equal("""{ "where": { "a": 1 } }""", QueryBuilder.WhereOut(new List<IQuery> { QueryBuilder.Equals("a", 1) }).ToString());
	}
	
	[Fact]
	public void WhereOut_OfEachValueType_WrapsTheValue()
	{
		Assert.Equal("""{ "where": { "n": 5 } }""", QueryBuilder.WhereOut("n", 5).ToString());
		Assert.Equal("""{ "where": { "n": 5 } }""", QueryBuilder.WhereOut("n", 5L).ToString());
		Assert.Equal("""{ "where": { "d": 1.5 } }""", QueryBuilder.WhereOut("d", 1.5).ToString());
		Assert.Equal("""{ "where": { "f": 1.5 } }""", QueryBuilder.WhereOut("f", 1.5f).ToString());
		Assert.Equal("""{ "where": { "b": true } }""", QueryBuilder.WhereOut("b", true).ToString());
		Assert.Equal("""{ "where": { "t": { "$date": "2026-01-31T10:00:00.000Z" } } }""", QueryBuilder.WhereOut("t", new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc)).ToString());
	}
	
	[Fact]
	public void Where_OnAFieldWithQueries_CombinesTheOperators()
	{
		var query = QueryBuilder.Where("age", [QueryBuilder.GreaterThan(18), QueryBuilder.LessThan(65)]);
		
		Assert.Equal("""{ "age": { "$gt": 18, "$lt": 65 } }""", query.ToString());
		QueryAssert.Filter("""{ "age": { "$gt": 18, "$lt": 65 } }""", query);
		Assert.Equal(query.ToString(), QueryBuilder.Combine("age", QueryBuilder.GreaterThan(18), QueryBuilder.LessThan(65)).ToString());
	}
	
	[Fact]
	public void Select_WritesTheProjection()
	{
		var query = QueryBuilder.Select(new Dictionary<string, bool> { ["a"] = true, ["b"] = false });
		
		Assert.Equal("""{ "select": { "a": 1, "b": 0 } }""", query.ToString());
	}
	
	[Fact]
	public void Combine_MergesTheQueries()
	{
		Assert.Equal("""{ "a": 1, "b": 2 }""", QueryBuilder.Combine(QueryBuilder.Equals("a", 1), QueryBuilder.Equals("b", 2)).ToString());
		Assert.Equal("""{ "a": 1 }""", QueryBuilder.Combine(new List<IQuery> { QueryBuilder.Equals("a", 1) }).ToString());
	}
	
	#endregion
}
