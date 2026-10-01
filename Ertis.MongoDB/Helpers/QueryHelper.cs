using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Ertis.MongoDB.Helpers;

/// <summary>
/// Converts the string values of the json queries: the ids (the values of the _id fields) to ObjectIds and the ISO 8601 dates to dates.
/// The queries are read with the MongoDB json reader, so the shell syntax (unquoted names, ObjectId(...), ISODate(...)) and the extended json are supported.
/// </summary>
public static class QueryHelper
{
	#region Fields
	
	private static readonly JsonWriterSettings ShellWriterSettings = new() { OutputMode = JsonOutputMode.Shell };
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Returns the query with the converted values in the shell syntax; an empty or invalid query is returned as it is
	/// </summary>
	public static string EnsureObjectIdsAndISODates(string json)
	{
		return Ensure(json, convertObjectIds: true, convertDates: true);
	}
	
	internal static string Ensure(string json, bool convertObjectIds, bool convertDates)
	{
		if (string.IsNullOrEmpty(json))
		{
			return json;
		}
		
		var value = Parse(json);
		if (value == null)
		{
			return json;
		}
		
		return Convert(value, null, convertObjectIds, convertDates).ToJson(ShellWriterSettings);
	}
	
	/// <summary>
	/// The filter of the json query with the converted values; an empty query matches every document.
	/// An invalid query is given to the driver as it is, which throws its parsing error when the filter is rendered.
	/// </summary>
	internal static FilterDefinition<T> CreateFilterDefinition<T>(string? query)
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			return FilterDefinition<T>.Empty;
		}
		
		if (Parse(query) is BsonDocument document)
		{
			return new BsonDocumentFilterDefinition<T>((BsonDocument) Convert(document, null, convertObjectIds: true, convertDates: true));
		}
		
		return new JsonFilterDefinition<T>(query);
	}
	
	/// <summary>
	/// The stages of the json aggregation pipeline with the converted values
	/// </summary>
	internal static PipelineDefinition<T, BsonDocument> CreatePipelineDefinition<T>(string pipeline)
	{
		var stages = BsonSerializer.Deserialize<BsonArray>(pipeline);
		return PipelineDefinition<T, BsonDocument>.Create(stages.Select(x => (BsonDocument) Convert(x.AsBsonDocument, null, convertObjectIds: true, convertDates: true)));
	}
	
	private static BsonValue? Parse(string json)
	{
		try
		{
			using var reader = new JsonReader(json);
			var value = BsonValueSerializer.Instance.Deserialize(BsonDeserializationContext.CreateRoot(reader));
			return reader.IsAtEndOfFile() ? value : null;
		}
		catch (Exception ex) when (ex is FormatException or BsonException or EndOfStreamException)
		{
			return null;
		}
	}
	
	/// <param name="value"></param>
	/// <param name="fieldName">The nearest field name of the value (the operators are skipped)</param>
	/// <param name="convertObjectIds"></param>
	/// <param name="convertDates"></param>
	private static BsonValue Convert(BsonValue value, string? fieldName, bool convertObjectIds, bool convertDates)
	{
		switch (value)
		{
			case BsonDocument document:
			{
				foreach (var element in document.Elements.ToArray())
				{
					var elementFieldName = element.Name.StartsWith('$') ? fieldName : element.Name;
					document[element.Name] = Convert(element.Value, elementFieldName, convertObjectIds, convertDates);
				}
				
				return document;
			}
			case BsonArray array:
			{
				for (var i = 0; i < array.Count; i++)
				{
					array[i] = Convert(array[i], fieldName, convertObjectIds, convertDates);
				}
				
				return array;
			}
			case BsonString bsonString:
			{
				var text = bsonString.Value;
				if (convertObjectIds && IsIdField(fieldName) && ObjectId.TryParse(text, out var objectId))
				{
					return objectId;
				}
				
				if (convertDates && ISODateHelper.TryParseDateTime(text, out var dateTime))
				{
					return new BsonDateTime(dateTime);
				}
				
				return value;
			}
			default:
				return value;
		}
	}
	
	private static bool IsIdField(string? fieldName)
	{
		return fieldName != null && (fieldName == "_id" || fieldName.EndsWith("._id", StringComparison.Ordinal));
	}
	
	#endregion
}
