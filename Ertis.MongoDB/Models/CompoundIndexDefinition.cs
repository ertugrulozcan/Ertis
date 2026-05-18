using System.Collections.ObjectModel;
using Ertis.Core.Collections;

namespace Ertis.MongoDB.Models;

public class CompoundIndexDefinition : IndexDefinitionBase
{
	#region Properties
	
	// ReSharper disable once MemberCanBePrivate.Global
	public ReadOnlyCollection<SingleIndexDefinition> Indexes { get; }
	
	public override IndexType Type => IndexType.Compound;
	
	public override string Key => string.Join('_', this.Indexes.Select(index => $"{index.Field}_{(index.Direction is SortDirection.Descending ? "-1" : "1")}"));
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="indexes"></param>
	public CompoundIndexDefinition(IEnumerable<SingleIndexDefinition> indexes)
	{
		this.Indexes = new ReadOnlyCollection<SingleIndexDefinition>(indexes.ToList());
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="indexes"></param>
	public CompoundIndexDefinition(params SingleIndexDefinition[] indexes)
	{
		if (!indexes.Any())
		{
			throw new ArgumentException("Compound indexes must be included at least one single index");
		}
		
		this.Indexes = new ReadOnlyCollection<SingleIndexDefinition>(indexes.ToList());
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="fieldNames"></param>
	public CompoundIndexDefinition(params string[] fieldNames)
	{
		if (!fieldNames.Any())
		{
			throw new ArgumentException("Compound indexes must be included at least one single index");
		}
		
		this.Indexes = new ReadOnlyCollection<SingleIndexDefinition>(fieldNames.Select(x => new SingleIndexDefinition(x)).ToList());
	}
	
	#endregion
}