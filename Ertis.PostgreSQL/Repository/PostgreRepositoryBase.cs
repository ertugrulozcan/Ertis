using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.PostgreSQL.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Ertis.PostgreSQL.Repository
{
	public abstract class PostgreRepositoryBase<TEntity> : IPostgreRepository<TEntity> where TEntity : class, IEntity<int>
	{
		#region Services

		private readonly DbContext database;

		#endregion

		#region Properties

		public bool TrackingEnabled { get; set; } = false;

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext"></param>
		protected PostgreRepositoryBase(DbContext dbContext)
		{
			this.database = dbContext;
		}

		#endregion
		
		#region Find Methods

		// ReSharper disable once MemberCanBePrivate.Global
		protected DbSet<TEntity> GetDbSet()
		{
			return this.database.Set<TEntity>();
		}
		
		protected virtual IQueryable<TEntity> ConfigureDbSet(DbContext dbContext)
		{
			return this.GetDbSet();
		}
		
		public virtual TEntity FindOne(int id)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			return this.TrackingEnabled ? 
				dbSet.FirstOrDefault(x => x.Id == id) : 
				dbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);
		}

		public virtual async Task<TEntity> FindOneAsync(int id, CancellationToken cancellationToken = default)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			if (this.TrackingEnabled)
			{
				return await dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
			}
			else
			{
				return await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
			}
		}

		public virtual TEntity FindOne(Expression<Func<TEntity, bool>> expression)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			if (this.TrackingEnabled)
			{
				return dbSet.FirstOrDefault(expression);	
			}
			else
			{
				return dbSet.AsNoTracking().FirstOrDefault(expression);
			}
		}

		public virtual async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			if (this.TrackingEnabled)
			{
				return await dbSet.FirstOrDefaultAsync(expression, cancellationToken: cancellationToken);
			}
			else
			{
				return await dbSet.AsNoTracking().FirstOrDefaultAsync(expression, cancellationToken: cancellationToken);	
			}
		}

		public virtual IPaginationCollection<TEntity> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(null, skip, limit, withCount, orderBy, sortDirection);
		}
		
		public virtual IPaginationCollection<TEntity> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null)
		{
			return this.ExecuteWhere(null, skip, limit, withCount, sorting);
		}

		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.ExecuteWhereAsync(null, skip, limit, withCount, orderBy, sortDirection, cancellationToken: cancellationToken);
		}
		
		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.ExecuteWhereAsync(null, skip, limit, withCount, sorting, cancellationToken: cancellationToken);
		}

		public virtual IPaginationCollection<TEntity> Find(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(expression, skip, limit, withCount, orderBy, sortDirection);
		}
		
		public virtual IPaginationCollection<TEntity> Find(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null)
		{
			return this.ExecuteWhere(expression, skip, limit, withCount, sorting);
		}

		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, orderBy, sortDirection, cancellationToken: cancellationToken);
		}
		
		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null,
			CancellationToken cancellationToken = default)
		{
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, sorting, cancellationToken: cancellationToken);
		}

		public virtual IPaginationCollection<TEntity> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.ExecuteWhere(expression, skip, limit, withCount, orderBy, sortDirection);
		}
		
		public virtual IPaginationCollection<TEntity> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.ExecuteWhere(expression, skip, limit, withCount, sorting);
		}

		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query, cancellationToken: cancellationToken);
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, orderBy, sortDirection, cancellationToken: cancellationToken);
		}
		
		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null,
			CancellationToken cancellationToken = default)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query, cancellationToken: cancellationToken);
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, sorting, cancellationToken: cancellationToken);
		}

		private IPaginationCollection<TEntity> ExecuteWhere(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(
				expression,
				skip,
				limit,
				withCount,
				new Sorting(orderBy, sortDirection));
		}

		private IPaginationCollection<TEntity> ExecuteWhere(
			Expression<Func<TEntity, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null)
		{
			var db = this.ConfigureDbSet(this.database);
			var dbSet = this.TrackingEnabled ? db : db.AsNoTracking();
			var queryable = expression != null ? dbSet.Where(expression) : dbSet;

			if (sorting is { Count: > 0 })
			{
				for (var i = 0; i < sorting.Count; i++)
				{
					var sortExpression = ExpressionHelper.ConvertSortExpression<TEntity>(sorting[i].OrderBy);
					if (sortExpression != null)
					{
						if (i > 0 && queryable is IOrderedQueryable<TEntity> orderedQueryable)
						{
							queryable = sorting[i].SortDirection == SortDirection.Descending 
								? orderedQueryable.ThenByDescending(sortExpression) 
								: orderedQueryable.ThenBy(sortExpression);
						}
						else
						{
							queryable = sorting[i].SortDirection == SortDirection.Descending 
								? queryable.OrderByDescending(sortExpression) 
								: queryable.OrderBy(sortExpression);
						}
					}
				}
			}
			
			if (skip != null && limit != null)
			{
				queryable = queryable.Skip(skip.Value).Take(limit.Value);
			}
			else if (skip != null)
			{
				queryable = queryable.Skip(skip.Value);
			}
			else if (limit != null)
			{
				queryable = queryable.Take(limit.Value);
			}

			long? totalCount = null;
			if (withCount != null && withCount.Value)
			{
				totalCount = expression != null ? 
					dbSet.LongCount(expression) : 
					dbSet.LongCount();
			}

			return new PaginationCollection<TEntity>
			{
				Items = queryable.ToList(),
				Count = totalCount ?? 0
			};
		}
		
		private async Task<IPaginationCollection<TEntity>> ExecuteWhereAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.ExecuteWhereAsync(
				expression,
				skip,
				limit,
				withCount,
				new Sorting(orderBy, sortDirection),
				cancellationToken: cancellationToken);
		}

		private async Task<IPaginationCollection<TEntity>> ExecuteWhereAsync(
			Expression<Func<TEntity, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null,
			CancellationToken cancellationToken = default)
		{
			var db = this.ConfigureDbSet(this.database);
			var dbSet = this.TrackingEnabled ? db : db.AsNoTracking();
			var queryable = expression != null ? dbSet.Where(expression) : dbSet;

			if (sorting is { Count: > 0 })
			{
				for (var i = 0; i < sorting.Count; i++)
				{
					var sortExpression = ExpressionHelper.ConvertSortExpression<TEntity>(sorting[i].OrderBy);
					if (sortExpression != null)
					{
						if (i > 0 && queryable is IOrderedQueryable<TEntity> orderedQueryable)
						{
							queryable = sorting[i].SortDirection == SortDirection.Descending 
								? orderedQueryable.ThenByDescending(sortExpression) 
								: orderedQueryable.ThenBy(sortExpression);
						}
						else
						{
							queryable = sorting[i].SortDirection == SortDirection.Descending 
								? queryable.OrderByDescending(sortExpression) 
								: queryable.OrderBy(sortExpression);
						}
					}
				}
			}
			
			if (skip != null && limit != null)
			{
				queryable = queryable.Skip(skip.Value).Take(limit.Value);
			}
			else if (skip != null)
			{
				queryable = queryable.Skip(skip.Value);
			}
			else if (limit != null)
			{
				queryable = queryable.Take(limit.Value);
			}

			long? totalCount = null;
			if (withCount != null && withCount.Value)
			{
				totalCount = expression != null ? 
					await dbSet.LongCountAsync(expression, cancellationToken: cancellationToken) : 
					await dbSet.LongCountAsync(cancellationToken: cancellationToken);
			}

			return new PaginationCollection<TEntity>
			{
				Items = await queryable.ToListAsync(cancellationToken: cancellationToken),
				Count = totalCount ?? 0
			};
		}
		
		#endregion
		
		#region Insert Methods

		public virtual TEntity Insert(TEntity entity, InsertOptions? options = null)
		{
			var cursor = this.GetDbSet().Add(entity);
			this.database.SaveChanges();
			return this.FindOne(cursor.Entity.Id);
		}

		public virtual async Task<TEntity> InsertAsync(TEntity entity, InsertOptions? options = null, CancellationToken cancellationToken = default)
		{
			var cursor = (await this.GetDbSet().AddAsync(entity, cancellationToken: cancellationToken));
			await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
			return await this.FindOneAsync(cursor.Entity.Id, cancellationToken: cancellationToken);
		}
		
		public void BulkInsert(IEnumerable<TEntity> entities, InsertOptions? options = null)
		{
			this.GetDbSet().AddRange(entities);
			this.database.SaveChanges();
		}

		public async Task BulkInsertAsync(IEnumerable<TEntity> entities, InsertOptions? options = null, CancellationToken cancellationToken = default)
		{
			await this.GetDbSet().AddRangeAsync(entities, cancellationToken: cancellationToken);
			await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
		}

		#endregion
		
		#region Update Methods

		public virtual TEntity Update(TEntity entity, int id = default, UpdateOptions? options = null)
		{
			if (this.TrackingEnabled)
			{
				var cursor = this.GetDbSet().Update(entity);
				this.database.SaveChanges();
				return this.FindOne(cursor.Entity.Id);
			}
			else
			{
				var current = this.GetDbSet().AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
				if (current == null)
				{
					return null;
				}

				var entry = this.database.Entry(current);
				entry.State = EntityState.Modified;
				entry.CurrentValues.SetValues(entity);
				this.database.SaveChanges();
				return entity;
			}
		}

		public virtual async Task<TEntity> UpdateAsync(TEntity entity, int id = default, UpdateOptions? options = null, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				var cursor = this.GetDbSet().Update(entity);
				await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
				return cursor.Entity;	
			}
			else
			{
				var current = await this.GetDbSet().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken: cancellationToken);
				if (current == null)
				{
					return null;
				}

				var entry = this.database.Entry(current);
				entry.State = EntityState.Modified;
				entry.CurrentValues.SetValues(entity);
				await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
				return entity;
			}
		}

		public virtual TEntity Upsert(TEntity entity, int id)
		{
			var current = this.FindOne(id > 0 ? id : entity.Id);
			if (current == null)
			{
				return this.Insert(entity);
			}
			else
			{
				return this.Update(entity, id);
			}
		}

		public virtual async Task<TEntity> UpsertAsync(TEntity entity, int id, CancellationToken cancellationToken = default)
		{
			var current = await this.FindOneAsync(id > 0 ? id : entity.Id, cancellationToken: cancellationToken);
			if (current == null)
			{
				return await this.InsertAsync(entity, cancellationToken: cancellationToken);
			}
			else
			{
				return await this.UpdateAsync(entity, id, cancellationToken: cancellationToken);
			}
		}

		#endregion
		
		#region Delete Methods

		private bool Delete(TEntity entity)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().Remove(entity);
				this.database.SaveChanges();
				return true;
			}
			else
			{
				this.database.Entry(entity).State = EntityState.Deleted;
				this.GetDbSet().Remove(entity);
				this.database.SaveChanges();
				return true;	
			}
		}
		
		public virtual bool Delete(int id)
		{
			var entity = this.FindOne(id);
			if (entity == null)
				return false;
			
			return this.Delete(entity);
		}

		private async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().Remove(entity);
				await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
				return true;
			}
			else
			{
				this.database.Entry(entity).State = EntityState.Deleted;
				this.GetDbSet().Remove(entity);
				await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
				return true;	
			}
		}
		
		public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			var entity = await this.FindOneAsync(id, cancellationToken: cancellationToken);
			if (entity == null)
				return false;
			
			return await this.DeleteAsync(entity, cancellationToken: cancellationToken);
		}
		
		public bool BulkDelete(IEnumerable<TEntity> entities)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().RemoveRange(entities);
				this.database.SaveChanges();
				return true;
			}
			else
			{
				var table = this.GetDbSet();
				var noTrackingEntities = entities.Select(x => table.First(y => y.Id == x.Id));
				table.RemoveRange(noTrackingEntities);
				this.database.SaveChanges();
				return true;	
			}
		}

		public async Task<bool> BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().RemoveRange(entities);
				await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
				return true;
			}
			else
			{
				var table = this.GetDbSet();
				var noTrackingEntities = entities.Select(x => table.First(y => y.Id == x.Id));
				table.RemoveRange(noTrackingEntities);
				await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
				return true;	
			}
		}
		
		public bool DeleteMany(Expression<Func<TEntity, bool>> expression)
		{
			throw new Exception("The method or operation is not implemented for PostgreSQL.");
		}
		
		public async Task<bool> DeleteManyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask;
			throw new Exception("The method or operation is not implemented for PostgreSQL.");
		}
		
		public bool DeleteMany(string query)
		{
			throw new Exception("The method or operation is not implemented for PostgreSQL.");
		}
		
		public async Task<bool> DeleteManyAsync(string query, CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask;
			throw new Exception("The method or operation is not implemented for PostgreSQL.");
		}
		
		public bool Clear()
		{
			throw new Exception("The method or operation is not implemented for PostgreSQL.");
		}
		
		public async Task<bool> ClearAsync(CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask;
			throw new Exception("The method or operation is not implemented for PostgreSQL.");
		}

		#endregion
		
		#region Count Methods

		public virtual long Count()
		{
			if (this.TrackingEnabled)
			{
				return this.GetDbSet().LongCount();
			}
			else
			{
				return this.GetDbSet().AsNoTracking().LongCount();	
			}
		}

		public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				return await this.GetDbSet().LongCountAsync(cancellationToken: cancellationToken);
			}
			else
			{
				return await this.GetDbSet().AsNoTracking().LongCountAsync(cancellationToken: cancellationToken);	
			}
		}

		public virtual long Count(string query)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.Count(expression);
		}

		public virtual async Task<long> CountAsync(string query, CancellationToken cancellationToken = default)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query, cancellationToken: cancellationToken);
			return await this.CountAsync(expression, cancellationToken: cancellationToken);
		}

		public virtual long Count(Expression<Func<TEntity, bool>> expression)
		{
			if (this.TrackingEnabled)
			{
				return this.GetDbSet().Count(expression);
			}
			else
			{
				return this.GetDbSet().AsNoTracking().Count(expression);	
			}
		}

		public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				return await this.GetDbSet().CountAsync(expression, cancellationToken: cancellationToken);
			}
			else
			{
				return await this.GetDbSet().AsNoTracking().CountAsync(expression, cancellationToken: cancellationToken);	
			}
		}

		#endregion

		#region Cursor Methods

		protected int SaveChanges()
		{
			return this.database.SaveChanges();
		}
		
		protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await this.database.SaveChangesAsync(cancellationToken: cancellationToken);
		}

		#endregion
	}
}