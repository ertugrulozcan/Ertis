using Ertis.MongoDB.Queries.Tests.TestHelpers;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// The comparison, element and array operators ($eq, $ne, $gt, $gte, $lt, $lte, $in, $nin, $not, $exists, $type)
/// </summary>
public class QueryBuilderOperatorTests
{
	#region Comparison Methods
	
	public static TheoryData<IQuery, string> FieldComparisons()
	{
		return new TheoryData<IQuery, string>
		{
			{ QueryBuilder.Equals("a", 1), """{ "a": 1 }""" },
			{ QueryBuilder.NotEquals("a", "x"), """{ "a": { $ne: "x" } }""" },
			{ QueryBuilder.GreaterThan("a", 1), """{ "a": { $gt: 1 } }""" },
			{ QueryBuilder.GreaterThanOrEqual("a", 1), """{ "a": { $gte: 1 } }""" },
			{ QueryBuilder.LessThan("a", 1), """{ "a": { $lt: 1 } }""" },
			{ QueryBuilder.LessThanOrEqual("a", 1), """{ "a": { $lte: 1 } }""" }
		};
	}
	
	[Theory]
	[MemberData(nameof(FieldComparisons))]
	public void Comparison_OnAField_WritesTheOperator(IQuery query, string expected)
	{
		Assert.Equal(expected, query.ToString());
		QueryAssert.Filter(expected, query);
	}
	
	public static TheoryData<IQuery, string> OperatorFragments()
	{
		return new TheoryData<IQuery, string>
		{
			{ QueryBuilder.Equals(1), "{ $eq: 1 }" },
			{ QueryBuilder.NotEquals("x"), """{ $ne: "x" }""" },
			{ QueryBuilder.GreaterThan(1), "{ $gt: 1 }" },
			{ QueryBuilder.GreaterThanOrEqual(1), "{ $gte: 1 }" },
			{ QueryBuilder.LessThan(1), "{ $lt: 1 }" },
			{ QueryBuilder.LessThanOrEqual(1), "{ $lte: 1 }" },
			{ QueryBuilder.In(new[] { 1, 2 }), "{ $in: [ 1, 2 ] }" },
			{ QueryBuilder.Contains(new[] { 1, 2 }), "{ $in: [ 1, 2 ] }" },
			{ QueryBuilder.Nin(new[] { 1 }), "{ $nin: [ 1 ] }" },
			{ QueryBuilder.NotContains(new[] { 1 }), "{ $nin: [ 1 ] }" },
			{ QueryBuilder.Not(5), "{ $not: { $eq: 5 } }" },
			{ QueryBuilder.Exists(false), "{ $exists: false }" },
			{ QueryBuilder.TypeOf(BsonType.String), """{ $type: "string" }""" }
		};
	}
	
	[Theory]
	[MemberData(nameof(OperatorFragments))]
	public void Fragment_WritesTheOperatorWithoutAField(IQuery query, string expected)
	{
		Assert.Equal(expected, query.ToString());
		QueryAssert.Filter(expected, query);
	}
	
	#endregion
	
	#region Array Methods
	
	[Fact]
	public void In_OnAField_WritesTheValues()
	{
		Assert.Equal("""{ "a": { $in: [ 1, 2 ] } }""", QueryBuilder.In("a", new[] { 1, 2 }).ToString());
		Assert.Equal("""{ "a": { $in: [ "x", "y" ] } }""", QueryBuilder.Contains("a", new[] { "x", "y" }).ToString());
		Assert.Equal("""{ "a": { $nin: [ "x" ] } }""", QueryBuilder.Nin("a", new[] { "x" }).ToString());
		Assert.Equal("""{ "a": { $nin: [ "x" ] } }""", QueryBuilder.NotContains("a", new[] { "x" }).ToString());
	}
	
	[Fact]
	public void In_WithNoValues_WritesAnEmptyArray()
	{
		QueryAssert.Filter("""{ "a": { "$in": [] } }""", QueryBuilder.In("a", Array.Empty<int>()));
	}
	
	#endregion
	
	#region Element Methods
	
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Exists_OnAField_WritesTheFlag(bool value)
	{
		QueryAssert.Filter($$"""{ "a": { "$exists": {{value.ToString().ToLowerInvariant()}} } }""", QueryBuilder.Exists("a", value));
	}
	
	[Theory]
	[InlineData(BsonType.Double, "double")]
	[InlineData(BsonType.String, "string")]
	[InlineData(BsonType.Object, "object")]
	[InlineData(BsonType.Array, "array")]
	[InlineData(BsonType.BinData, "binData")]
	[InlineData(BsonType.Undefined, "undefined")]
	[InlineData(BsonType.ObjectId, "objectId")]
	[InlineData(BsonType.Bool, "bool")]
	[InlineData(BsonType.Date, "date")]
	[InlineData(BsonType.Null, "null")]
	[InlineData(BsonType.Regex, "regex")]
	[InlineData(BsonType.DbPointer, "dbPointer")]
	[InlineData(BsonType.Javascript, "javascript")]
	[InlineData(BsonType.Symbol, "symbol")]
	[InlineData(BsonType.JavascriptWithScope, "javascriptWithScope")]
	[InlineData(BsonType.Int, "int")]
	[InlineData(BsonType.Long, "long")]
	[InlineData(BsonType.Decimal, "decimal")]
	[InlineData(BsonType.MinKey, "minKey")]
	[InlineData(BsonType.MaxKey, "maxKey")]
	public void TypeOf_WritesTheMongoTypeAlias(BsonType type, string expectedAlias)
	{
		QueryAssert.Filter($$"""{ "a": { "$type": "{{expectedAlias}}" } }""", QueryBuilder.TypeOf("a", type));
	}
	
	[Fact]
	public void TypeOf_TimeStamp_WritesTheMongoTypeAlias()
	{
		QueryAssert.Filter("""{ "a": { "$type": "timestamp" } }""", QueryBuilder.TypeOf("a", BsonType.TimeStamp));
	}
	
	#endregion
	
	#region Not Methods
	
	[Fact]
	public void Not_OnAFieldValue_WritesNotEquals()
	{
		QueryAssert.Filter("""{ "a": { "$not": { "$eq": 5 } } }""", QueryBuilder.Not("a", 5));
	}
	
	[Fact]
	public void Not_OfAnExpression_NegatesItsOperator()
	{
		Assert.Equal("""{ "age": { $not: { $gt: 18 } } }""", QueryBuilder.Not(QueryBuilder.GreaterThan("age", 18)).ToString());
		Assert.Equal("""{ "name": { $not: { $regex: "^a" } } }""", QueryBuilder.Not(QueryBuilder.Regex("name", "^a")).ToString());
	}
	
	#endregion
}
