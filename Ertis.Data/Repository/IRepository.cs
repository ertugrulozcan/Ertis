using Ertis.Data.Models;

namespace Ertis.Data.Repository
{
	public interface IRepository<TEntity, in TIdentifier> : IRepositoryBase<TEntity, TIdentifier> where TEntity : IEntity<TIdentifier>
	{
		
	}
}