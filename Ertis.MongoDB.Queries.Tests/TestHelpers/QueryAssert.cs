using System.Text.Json;
using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests.TestHelpers;

public static class QueryAssert
{
	#region Methods
	
	/// <summary>
	/// Parses the query with the MongoDB json parser (the consumers pass the query string to a JsonFilterDefinition).
	/// The query must be a strict json too (e.g. an API reading the body with System.Text.Json).
	/// </summary>
	public static BsonDocument Parse(IQuery query)
	{
		var json = query.ToString();
		using (JsonDocument.Parse(json))
		{
		}
		
		return BsonDocument.Parse(json);
	}
	
	/// <summary>
	/// Asserts the MongoDB filter of the query (the parsed document, not the text)
	/// </summary>
	public static void Filter(string expectedFilter, IQuery query)
	{
		Assert.Equal(BsonDocument.Parse(expectedFilter), Parse(query));
	}
	
	/// <summary>
	/// Asserts the filter of a single field: the query has only this field, with this value
	/// </summary>
	public static void SingleField(string expectedField, BsonValue expectedValue, IQuery query)
	{
		var document = Parse(query);
		
		var element = Assert.Single(document.Elements);
		Assert.Equal(expectedField, element.Name);
		Assert.Equal(expectedValue, element.Value);
	}
	
	/// <summary>
	/// The $regex operator of a field with the regular expression value: { "$regex": /pattern/options }
	/// </summary>
	public static BsonDocument RegexOperator(string pattern, string options = "")
	{
		return new BsonDocument("$regex", new BsonRegularExpression(pattern, options));
	}
	
	#endregion
}
