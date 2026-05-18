namespace Ertis.MongoDB.Models;

public interface IIndexDefinition
{
	IndexType Type { get; }
	
	string Key { get; }
}