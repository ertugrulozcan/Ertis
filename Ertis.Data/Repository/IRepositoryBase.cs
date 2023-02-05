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
		
		ValueTask<TEntity> FindOneAsync(TIdentifier id, CancellationToken cancellationToken = default);
		
		TEntity FindOne(Expression<Func<TEntity, bool>> expression);
		
		ValueTask<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

		IPaginationCollection<TEntity> Find(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default);
		
		IPaginationCollection<TEntity> Find(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default);

		IPaginationCollection<TEntity> Find(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);

		ValueTask<IPaginationCollection<TEntity>> FindAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default);
		
		#endregion
		
		#region Insert Methods

		TEntity Insert(TEntity entity, InsertOptions? options = null);
		
		ValueTask<TEntity> InsertAsync(TEntity entity, InsertOptions? options = null, CancellationToken cancellationToken = default);
		
		void BulkInsert(IEnumerable<TEntity> entity, InsertOptions? options = null);
		
		ValueTask BulkInsertAsync(IEnumerable<TEntity> entity, InsertOptions? options = null, CancellationToken cancellationToken = default);

		#endregion
		
		#region Update Methods

		TEntity Update(TEntity entity, TIdentifier id = default, UpdateOptions? options = null);
		
		ValueTask<TEntity> UpdateAsync(TEntity entity, TIdentifier id = default, UpdateOptions? options = null, CancellationToken cancellationToken = default);

		TEntity Upsert(TEntity entity, TIdentifier id = default);
		
		ValueTask<TEntity> UpsertAsync(TEntity entity, TIdentifier id = default, CancellationToken cancellationToken = default);

		#endregion
		
		#region Delete Methods

		bool Delete(TIdentifier id);
		
		ValueTask<bool> DeleteAsync(TIdentifier id, CancellationToken cancellationToken = default);
		
		bool BulkDelete(IEnumerable<TEntity> entities);
		
		ValueTask<bool> BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

		#endregion
		
		#region Count Methods

		long Count();
		
		ValueTask<long> CountAsync(CancellationToken cancellationToken = default);
		
		long Count(string query);
		
		ValueTask<long> CountAsync(string query, CancellationToken cancellationToken = default);

		long Count(Expression<Func<TEntity, bool>> expression);

		ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

		#endregion
    }
}