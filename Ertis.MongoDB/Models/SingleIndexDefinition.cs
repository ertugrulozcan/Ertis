using Ertis.Core.Collections;

namespace Ertis.MongoDB.Models;

public class SingleIndexDefinition : IndexDefinitionBase
{
	#region Properties
	
	// ReSharper disable once MemberCanBePrivate.Global
	public string Field { get; }
	
	// ReSharper disable once MemberCanBePrivate.Global
	public SortDirection? Direction { get; }
	
	public override IndexType Type => IndexType.Single;
	
	public override string Key => $"{this.Field}_{(this.Direction is SortDirection.Descending ? "-1" : "1")}";
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="field"></param>
	/// <param name="direction"></param>
	public SingleIndexDefinition(string field, SortDirection? direction = null)
	{
		this.Field = field;
		this.Direction = direction;
	}
	
	#endregion
}