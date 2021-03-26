using Ertis.Data.Models;
using Ertis.Data.Repository;

namespace Ertis.PostgreSQL.Repository
{
	public interface IPostgreRepository<TEntity> : IRepository<TEntity, int> where TEntity : IEntity<int>
	{
		bool TrackingEnabled { get; set; }
	}
}