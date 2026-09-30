using MongoDB.Bson;

namespace Ertis.MongoDB.Queries.Tests.TestHelpers;

public static class QueryAssert
{
	#region Methods
	
	/// <summary>
	/// Parses the query with the MongoDB json parser (the consumers pass the query string to a JsonFilterDefinition)
	/// </summary>
	public static BsonDocument Parse(IQuery query)
	{
		return BsonDocument.Parse(query.ToString());
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
	
	#endregion
}
