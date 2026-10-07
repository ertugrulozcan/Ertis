using Ertis.MongoDB.Queries.Tests.TestHelpers;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// $all, $size, $mod and the ranges (Between)
/// </summary>
public class QueryBuilderArrayAndRangeTests
{
	#region Constants
	
	private static readonly DateTime From = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	
	private static readonly DateTime To = new(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
	
	#endregion
	
	#region Array Methods
	
	[Fact]
	public void All_WritesTheValues()
	{
		Assert.Equal("""{ "tags": { "$all": [ "a", "b" ] } }""", QueryBuilder.All("tags", new[] { "a", "b" }).ToString());
		QueryAssert.Filter("""{ "$all": [ 1, 2 ] }""", QueryBuilder.All(new[] { 1, 2 }));
	}
	
	[Fact]
	public void Size_WritesTheNumberOfItems()
	{
		Assert.Equal("""{ "tags": { "$size": 0 } }""", QueryBuilder.Size("tags", 0).ToString());
		QueryAssert.Filter("""{ "$size": 3 }""", QueryBuilder.Size(3));
	}
	
	[Fact]
	public void Size_Negative_Throws()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => QueryBuilder.Size("tags", -1));
	}
	
	#endregion
	
	#region Mod Methods
	
	[Fact]
	public void Mod_WritesTheDivisorAndTheRemainder()
	{
		Assert.Equal("""{ "n": { "$mod": [ 4, 0 ] } }""", QueryBuilder.Mod("n", 4, 0).ToString());
		QueryAssert.Filter("""{ "$mod": [ 2, 1 ] }""", QueryBuilder.Mod(2, 1));
	}
	
	[Fact]
	public void Mod_ZeroDivisor_Throws()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => QueryBuilder.Mod("n", 0, 1));
	}
	
	#endregion
	
	#region Between Methods
	
	[Fact]
	public void Between_IncludesBothBoundsByDefault()
	{
		Assert.Equal("""{ "age": { "$gte": 18, "$lte": 65 } }""", QueryBuilder.Between("age", 18, 65).ToString());
	}
	
	[Theory]
	[InlineData(true, false, "$gte", "$lt")]
	[InlineData(false, true, "$gt", "$lte")]
	[InlineData(false, false, "$gt", "$lt")]
	public void Between_ExcludedBound_UsesTheStrictOperator(bool includeFrom, bool includeTo, string fromOperator, string toOperator)
	{
		var query = QueryBuilder.Between("created_at", From, To, includeFrom, includeTo);
		
		QueryAssert.SingleField("created_at", new BsonDocument { { fromOperator, new BsonDateTime(From) }, { toOperator, new BsonDateTime(To) } }, query);
	}
	
	[Fact]
	public void Between_WithoutAField_IsCombinedOnAField()
	{
		QueryAssert.Filter("""{ "scores": { "$elemMatch": { "$gte": 80, "$lt": 85 } } }""", QueryBuilder.ElemMatch("scores", QueryBuilder.Between(80, 85, includeTo: false)));
	}
	
	#endregion
}
