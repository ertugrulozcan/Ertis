using Ertis.Core.Collections;
using MongoDB.Bson;

namespace Ertis.MongoDB.Models;

public class IndexOptions
{
	#region Properties
	
	// ReSharper disable once UnusedAutoPropertyAccessor.Global
	public IEnumerable<SingleIndexDefinition>? Hint { get; set; }
	
	#endregion
	
	#region Methods
	
	// ReSharper disable once UnusedMember.Global
	internal BsonValue? GetIndexHint()
	{
		BsonValue? hint = null;
		if (this.Hint != null && this.Hint.Any())
		{
			var hintDocument = new BsonDocument();
			foreach (var indexDefinition in this.Hint)
			{
				hintDocument.Add(indexDefinition.Field, indexDefinition.Direction == SortDirection.Descending ? -1 : 1);
			}
			
			hint = hintDocument;
		}
		
		return hint;
	}
	
	#endregion
}