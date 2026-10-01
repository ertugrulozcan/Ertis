using Ertis.Extensions.AspNetCore.Exceptions;
using Ertis.Extensions.AspNetCore.Helpers;
using Ertis.Extensions.AspNetCore.Versioning;

namespace Ertis.Extensions.AspNetCore.Tests.Helpers;

public class QueryHelperTests
{
	#region Query Methods
	
	[Fact]
	public void ExtractWhereQuery_WithNull_ReturnsNull()
	{
		Assert.Null(QueryHelper.ExtractWhereQuery(null!));
	}
	
	[Theory]
	[InlineData("[1, 2]")]
	[InlineData("5")]
	[InlineData("""{ "select": {} }""")]
	public void ExtractWhereQuery_WithoutAWhereNode_ReturnsNull(string body)
	{
		Assert.Null(QueryHelper.ExtractWhereQuery(body));
	}
	
	[Fact]
	public void ExtractProjection_WithNull_ReturnsAnEmptyDictionary()
	{
		Assert.Empty(QueryHelper.ExtractProjection(null!));
	}
	
	[Theory]
	[InlineData("""{ "select": [ "name" ] }""")]
	[InlineData("""{ "where": {} }""")]
	[InlineData("[1]")]
	public void ExtractProjection_WithoutASelectObject_ReturnsAnEmptyDictionary(string body)
	{
		Assert.Empty(QueryHelper.ExtractProjection(body));
	}
	
	[Theory]
	[InlineData("1", true)]
	[InlineData("0", false)]
	[InlineData("5", true)]
	[InlineData("-1", true)]
	[InlineData("1.0", true)]
	[InlineData("2.5", true)]
	[InlineData("0.0", false)]
	[InlineData("true", true)]
	[InlineData("false", false)]
	[InlineData("\"true\"", true)]
	[InlineData("\"True\"", true)]
	[InlineData("\"false\"", false)]
	[InlineData("\"1\"", true)]
	[InlineData("\"0\"", false)]
	[InlineData("\"5\"", false)]
	public void ExtractProjection_ReadsTheSelection(string jsonValue, bool expected)
	{
		var projection = QueryHelper.ExtractProjection($$"""{ "select": { "field": {{jsonValue}} } }""");
		
		Assert.Equal(expected, Assert.Contains("field", projection));
	}
	
	[Theory]
	[InlineData("\"x\"")]
	[InlineData("null")]
	[InlineData("{}")]
	[InlineData("[]")]
	public void ExtractProjection_WithAnUnsupportedValue_SkipsTheField(string jsonValue)
	{
		var projection = QueryHelper.ExtractProjection($$"""{ "select": { "field": {{jsonValue}}, "other": 1 } }""");
		
		Assert.Equal(["other"], projection.Keys);
	}
	
	[Theory]
	[InlineData("""{ "where": { "a": 1 } /* comment */ }""")]
	[InlineData("""{ "where": { "a": 1, }, }""")]
	public void ExtractWhereQuery_AllowsCommentsAndTrailingCommas(string body)
	{
		var where = QueryHelper.ExtractWhereQuery(body);
		
		Assert.Equal(1, System.Text.Json.Nodes.JsonNode.Parse(where!, documentOptions: new System.Text.Json.JsonDocumentOptions { AllowTrailingCommas = true })!["a"]!.GetValue<int>());
	}
	
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void ExtractWhereQuery_WithAnEmptyBody_ReturnsNull(string body)
	{
		Assert.Null(QueryHelper.ExtractWhereQuery(body));
		Assert.Empty(QueryHelper.ExtractProjection(body));
	}
	
	[Fact]
	public void ExtractWhereQuery_WithDateStrings_KeepsTheInstant()
	{
		var where = QueryHelper.ExtractWhereQuery("""{ "where": { "t": { "$gt": "2026-01-31T10:00:00+03:00" } } }""");
		
		var value = System.Text.Json.Nodes.JsonNode.Parse(where!)!["t"]!["$gt"]!.GetValue<string>();
		Assert.Equal(DateTimeOffset.Parse("2026-01-31T07:00:00Z"), DateTimeOffset.Parse(value));
	}
	
	[Fact]
	public void ExtractWhereQuery_ReturnsTheRawJsonOfTheWhereNode()
	{
		var where = QueryHelper.ExtractWhereQuery("""{ "where": { "name":"Jane",  "age": { "$gt": 18 } } }""");
		
		Assert.Equal("""{ "name":"Jane",  "age": { "$gt": 18 } }""", where);
	}
	
	/// <summary>
	/// The body is parsed as standard json: the relaxed syntax of the former parser (unquoted names, single quotes) is rejected
	/// </summary>
	[Theory]
	[InlineData("""{ where: { age: { $gt: 18 } } }""")]
	[InlineData("""{ 'where': { 'a': 'b' } }""")]
	[InlineData("""{ "where": { "a": 1 } } extra""")]
	[InlineData("{ invalid")]
	public void ExtractWhereQuery_WithANonStandardJson_ThrowsJsonException(string body)
	{
		Assert.ThrowsAny<System.Text.Json.JsonException>(() => QueryHelper.ExtractWhereQuery(body));
	}
	
	[Fact]
	public void ExtractProjection_WithARepeatedField_KeepsTheLastValue()
	{
		var projection = QueryHelper.ExtractProjection("""{ "select": { "name": 1, "name": 0 } }""");
		
		Assert.False(projection["name"]);
	}
	
	#endregion
	
	#region Other Methods
	
	[Theory]
	[InlineData(1, 0, "1")]
	[InlineData(2, 3, "2.3")]
	public void ApiVersionOptions_ToString_WritesTheVersion(int major, int minor, string expected)
	{
		Assert.Equal(expected, new ApiVersionOptions { Major = major, Minor = minor }.ToString());
	}
	
	[Fact]
	public void Exceptions_HaveTheirErrorCodes()
	{
		Assert.Equal("NegativeSkipError", new NegativeSkipException().ErrorCode);
		Assert.Equal("NegativeLimitError", new NegativeLimitException().ErrorCode);
		Assert.Equal("QueryBodyNullError", new QueryBodyNullException().ErrorCode);
		Assert.Equal(400, new QueryBodyNullException(new Exception()).Error.StatusCode);
		Assert.Equal(400, new NegativeSkipException(new Exception()).Error.StatusCode);
		Assert.Equal(400, new NegativeLimitException(new Exception()).Error.StatusCode);
		Assert.Equal("InvalidQuery", new InvalidQueryException().ErrorCode);
		Assert.Equal("The request body is not a valid JSON document", new InvalidQueryException(new Exception()).Error.Message);
	}
	
	#endregion
}
