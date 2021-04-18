using System;
using System.Linq;
using System.Linq.Expressions;
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
			if (this.TrackingEnabled)
			{
				return dbSet.FirstOrDefault(x => x.Id == id);
			}
			else
			{
				return dbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);
			}
		}

		public virtual async Task<TEntity> FindOneAsync(int id)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			if (this.TrackingEnabled)
			{
				return await dbSet.FirstOrDefaultAsync(x => x.Id == id);
			}
			else
			{
				return await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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

		public virtual async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression)
		{
			var dbSet = this.ConfigureDbSet(this.database);
			if (this.TrackingEnabled)
			{
				return await dbSet.FirstOrDefaultAsync(expression);
			}
			else
			{
				return await dbSet.AsNoTracking().FirstOrDefaultAsync(expression);	
			}
		}

		public virtual IPaginationCollection<TEntity> Find(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(null, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return await this.ExecuteWhereAsync(null, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual IPaginationCollection<TEntity> Find(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual IPaginationCollection<TEntity> Find(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.ExecuteWhere(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public virtual async Task<IPaginationCollection<TEntity>> FindAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query);
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, sortField, sortDirection);
		}

		private IPaginationCollection<TEntity> ExecuteWhere(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value);	
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression).Take(limit.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression);
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression).Skip(skip.Value);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression).Skip(skip.Value);		
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression).Take(limit.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression);
						}
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
		
		private async Task<IPaginationCollection<TEntity>> ExecuteWhereAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);	
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression).Skip(skip.Value);
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression).Skip(skip.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression).Take(limit.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.Where(expression).OrderByDescending(sortExpression);
						}
						else
						{
							queryable = dbSet.Where(expression).OrderBy(sortExpression);
						}
					}
					else
					{
						queryable = dbSet.Where(expression);	
					}
				}

				if (withCount != null && withCount.Value)
				{
					totalCount = await dbSet.LongCountAsync(expression);
				}
			}
			else
			{
				if (skip != null && limit != null)
				{
					if (sortExpression != null)
					{
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression).Skip(skip.Value).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression).Skip(skip.Value).Take(limit.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression).Skip(skip.Value);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression).Skip(skip.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression).Take(limit.Value);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression).Take(limit.Value);
						}
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
						if (sortDirection == SortDirection.Descending)
						{
							queryable = dbSet.OrderByDescending(sortExpression);
						}
						else
						{
							queryable = dbSet.OrderBy(sortExpression);
						}
					}
					else
					{
						queryable = dbSet;	
					}
				}
				
				if (withCount != null && withCount.Value)
				{
					totalCount = await dbSet.LongCountAsync();
				}
			}

			return new PaginationCollection<TEntity>
			{
				Items = await queryable.ToListAsync(),
				Count = totalCount ?? 0
			};
		}
		
		#endregion
		
		#region Insert Methods

		public virtual TEntity Insert(TEntity entity)
		{
			var cursor = this.GetDbSet().Add(entity);
			this.database.SaveChanges();
			if (cursor == null)
			{
				return null;
			}
			
			return this.FindOne(cursor.Entity.Id);
		}

		public virtual async Task<TEntity> InsertAsync(TEntity entity)
		{
			var cursor = (await this.GetDbSet().AddAsync(entity));
			await this.database.SaveChangesAsync();
			if (cursor == null)
			{
				return null;
			}
			
			return await this.FindOneAsync(cursor.Entity.Id);
		}

		#endregion
		
		#region Update Methods

		public virtual TEntity Update(TEntity entity)
		{
			if (this.TrackingEnabled)
			{
				var cursor = this.GetDbSet().Update(entity);
				this.database.SaveChanges();
				
				if (cursor == null)
				{
					return null;
				}
			
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

		public virtual async Task<TEntity> UpdateAsync(TEntity entity)
		{
			if (this.TrackingEnabled)
			{
				var cursor = this.GetDbSet().Update(entity);
				await this.database.SaveChangesAsync();
				return cursor?.Entity;	
			}
			else
			{
				var current = await this.GetDbSet().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
				if (current == null)
				{
					return null;
				}

				var entry = this.database.Entry(current);
				entry.State = EntityState.Modified;
				entry.CurrentValues.SetValues(entity);
				await this.database.SaveChangesAsync();
				return entity;
			}
		}

		public virtual TEntity Upsert(TEntity entity)
		{
			var current = this.FindOne(entity.Id);
			if (current == null)
			{
				return this.Insert(entity);
			}
			else
			{
				return this.Update(entity);
			}
		}

		public virtual async Task<TEntity> UpsertAsync(TEntity entity)
		{
			var current = await this.FindOneAsync(entity.Id);
			if (current == null)
			{
				return await this.InsertAsync(entity);
			}
			else
			{
				return await this.UpdateAsync(entity);
			}
		}

		#endregion
		
		#region Delete Methods

		public bool Delete(TEntity entity)
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

		public async Task<bool> DeleteAsync(TEntity entity)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().Remove(entity);
				await this.database.SaveChangesAsync();
				return true;
			}
			else
			{
				this.database.Entry(entity).State = EntityState.Deleted;
				this.GetDbSet().Remove(entity);
				await this.database.SaveChangesAsync();
				return true;	
			}
		}
		
		public virtual async Task<bool> DeleteAsync(int id)
		{
			var entity = await this.FindOneAsync(id);
			if (entity == null)
				return false;
			
			return await this.DeleteAsync(entity);
		}
		
		public bool BulkDelete(TEntity[] entities)
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

		public async Task<bool> BulkDeleteAsync(TEntity[] entities)
		{
			if (this.TrackingEnabled)
			{
				this.GetDbSet().RemoveRange(entities);
				await this.database.SaveChangesAsync();
				return true;
			}
			else
			{
				var table = this.GetDbSet();
				var noTrackingEntities = entities.Select(x => table.First(y => y.Id == x.Id));
				table.RemoveRange(noTrackingEntities);
				await this.database.SaveChangesAsync();
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

		public virtual async Task<long> CountAsync()
		{
			if (this.TrackingEnabled)
			{
				return await this.GetDbSet().LongCountAsync();
			}
			else
			{
				return await this.GetDbSet().AsNoTracking().LongCountAsync();	
			}
		}

		public virtual long Count(string query)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.Count(expression);
		}

		public virtual async Task<long> CountAsync(string query)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query);
			return await this.CountAsync(expression);
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

		public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> expression)
		{
			if (this.TrackingEnabled)
			{
				return await this.GetDbSet().CountAsync(expression);
			}
			else
			{
				return await this.GetDbSet().AsNoTracking().CountAsync(expression);	
			}
		}

		#endregion

		#region Cursor Methods

		protected int SaveChanges()
		{
			return this.database.SaveChanges();
		}
		
		protected async Task<int> SaveChangesAsync()
		{
			return await this.database.SaveChangesAsync();
		}

		#endregion
	}
}