using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ertis.Core.Collections;

namespace Ertis.Data.Repository
{
    public interface IRepositoryBase<TEntity, in TIdentifier>
    {
        #region Find & Query Methods

		TEntity FindOne(TIdentifier id);
		
		ValueTask<TEntity> FindOneAsync(TIdentifier id);
		
		TEntity FindOne(Expression<Func<TEntity, bool>> expression);
		
		ValueTask<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression);

		IPaginationCollection<TEntity> Find(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		IPaginationCollection<TEntity> Find(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);

		IPaginationCollection<TEntity> Find(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);

		ValueTask<IPaginationCollection<TEntity>> FindAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		#endregion
		
		#region Insert Methods

		TEntity Insert(TEntity entity);
		
		ValueTask<TEntity> InsertAsync(TEntity entity);
		
		void BulkInsert(IEnumerable<TEntity> entity);
		
		ValueTask BulkInsertAsync(IEnumerable<TEntity> entity);

		#endregion
		
		#region Update Methods

		TEntity Update(TEntity entity, TIdentifier id = default);
		
		ValueTask<TEntity> UpdateAsync(TEntity entity, TIdentifier id = default);

		TEntity Upsert(TEntity entity, TIdentifier id = default);
		
		ValueTask<TEntity> UpsertAsync(TEntity entity, TIdentifier id = default);

		#endregion
		
		#region Delete Methods

		bool Delete(TIdentifier id);
		
		ValueTask<bool> DeleteAsync(TIdentifier id);
		
		bool BulkDelete(IEnumerable<TEntity> entities);
		
		ValueTask<bool> BulkDeleteAsync(IEnumerable<TEntity> entities);

		#endregion
		
		#region Count Methods

		long Count();
		
		ValueTask<long> CountAsync();
		
		long Count(string query);
		
		ValueTask<long> CountAsync(string query);

		long Count(Expression<Func<TEntity, bool>> expression);

		ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> expression);

		#endregion
    }
}