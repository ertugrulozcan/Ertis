using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ertis.MongoDB.Helpers;

/// <summary>
/// Reads a strict json query to a BsonValue, keeping the query operators of the documents as they are.
/// The MongoDB JsonReader reads a document which starts with "$regex" as a legacy extended json regular expression value,
/// so it fails with a query like { "name": { "$regex": "^a", "$ne": "abc" } } (it expects "$options" after "$regex"),
/// and it reads { "$regex": "^a" } as a regular expression value. This reader keeps them as the $regex operator documents
/// (MongoDB runs the operator and the regular expression value as the same filter).
/// The extended json values ({ "$oid": ... }, { "$date": ... }, { "$numberLong": ... } etc.) and the numbers are read by
/// the MongoDB JsonReader, so their types are the same as before.
/// </summary>
public static class QueryJsonReader
{
	#region Constants

	/// <summary>
	/// The first keys of the extended json values (canonical, relaxed and the legacy ones except "$regex", which is a query operator too).
	/// "$type" is not one of them: it is a query operator too and the legacy binary value starts with "$binary".
	/// </summary>
	private static readonly HashSet<string> ExtendedJsonKeys =
	[
		"$oid",
		"$date",
		"$numberInt",
		"$numberLong",
		"$numberDouble",
		"$numberDecimal",
		"$binary",
		"$uuid",
		"$regularExpression",
		"$timestamp",
		"$minKey",
		"$maxKey",
		"$symbol",
		"$code",
		"$dbPointer",
		"$undefined"
	];

	#endregion

	#region Methods

	/// <summary>
	/// Reads the strict json (a document, an array or a value); false if it is not a strict json (e.g. the shell syntax) or a value is invalid
	/// </summary>
	public static bool TryRead(string json, [NotNullWhen(true)] out BsonValue? value)
	{
		try
		{
			using var document = JsonDocument.Parse(json);
			value = ReadValue(document.RootElement);
			return true;
		}
		catch (Exception ex) when (ex is JsonException or FormatException or BsonException or EndOfStreamException or InvalidOperationException)
		{
			value = null;
			return false;
		}
	}

	private static BsonValue ReadValue(JsonElement element)
	{
		return element.ValueKind switch
		{
			JsonValueKind.Object => ReadObject(element),
			JsonValueKind.Array => new BsonArray(element.EnumerateArray().Select(ReadValue)),
			JsonValueKind.String => new BsonString(element.GetString()!),
			JsonValueKind.True => BsonBoolean.True,
			JsonValueKind.False => BsonBoolean.False,
			JsonValueKind.Null => BsonNull.Value,
			// The numbers (Int32, Int64 or Double like the MongoDB JsonReader)
			_ => ReadWithMongoJsonReader(element.GetRawText())
		};
	}

	private static BsonValue ReadObject(JsonElement element)
	{
		using var properties = element.EnumerateObject();
		if (properties.MoveNext() && ExtendedJsonKeys.Contains(properties.Current.Name))
		{
			return ReadWithMongoJsonReader(element.GetRawText());
		}

		// Add throws InvalidOperationException for a duplicate name, so the query is read by the MongoDB JsonReader as before
		var document = new BsonDocument();
		foreach (var property in element.EnumerateObject())
		{
			document.Add(property.Name, ReadValue(property.Value));
		}

		return document;
	}

	private static BsonValue ReadWithMongoJsonReader(string json)
	{
		using var reader = new JsonReader(json);
		return BsonValueSerializer.Instance.Deserialize(BsonDeserializationContext.CreateRoot(reader));
	}

	#endregion
}
