// ReSharper disable UnusedMember.Global
namespace Ertis.Data.Repository;

// ReSharper disable once UnusedType.Global
public interface IRepositoryActionBinder
{
	TEntity BeforeInsert<TEntity>(TEntity entity);
	
	TEntity AfterInsert<TEntity>(TEntity entity);
	
	TEntity BeforeUpdate<TEntity>(TEntity entity);
	
	TEntity AfterUpdate<TEntity>(TEntity entity);
}