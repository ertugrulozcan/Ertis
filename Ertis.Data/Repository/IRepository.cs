using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;

namespace Ertis.Data.Repository
{
	public interface IRepository<TEntity, in TIdentifier> where TEntity : IEntity<TIdentifier>
	{
		#region Find & Query Methods

		TEntity FindOne(TIdentifier id);
		
		Task<TEntity> FindOneAsync(TIdentifier id);
		
		TEntity FindOne(Expression<Func<TEntity, bool>> expression);
		
		Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression);

		IPaginationCollection<TEntity> Find(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		Task<IPaginationCollection<TEntity>> FindAsync(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		IPaginationCollection<TEntity> Find(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		Task<IPaginationCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);

		IPaginationCollection<TEntity> Find(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);

		Task<IPaginationCollection<TEntity>> FindAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		#endregion
		
		#region Insert Methods

		TEntity Insert(TEntity entity);
		
		Task<TEntity> InsertAsync(TEntity entity);

		#endregion
		
		#region Update Methods

		TEntity Update(TEntity entity);
		
		Task<TEntity> UpdateAsync(TEntity entity);

		TEntity Upsert(TEntity entity);
		
		Task<TEntity> UpsertAsync(TEntity entity);

		#endregion
		
		#region Delete Methods

		bool Delete(TIdentifier id);
		
		Task<bool> DeleteAsync(TIdentifier id);
		
		bool BulkDelete(TEntity[] entities);
		
		Task<bool> BulkDeleteAsync(TEntity[] entities);

		#endregion
		
		#region Count Methods

		long Count();
		
		Task<long> CountAsync();
		
		long Count(string query);
		
		Task<long> CountAsync(string query);

		long Count(Expression<Func<TEntity, bool>> expression);

		Task<long> CountAsync(Expression<Func<TEntity, bool>> expression);

		#endregion
	}
}