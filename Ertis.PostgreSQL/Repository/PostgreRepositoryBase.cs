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

		public virtual async ValueTask<TEntity> FindOneAsync(int id, CancellationToken cancellationToken = default)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			if (this.TrackingEnabled)
			{
				return await dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
			}
			else
			{
				return await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

		public virtual async ValueTask<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			if (this.TrackingEnabled)
			{
				return await dbSet.FirstOrDefaultAsync(expression, cancellationToken);
			}
			else
			{
				return await dbSet.AsNoTracking().FirstOrDefaultAsync(expression, cancellationToken);	
			}
		}

		public virtual IPaginationCollection<TEntity> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(null, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual async ValueTask<IPaginationCollection<TEntity>> FindAsync(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.ExecuteWhereAsync(null, skip, limit, withCount, sortField, sortDirection, cancellationToken);
		}

		public virtual IPaginationCollection<TEntity> Find(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual async ValueTask<IPaginationCollection<TEntity>> FindAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, sortField, sortDirection, cancellationToken);
		}

		public virtual IPaginationCollection<TEntity> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.ExecuteWhere(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual async ValueTask<IPaginationCollection<TEntity>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query);
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, sortField, sortDirection, cancellationToken);
		}

		private IPaginationCollection<TEntity> ExecuteWhere(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			IQueryable<TEntity> queryable;
			long? totalCount = null;

			var db = this.ConfigureDbSet(this.database);
			var dbSet = this.TrackingEnabled ? db : db.AsNoTracking();
			var sortExpression = ExpressionHelper.ConvertSortExpression<TEntity>(sortField);
			if (expression != null)
			{
				if (skip != null && limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value) : 
							dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Where(expression).Skip(skip.Value).Take(limit.Value);	
					}
				}
				else if (skip != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value) : 
							dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value);
					}
					else
					{
						queryable = dbSet.Where(expression).Skip(skip.Value);	
					}
				}
				else if (limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression).Take(limit.Value) : 
							dbSet.Where(expression).OrderBy(sortExpression).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Where(expression).Take(limit.Value);	
					}
				}
				else
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression) : 
							dbSet.Where(expression).OrderBy(sortExpression);
					}
					else
					{
						queryable = dbSet.Where(expression);	
					}
				}

				if (withCount != null && withCount.Value)
				{
					totalCount = dbSet.LongCount(expression);
				}
			}
			else
			{
				if (skip != null && limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value) : 
							dbSet.OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Skip(skip.Value).Take(limit.Value);	
					}
				}
				else if (skip != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression).Skip(skip.Value) : 
							dbSet.OrderBy(sortExpression).Skip(skip.Value);
					}
					else
					{
						queryable = dbSet.Skip(skip.Value);	
					}
				}
				else if (limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression).Take(limit.Value) : 
							dbSet.OrderBy(sortExpression).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Take(limit.Value);	
					}
				}
				else
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression) : 
							dbSet.OrderBy(sortExpression);
					}
					else
					{
						queryable = dbSet;	
					}
				}
				
				if (withCount != null && withCount.Value)
				{
					totalCount = dbSet.LongCount();
				}
			}

			return new PaginationCollection<TEntity>
			{
				Items = queryable.ToList(),
				Count = totalCount ?? 0
			};
		}
		
		private async ValueTask<IPaginationCollection<TEntity>> ExecuteWhereAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			IQueryable<TEntity> queryable;
			long? totalCount = null;
			
			var db = this.ConfigureDbSet(this.database);
			var dbSet = this.TrackingEnabled ? db : db.AsNoTracking();
			var sortExpression = ExpressionHelper.ConvertSortExpression<TEntity>(sortField);
			if (expression != null)
			{
				if (skip != null && limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value) : 
							dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Where(expression).Skip(skip.Value).Take(limit.Value);	
					}
				}
				else if (skip != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value) : 
							dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value);
					}
					else
					{
						queryable = dbSet.Where(expression).Skip(skip.Value);	
					}
				}
				else if (limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression).Take(limit.Value) : 
							dbSet.Where(expression).OrderBy(sortExpression).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Where(expression).Take(limit.Value);	
					}
				}
				else
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.Where(expression).OrderByDescending(sortExpression) : 
							dbSet.Where(expression).OrderBy(sortExpression);
					}
					else
					{
						queryable = dbSet.Where(expression);	
					}
				}

				if (withCount != null && withCount.Value)
				{
					totalCount = await dbSet.LongCountAsync(expression, cancellationToken);
				}
			}
			else
			{
				if (skip != null && limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value) : 
							dbSet.OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Skip(skip.Value).Take(limit.Value);	
					}
				}
				else if (skip != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression).Skip(skip.Value) : 
							dbSet.OrderBy(sortExpression).Skip(skip.Value);
					}
					else
					{
						queryable = dbSet.Skip(skip.Value);	
					}
				}
				else if (limit != null)
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression).Take(limit.Value) : 
							dbSet.OrderBy(sortExpression).Take(limit.Value);
					}
					else
					{
						queryable = dbSet.Take(limit.Value);	
					}
				}
				else
				{
					if (sortExpression != null)
					{
						queryable = sortDirection == SortDirection.Descending ? 
							dbSet.OrderByDescending(sortExpression) : 
							dbSet.OrderBy(sortExpression);
					}
					else
					{
						queryable = dbSet;	
					}
				}
				
				if (withCount != null && withCount.Value)
				{
					totalCount = await dbSet.LongCountAsync(cancellationToken);
				}
			}

			return new PaginationCollection<TEntity>
			{
				Items = await queryable.ToListAsync(cancellationToken),
				Count = totalCount ?? 0
			};
		}
		
		#endregion
		
		#region Insert Methods

		public virtual TEntity Insert(TEntity entity)
		{
			var cursor = this.GetDbSet().Add(entity);
			this.database.SaveChanges();
			return this.FindOne(cursor.Entity.Id);
		}

		public virtual async ValueTask<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			var cursor = (await this.GetDbSet().AddAsync(entity, cancellationToken));
			await this.database.SaveChangesAsync(cancellationToken);
			return await this.FindOneAsync(cursor.Entity.Id, cancellationToken);
		}
		
		public void BulkInsert(IEnumerable<TEntity> entities)
		{
			this.GetDbSet().AddRange(entities);
			this.database.SaveChanges();
		}

		public async ValueTask BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		{
			await this.GetDbSet().AddRangeAsync(entities, cancellationToken);
			await this.database.SaveChangesAsync(cancellationToken);
		}

		#endregion
		
		#region Update Methods

		public virtual TEntity Update(TEntity entity, int id = default)
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

		public virtual async ValueTask<TEntity> UpdateAsync(TEntity entity, int id = default, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				var cursor = this.GetDbSet().Update(entity);
				await this.database.SaveChangesAsync(cancellationToken);
				return cursor.Entity;	
			}
			else
			{
				var current = await this.GetDbSet().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
				if (current == null)
				{
					return null;
				}

				var entry = this.database.Entry(current);
				entry.State = EntityState.Modified;
				entry.CurrentValues.SetValues(entity);
				await this.database.SaveChangesAsync(cancellationToken);
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

		public virtual async ValueTask<TEntity> UpsertAsync(TEntity entity, int id, CancellationToken cancellationToken = default)
		{
			var current = await this.FindOneAsync(id > 0 ? id : entity.Id, cancellationToken);
			if (current == null)
			{
				return await this.InsertAsync(entity, cancellationToken);
			}
			else
			{
				return await this.UpdateAsync(entity, id, cancellationToken);
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

		private async ValueTask<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().Remove(entity);
				await this.database.SaveChangesAsync(cancellationToken);
				return true;
			}
			else
			{
				this.database.Entry(entity).State = EntityState.Deleted;
				this.GetDbSet().Remove(entity);
				await this.database.SaveChangesAsync(cancellationToken);
				return true;	
			}
		}
		
		public virtual async ValueTask<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			var entity = await this.FindOneAsync(id, cancellationToken);
			if (entity == null)
				return false;
			
			return await this.DeleteAsync(entity, cancellationToken);
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

		public async ValueTask<bool> BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().RemoveRange(entities);
				await this.database.SaveChangesAsync(cancellationToken);
				return true;
			}
			else
			{
				var table = this.GetDbSet();
				var noTrackingEntities = entities.Select(x => table.First(y => y.Id == x.Id));
				table.RemoveRange(noTrackingEntities);
				await this.database.SaveChangesAsync(cancellationToken);
				return true;	
			}
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

		public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				return await this.GetDbSet().LongCountAsync(cancellationToken);
			}
			else
			{
				return await this.GetDbSet().AsNoTracking().LongCountAsync(cancellationToken);	
			}
		}

		public virtual long Count(string query)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.Count(expression);
		}

		public virtual async ValueTask<long> CountAsync(string query, CancellationToken cancellationToken = default)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query);
			return await this.CountAsync(expression, cancellationToken);
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

		public virtual async ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
		{
			if (this.TrackingEnabled)
			{
				return await this.GetDbSet().CountAsync(expression, cancellationToken);
			}
			else
			{
				return await this.GetDbSet().AsNoTracking().CountAsync(expression, cancellationToken);	
			}
		}

		#endregion

		#region Cursor Methods

		protected int SaveChanges()
		{
			return this.database.SaveChanges();
		}
		
		protected async ValueTask<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await this.database.SaveChangesAsync(cancellationToken);
		}

		#endregion
	}
}