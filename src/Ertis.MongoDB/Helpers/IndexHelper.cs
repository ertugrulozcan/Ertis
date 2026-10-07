using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using SortDirection = Ertis.Core.Collections.SortDirection;

namespace Ertis.MongoDB.Helpers;

/// <summary>
/// Converts the index definitions to the index models of the driver, and the listed indexes to the index definitions
/// </summary>
internal static class IndexHelper
{
	#region Read Methods
	
	public static IEnumerable<IIndexDefinition> ToIndexDefinitions(IEnumerable<BsonDocument> indexes)
	{
		foreach (var index in indexes)
		{
			if (!index.TryGetValue("key", out var key) || !key.IsBsonDocument || key.AsBsonDocument.ElementCount == 0)
			{
				continue;
			}
			
			var isUnique = index.TryGetValue("unique", out var unique) && unique.ToBoolean();
			var elements = key.AsBsonDocument.Elements.ToArray();
			if (elements.Any(x => x.Name == "_fts"))
			{
				yield return ToTextIndexDefinition(index, isUnique);
			}
			else if (elements.Length == 1)
			{
				var direction = GetDirection(elements[0].Value);
				if (index.TryGetValue("expireAfterSeconds", out var expireAfterSeconds) && expireAfterSeconds.IsNumeric)
				{
					yield return new TTLIndexDefinition(elements[0].Name, direction ?? SortDirection.Ascending, TimeSpan.FromSeconds(expireAfterSeconds.ToDouble())) { IsUnique = isUnique };
				}
				else
				{
					yield return new SingleIndexDefinition(elements[0].Name, direction) { IsUnique = isUnique };
				}
			}
			else
			{
				yield return new CompoundIndexDefinition(elements.Select(x => new SingleIndexDefinition(x.Name, GetDirection(x.Value))).ToArray()) { IsUnique = isUnique };
			}
		}
	}
	
	/// <summary>
	/// The fields of a text index are read from its weights (its name may be any name)
	/// </summary>
	private static TextIndexDefinition ToTextIndexDefinition(BsonDocument index, bool isUnique)
	{
		var weightedFields = new Dictionary<string, int>();
		if (index.TryGetValue("weights", out var weights) && weights.IsBsonDocument)
		{
			foreach (var element in weights.AsBsonDocument)
			{
				weightedFields[element.Name] = element.Value.IsNumeric ? element.Value.ToInt32() : 1;
			}
		}
		
		var locale = IndexLocale.none;
		if (index.TryGetValue("default_language", out var defaultLanguage) && defaultLanguage.IsString && Enum.TryParse<IndexLocale>(defaultLanguage.AsString, out var indexLocale))
		{
			locale = indexLocale;
		}
		
		return new TextIndexDefinition(weightedFields, locale) { IsUnique = isUnique };
	}
	
	private static SortDirection? GetDirection(BsonValue value)
	{
		if (!value.IsNumeric)
		{
			// e.g. hashed, 2dsphere
			return null;
		}
		
		return value.ToDouble() < 0 ? SortDirection.Descending : SortDirection.Ascending;
	}
	
	#endregion
	
	#region Create Methods
	
	public static CreateIndexModel<T> ToIndexModel<T>(IIndexDefinition indexDefinition)
	{
		return indexDefinition switch
		{
			TTLIndexDefinition ttlIndex when indexDefinition.Type == IndexType.TTL => new CreateIndexModel<T>(GetKeys<T>(ttlIndex), new CreateIndexOptions { ExpireAfter = ttlIndex.ExpireAfter, Unique = ttlIndex.IsUnique }),
			SingleIndexDefinition singleIndex when indexDefinition.Type == IndexType.Single => new CreateIndexModel<T>(GetKeys<T>(singleIndex), new CreateIndexOptions { Unique = singleIndex.IsUnique }),
			CompoundIndexDefinition compoundIndex when indexDefinition.Type == IndexType.Compound => new CreateIndexModel<T>(Builders<T>.IndexKeys.Combine(compoundIndex.Indexes.Select(GetKeys<T>)), new CreateIndexOptions { Unique = compoundIndex.IsUnique }),
			TextIndexDefinition textIndex when indexDefinition.Type == IndexType.Text => ToTextIndexModel<T>(textIndex),
			_ => throw new ArgumentOutOfRangeException(nameof(indexDefinition), indexDefinition.Type, "The index type is not supported")
		};
	}
	
	public static IndexKeysDefinition<T> GetKeys<T>(string fieldName, SortDirection? direction)
	{
		return direction is SortDirection.Descending ? Builders<T>.IndexKeys.Descending(fieldName) : Builders<T>.IndexKeys.Ascending(fieldName);
	}
	
	private static IndexKeysDefinition<T> GetKeys<T>(SingleIndexDefinition indexDefinition)
	{
		return GetKeys<T>(indexDefinition.Field, indexDefinition.Direction);
	}
	
	private static CreateIndexModel<T> ToTextIndexModel<T>(TextIndexDefinition indexDefinition)
	{
		if (indexDefinition.WeightedFields == null || indexDefinition.WeightedFields.Count == 0)
		{
			throw new IndexException("No fields defined");
		}
		
		var options = new CreateIndexOptions
		{
			Name = indexDefinition.Key,
			DefaultLanguage = indexDefinition.Locale.ToString(),
			Unique = indexDefinition.IsUnique
		};
		
		var isWeighted = indexDefinition.WeightedFields.Count > 1 && indexDefinition.WeightedFields.Any(x => x.Value > 1);
		if (isWeighted)
		{
			options.Weights = new BsonDocument(indexDefinition.WeightedFields);
		}
		
		var keys = Builders<T>.IndexKeys.Combine(indexDefinition.Fields.Select(x => Builders<T>.IndexKeys.Text(x)));
		return new CreateIndexModel<T>(keys, options);
	}
	
	#endregion
}
