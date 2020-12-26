namespace Ertis.Data.Repository
{
	public interface IRepositoryActionBinder
	{
		TEntity BeforeInsert<TEntity>(TEntity entity);
		
		TEntity AfterInsert<TEntity>(TEntity entity);
		
		TEntity BeforeUpdate<TEntity>(TEntity entity);
		
		TEntity AfterUpdate<TEntity>(TEntity entity);
	}
}