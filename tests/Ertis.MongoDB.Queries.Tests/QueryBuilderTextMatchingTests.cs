using Ertis.MongoDB.Queries.Tests.TestHelpers;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests;

/// <summary>
/// StartsWith, EndsWith and ContainsText match a text as it is (e.g. a user input): its regular expression characters are escaped
/// </summary>
public class QueryBuilderTextMatchingTests
{
	#region Methods
	
	[Fact]
	public void StartsWithEndsWithAndContainsText_WriteTheAnchoredPatterns()
	{
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("^ja"), QueryBuilder.StartsWith("name", "ja"));
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("ne$"), QueryBuilder.EndsWith("name", "ne"));
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("an"), QueryBuilder.ContainsText("name", "an"));
	}
	
	[Fact]
	public void IgnoreCase_AddsTheCaseInsensitivityOption()
	{
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("^ja", "i"), QueryBuilder.StartsWith("name", "ja", ignoreCase: true));
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("ne$", "i"), QueryBuilder.EndsWith("name", "ne", ignoreCase: true));
		QueryAssert.SingleField("name", QueryAssert.RegexOperator("an", "i"), QueryBuilder.ContainsText("name", "an", ignoreCase: true));
	}
	
	[Fact]
	public void ContainsText_WithRegularExpressionCharacters_MatchesThemAsThey()
	{
		// Unescaped, "a.b+c@x.com" would match "aXbbbc@xYcom" and "(a+)+$" would be a slow pattern
		QueryAssert.SingleField("email", QueryAssert.RegexOperator(@"a\.b\+c@x\.com"), QueryBuilder.ContainsText("email", "a.b+c@x.com"));
		QueryAssert.SingleField("name", QueryAssert.RegexOperator(@"\(a\+\)\+\$"), QueryBuilder.ContainsText("name", "(a+)+$"));
	}
	
	[Fact]
	public void ContainsText_InSlashes_IsNotReadAsADelimitedPattern()
	{
		QueryAssert.SingleField("path", QueryAssert.RegexOperator(@"\/home\/"), QueryBuilder.ContainsText("path", "/home/"));
	}
	
	[Fact]
	public void EscapeRegex_EscapesEveryRegularExpressionCharacter()
	{
		Assert.Equal(@"\\\^\$\.\|\?\*\+\(\)\[\]\{\}\/ plain-text_1", QueryBuilder.EscapeRegex(@"\^$.|?*+()[]{}/ plain-text_1"));
	}
	
	[Fact]
	public void StartsWith_WithoutAField_MatchesTheItemsOfAnArray()
	{
		var query = QueryBuilder.ElemMatch("tags", QueryBuilder.StartsWith("a", ignoreCase: true));
		
		QueryAssert.SingleField("tags", new BsonDocument("$elemMatch", QueryAssert.RegexOperator("^a", "i")), query);
	}
	
	/// <summary>
	/// The legacy { "$regex": ..., "$options": ... } pair could not be read next to another operator of the field (the json reader threw)
	/// </summary>
	[Fact]
	public void TextMatching_WithAnotherOperatorOfTheField_IsMerged()
	{
		var query = QueryBuilder.Where(QueryBuilder.StartsWith("name", "j", ignoreCase: true), QueryBuilder.NotEquals("name", "John"));
		
		var expected = QueryAssert.RegexOperator("^j", "i").Add("$ne", "John");
		QueryAssert.SingleField("name", expected, query);
	}
	
	#endregion
}
