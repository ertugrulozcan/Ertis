using Ertis.Core.Collections;

namespace Ertis.MongoDB.Models;

public class TTLIndexDefinition : SingleIndexDefinition
{
	#region Properties
	
	public override IndexType Type => IndexType.TTL;
	
	// ReSharper disable once UnusedAutoPropertyAccessor.Global
	public TimeSpan ExpireAfter { get; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="field"></param>
	/// <param name="direction"></param>
	/// <param name="ttl"></param>
	public TTLIndexDefinition(string field, SortDirection direction, TimeSpan ttl) : base(field, direction)
	{
		this.ExpireAfter = ttl;
	}
	
	#endregion
}