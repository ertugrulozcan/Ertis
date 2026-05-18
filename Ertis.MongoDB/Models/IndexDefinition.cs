namespace Ertis.MongoDB.Models;

public abstract class IndexDefinitionBase : IIndexDefinition
{
	#region Properties
	
	public abstract IndexType Type { get; }
	
	public abstract string Key { get; }
	
	#endregion
	
	#region Methods
	
	public override string ToString()
	{
		return this.Key;
	}
	
	#endregion
}