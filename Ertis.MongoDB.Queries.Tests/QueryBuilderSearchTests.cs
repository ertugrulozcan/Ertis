using Ertis.MongoDB.Queries.Tests.TestHelpers;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// The regular expression ($regex) and the full text search ($text) queries
/// </summary>
public class QueryBuilderSearchTests
{
	#region Regex Methods
	
	[Fact]
	public void Regex_WritesThePattern()
	{
		var query = QueryBuilder.Regex("name", "^ja");
		
		Assert.Equal("""{ "name": { $regex: "^ja" } }""", query.ToString());
		QueryAssert.SingleField("name", new BsonRegularExpression("^ja"), query);
	}
	
	[Fact]
	public void Regex_WithSlashDelimiters_RemovesThem()
	{
		QueryAssert.SingleField("name", new BsonRegularExpression("^ja"), QueryBuilder.Regex("name", "/^ja/"));
	}
	
	[Theory]
	[InlineData("a/", "a/")]
	[InlineData("/a/b/", "a/b")]
	[InlineData("/a", "/a")]
	[InlineData("//", "")]
	public void Regex_TrimsOnlyTheDelimitersOfASlashForm(string regex, string expectedPattern)
	{
		QueryAssert.SingleField("path", new BsonRegularExpression(expectedPattern, string.Empty), QueryBuilder.Regex("path", regex));
	}
	
	[Theory]
	[InlineData(RegexOptions.CaseInsensitivity, "i")]
	[InlineData(RegexOptions.Multiline, "m")]
	[InlineData(RegexOptions.Extended, "x")]
	[InlineData(RegexOptions.AllowDot, "s")]
	[InlineData(RegexOptions.CaseInsensitivity | RegexOptions.Multiline, "im")]
	[InlineData(RegexOptions.CaseInsensitivity | RegexOptions.Extended, "ix")]
	[InlineData(RegexOptions.CaseInsensitivity | RegexOptions.AllowDot, "is")]
	[InlineData(RegexOptions.Multiline | RegexOptions.Extended, "mx")]
	[InlineData(RegexOptions.Multiline | RegexOptions.AllowDot, "ms")]
	[InlineData(RegexOptions.Extended | RegexOptions.AllowDot, "sx")]
	public void Regex_WithOptions_WritesTheOptions(RegexOptions options, string expectedOptions)
	{
		QueryAssert.SingleField("name", new BsonRegularExpression("^ja", expectedOptions), QueryBuilder.Regex("name", "^ja", options));
	}
	
	[Theory]
	[InlineData(RegexOptions.CaseInsensitivity | RegexOptions.Multiline | RegexOptions.AllowDot, "ims")]
	[InlineData(RegexOptions.CaseInsensitivity | RegexOptions.Multiline | RegexOptions.Extended | RegexOptions.AllowDot, "imsx")]
	public void Regex_WithThreeOrMoreOptions_WritesTheOptions(RegexOptions options, string expectedOptions)
	{
		QueryAssert.SingleField("name", new BsonRegularExpression("^ja", expectedOptions), QueryBuilder.Regex("name", "^ja", options));
	}
	
	[Fact]
	public void Regex_WithBackslashes_KeepsThePattern()
	{
		QueryAssert.SingleField("code", new BsonRegularExpression(@"^\d+\.\w$"), QueryBuilder.Regex("code", @"^\d+\.\w$"));
	}
	
	#endregion
	
	#region Full Text Search Methods
	
	[Fact]
	public void FullTextSearch_WritesTheSearch()
	{
		var query = QueryBuilder.FullTextSearch("jane doe");
		
		Assert.Equal("""{ $text: { $search: "jane doe" } }""", query.ToString());
		QueryAssert.Filter("""{ "$text": { "$search": "jane doe" } }""", query);
	}
	
	[Fact]
	public void FullTextSearch_WithOptions_WritesTheOptions()
	{
		var query = QueryBuilder.FullTextSearch("jane", "tr", isCaseSensitive: true, isDiacriticSensitive: true);
		
		QueryAssert.Filter("""{ "$text": { "$search": "jane", "$language": "tr", "$caseSensitive": true, "$diacriticSensitive": true } }""", query);
	}
	
	[Theory]
	[InlineData("none")]
	[InlineData("")]
	public void FullTextSearch_WithoutALanguage_OmitsTheLanguage(string language)
	{
		QueryAssert.Filter("""{ "$text": { "$search": "jane" } }""", QueryBuilder.FullTextSearch("jane", language));
	}
	
	#endregion
}
