using Ertis.Data.Models;
using Ertis.Data.Repository;

namespace Ertis.PostgreSQL.Repository;

public interface IPostgreRepository<TEntity> : IRepository<TEntity, int> where TEntity : IEntity<int>
{
	// ReSharper disable once UnusedMemberInSuper.Global
	bool TrackingEnabled { get; set; }
}