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
		
		Assert.Equal("""{ "name": { "$regex": { "$regularExpression": { "pattern": "^ja", "options": "" } } } }""", query.ToString());
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("^ja"), query);
	}
	
	[Fact]
	public void Regex_WithSlashDelimiters_RemovesThem()
	{
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("^ja"), QueryBuilder.Regex("name", "/^ja/"));
	}
	
	[Theory]
	[InlineData("a/", "a/")]
	[InlineData("/a/b/", "a/b")]
	[InlineData("/a", "/a")]
	[InlineData("//", "")]
	public void Regex_TrimsOnlyTheDelimitersOfASlashForm(string regex, string expectedPattern)
	{
		QueryAssert.SingleField("path", QueryAssert.RegexOperator(expectedPattern, string.Empty), QueryBuilder.Regex("path", regex));
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
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("^ja", expectedOptions), QueryBuilder.Regex("name", "^ja", options));
	}
	
	[Theory]
	[InlineData(RegexOptions.CaseInsensitivity | RegexOptions.Multiline | RegexOptions.AllowDot, "ims")]
	[InlineData(RegexOptions.CaseInsensitivity | RegexOptions.Multiline | RegexOptions.Extended | RegexOptions.AllowDot, "imsx")]
	public void Regex_WithThreeOrMoreOptions_WritesTheOptions(RegexOptions options, string expectedOptions)
	{
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("^ja", expectedOptions), QueryBuilder.Regex("name", "^ja", options));
	}
	
	[Fact]
	public void Regex_WithBackslashes_KeepsThePattern()
	{
		QueryAssert.SingleField("code", QueryAssert.RegexOperator(@"^\d+\.\w$"), QueryBuilder.Regex("code", @"^\d+\.\w$"));
	}
	
	#endregion
	
	#region Full Text Search Methods
	
	[Fact]
	public void FullTextSearch_WritesTheSearch()
	{
		var query = QueryBuilder.FullTextSearch("jane doe");
		
		Assert.Equal("""{ "$text": { "$search": "jane doe" } }""", query.ToString());
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
	
	[Fact]
	public void TextSearchLanguage_All_ContainsEveryLanguageOnce()
	{
		var languages = TextSearchLanguage.All;
		
		Assert.Equal(16, languages.Count);
		Assert.Equal(languages.Count, languages.Select(x => x.ISO6391Code).Distinct().Count());
		Assert.Contains(languages, x => x is { Name: "Turkish", ISO6391Code: "tr" });
		Assert.Contains(languages, x => x is { Name: "None", ISO6391Code: "none" });
	}
	
	/// <summary>
	/// The repositories build the search from the TextSearchOptions (e.g. MongoRepositoryBase.Search)
	/// </summary>
	[Fact]
	public void FullTextSearch_WithTheCodeOfTextSearchOptions_WritesTheLanguage()
	{
		var options = new TextSearchOptions { Language = TextSearchLanguage.Turkish, IsCaseSensitive = true };
		
		var query = QueryBuilder.FullTextSearch("kitap", options.Language.ISO6391Code, options.IsCaseSensitive, options.IsDiacriticSensitive);
		
		QueryAssert.Filter("""{ "$text": { "$search": "kitap", "$language": "tr", "$caseSensitive": true } }""", query);
	}
	
	[Fact]
	public void FullTextSearch_WithTextSearchOptionsWithoutALanguage_OmitsTheLanguage()
	{
		var options = new TextSearchOptions();
		
		var query = QueryBuilder.FullTextSearch("kitap", options.Language.ISO6391Code, options.IsCaseSensitive, options.IsDiacriticSensitive);
		
		QueryAssert.Filter("""{ "$text": { "$search": "kitap" } }""", query);
	}
	
	#endregion
}
