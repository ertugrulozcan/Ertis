using Ertis.MongoDB.Queries.Tests.TestHelpers;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// The user input written into the queries (e.g. a username of a login request) must stay a value: it can not add fields or operators to the filter
/// </summary>
public class QueryInjectionTests
{
	#region Constants
	
	private const string MEMBERSHIP_ID = "65a0f0c2e4b0a1b2c3d4e5f6";
	
	#endregion
	
	#region Methods
	
	public static TheoryData<string> MaliciousValues()
	{
		return new TheoryData<string>
		{
			$$"""nobody", "username": { "$ne": "nobody" }, "membership_id": "{{MEMBERSHIP_ID}}""",
			"""x", "$where": "sleep(100) || true""",
			"""x" } , "$or": [ { "a": 1 } ], "b": { "c": "d""",
			"o\"brien",
			@"back\slash",
			@"trailing\",
			"line\nbreak\ttab",
			"\u0001control",
			"Şule 😀 </script>"
		};
	}
	
	[Theory]
	[MemberData(nameof(MaliciousValues))]
	public void Equals_WithAnyString_KeepsTheStringAsTheValue(string value)
	{
		QueryAssert.SingleField("username", value, QueryBuilder.Equals("username", value));
	}
	
	[Theory]
	[InlineData("jane")]
	[InlineData("jane@example.com")]
	public void LoginQuery_WithAPlainUsername_MatchesOnlyThatUsername(string username)
	{
		AssertLoginQuery(username);
	}
	
	[Theory]
	[MemberData(nameof(MaliciousValues))]
	public void LoginQuery_WithAnyUsername_MatchesOnlyThatUsername(string username)
	{
		AssertLoginQuery(username);
	}
	
	private static void AssertLoginQuery(string username)
	{
		var query = QueryBuilder.Where(QueryBuilder.And(
			QueryBuilder.Equals("membership_id", MEMBERSHIP_ID),
			QueryBuilder.Or(
				QueryBuilder.Equals("username", username),
				QueryBuilder.Equals("email_address", username))));
		
		var expected = new BsonDocument("$and", new BsonArray
		{
			new BsonDocument("membership_id", MEMBERSHIP_ID),
			new BsonDocument("$or", new BsonArray
			{
				new BsonDocument("username", username),
				new BsonDocument("email_address", username)
			})
		});
		Assert.Equal(expected, QueryAssert.Parse(query));
	}
	
	[Theory]
	[MemberData(nameof(MaliciousValues))]
	public void Operators_WithAnyString_KeepTheStringAsTheValue(string value)
	{
		QueryAssert.SingleField("a", new BsonDocument("$ne", value), QueryBuilder.NotEquals("a", value));
		QueryAssert.SingleField("a", new BsonDocument("$in", new BsonArray { value }), QueryBuilder.In("a", new[] { value }));
		QueryAssert.Filter(new BsonDocument("$text", new BsonDocument("$search", value)).ToJson(), QueryBuilder.FullTextSearch(value));
	}
	
	[Theory]
	[MemberData(nameof(MaliciousValues))]
	public void Field_WithAnyName_KeepsTheNameAsTheField(string field)
	{
		QueryAssert.SingleField(field, 1, QueryBuilder.Equals(field, 1));
	}
	
	[Theory]
	[InlineData("65a0f0c2e4b0a1b2c3d4e5f6\") }, \"_id\": { \"$ne\": ObjectId(\"65a0f0c2e4b0a1b2c3d4e5f6")]
	[InlineData("not-an-object-id")]
	[InlineData("")]
	public void ObjectId_WithAnInvalidId_ThrowsFormatException(string id)
	{
		Assert.Throws<FormatException>(() => QueryBuilder.ObjectId(id));
	}
	
	[Theory]
	[InlineData("65a0f0c2e4b0a1b2c3d4e5f6")]
	[InlineData("65A0F0C2E4B0A1B2C3D4E5F6")]
	public void ObjectId_WithAValidId_IsAccepted(string id)
	{
		Assert.Equal($$"""{ "$oid": "{{id}}" }""", QueryBuilder.ObjectId(id).ToString());
	}
	
	[Theory]
	[InlineData("Jane", "\"Jane\"")]
	[InlineData("Şule 😀 </script>", "\"Şule 😀 </script>\"")]
	[InlineData("o\"brien", "\"o\\\"brien\"")]
	[InlineData(@"back\slash", "\"back\\\\slash\"")]
	[InlineData("line\nbreak", "\"line\\nbreak\"")]
	public void Equals_WritesTheStringEscapedOnlyWhenNeeded(string value, string expectedJsonString)
	{
		Assert.Equal($$"""{ "a": {{expectedJsonString}} }""", QueryBuilder.Equals("a", value).ToString());
	}
	
	/// <summary>
	/// A value of another type was written with its ToString as it is: a ToString shaped like a query added an operator
	/// </summary>
	[Fact]
	public void Equals_ValueWithAQueryShapedToString_StaysAValue()
	{
		var document = QueryAssert.Parse(QueryBuilder.Equals("a", new QueryShapedValue()));
		
		var element = Assert.Single(document.Elements);
		Assert.Equal("a", element.Name);
		Assert.False(document.Contains("$where"));
	}
	
	/// <summary>
	/// A key starting with '$' is an operator for MongoDB (e.g. "$where" runs JavaScript): a field name coming from a user
	/// (e.g. a dynamic filter) must not be able to add one
	/// </summary>
	[Theory]
	[MemberData(nameof(OperatorKeyQueries))]
	public void FieldName_StartingWithDollar_IsRejected(string name, Func<IQuery> build)
	{
		var exception = Assert.Throws<ArgumentException>(build);
		
		Assert.Contains("is not a field name", exception.Message);
		Assert.False(string.IsNullOrEmpty(name));
	}
	
	public static TheoryData<string, Func<IQuery>> OperatorKeyQueries => new()
	{
		{ "Equals", () => QueryBuilder.Equals("$where", "sleep(5000)") },
		{ "Where", () => QueryBuilder.Where("$where", "sleep(5000)") },
		{ "GreaterThan", () => QueryBuilder.GreaterThan("$expr", 1) },
		{ "In", () => QueryBuilder.In("$or", new[] { 1 }) },
		{ "Not", () => QueryBuilder.Not("$where", 1) },
		{ "Exists", () => QueryBuilder.Exists("$where", true) },
		{ "Regex", () => QueryBuilder.Regex("$where", ".*") },
		{ "ElemMatch", () => QueryBuilder.ElemMatch("$where", QueryBuilder.Equals("a", 1)) },
		{ "Combine", () => QueryBuilder.Combine("$expr", QueryBuilder.GreaterThan(1)) },
		{ "Where on a field", () => QueryBuilder.Where("$where", new[] { QueryBuilder.GreaterThan(1) }) },
		{ "Select", () => QueryBuilder.Select(new Dictionary<string, bool> { ["$where"] = true }) }
	};
	
	/// <summary>
	/// The other segments of a path may start with '$': MongoDB reads them as a path, not as an operator (e.g. the DBRef fields)
	/// </summary>
	[Theory]
	[InlineData("owner.$id")]
	[InlineData("owner.$ref")]
	[InlineData("owner.$db")]
	public void FieldPath_WithDollarInAnInnerSegment_IsAccepted(string path)
	{
		QueryAssert.SingleField(path, "x", QueryBuilder.Equals(path, "x"));
	}
	
	[Fact]
	public void Select_WithThePositionalOperator_IsAccepted()
	{
		QueryAssert.Filter("""{ "select": { "comments.$": 1 } }""", QueryBuilder.Select(new Dictionary<string, bool> { ["comments.$"] = true }));
	}
	
	private sealed class QueryShapedValue
	{
		public override string ToString() => "1, \"$where\": \"sleep(5000)\"";
	}
	
	#endregion
}
