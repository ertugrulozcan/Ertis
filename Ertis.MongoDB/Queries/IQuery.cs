namespace Ertis.MongoDB.Queries
{
	public interface IQuery : IQueryable
	{
		string Key { get; }
		
		IQueryable Value { get; }
	}
}