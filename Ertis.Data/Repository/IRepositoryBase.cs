using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;

namespace Ertis.Data.Repository
{
    public interface IRepositoryBase<TEntity, in TIdentifier>
    {
        #region Find & Query Methods

		TEntity FindOne(TIdentifier id);
		
		Task<TEntity> FindOneAsync(TIdentifier id, CancellationToken cancellationToken = default);
		
		TEntity FindOne(Expression<Func<TEntity, bool>> expression);
		
		Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

		IPaginationCollection<TEntity> Find(int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null);
		IPaginationCollection<TEntity> Find(int? skip = null, int? limit = null, bool? withCount = null, Sorting sorting = null);
		
		Task<IPaginationCollection<TEntity>> FindAsync(int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default);
		
		Task<IPaginationCollection<TEntity>> FindAsync(int? skip = null, int? limit = null, bool? withCount = null, Sorting sorting = null, CancellationToken cancellationToken = default);
		
		IPaginationCollection<TEntity> Find(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null);
		
		IPaginationCollection<TEntity> Find(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, Sorting sorting = null);
		
		Task<IPaginationCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default);
		
		Task<IPaginationCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, Sorting sorting = null, CancellationToken cancellationToken = default);

		IPaginationCollection<TEntity> Find(string query, int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null);
		
		IPaginationCollection<TEntity> Find(string query, int? skip = null, int? limit = null, bool? withCount = null, Sorting sorting = null);

		Task<IPaginationCollection<TEntity>> FindAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default);
		
		Task<IPaginationCollection<TEntity>> FindAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, Sorting sorting = null, CancellationToken cancellationToken = default);
		
		#endregion
		
		#region Insert Methods

		TEntity Insert(TEntity entity, InsertOptions? options = null);
		
		Task<TEntity> InsertAsync(TEntity entity, InsertOptions? options = null, CancellationToken cancellationToken = default);
		
		void BulkInsert(IEnumerable<TEntity> entity, InsertOptions? options = null);
		
		Task BulkInsertAsync(IEnumerable<TEntity> entity, InsertOptions? options = null, CancellationToken cancellationToken = default);

		#endregion
		
		#region Update Methods

		TEntity Update(TEntity entity, TIdentifier id = default, UpdateOptions? options = null);
		
		Task<TEntity> UpdateAsync(TEntity entity, TIdentifier id = default, UpdateOptions? options = null, CancellationToken cancellationToken = default);

		TEntity Upsert(TEntity entity, TIdentifier id = default);
		
		Task<TEntity> UpsertAsync(TEntity entity, TIdentifier id = default, CancellationToken cancellationToken = default);

		#endregion
		
		#region Delete Methods

		bool Delete(TIdentifier id);
		
		Task<bool> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default);
		
		bool BulkDelete(IEnumerable<TEntity> entities);
		
		Task<bool> BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

		#endregion
		
		#region Count Methods

		long Count();
		
		Task<long> CountAsync(CancellationToken cancellationToken = default);
		
		long Count(string query);
		
		Task<long> CountAsync(string query, CancellationToken cancellationToken = default);

		long Count(Expression<Func<TEntity, bool>> expression);

		Task<long> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

		#endregion
    }
}