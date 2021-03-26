using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.PostgreSQL.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

		protected virtual IQueryable<TEntity> ConfigureDbSet(DbContext dbContext)
		{
			return dbContext.Set<TEntity>();
		}
		
		public TEntity FindOne(int id)
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

		public async Task<TEntity> FindOneAsync(int id)
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

		public TEntity FindOne(Expression<Func<TEntity, bool>> expression)
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

		public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression)
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

		public IPaginationCollection<TEntity> Find(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(null, skip, limit, withCount, sortField, sortDirection);
		}

		public async Task<IPaginationCollection<TEntity>> FindAsync(int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return await this.ExecuteWhereAsync(null, skip, limit, withCount, sortField, sortDirection);
		}

		public IPaginationCollection<TEntity> Find(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return this.ExecuteWhere(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public async Task<IPaginationCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			return await this.ExecuteWhereAsync(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public IPaginationCollection<TEntity> Find(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.ExecuteWhere(expression, skip, limit, withCount, sortField, sortDirection);
		}

		public async Task<IPaginationCollection<TEntity>> FindAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null)
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
			if (expression != null)
			{
				if (skip != null && limit != null)
				{
					queryable = dbSet.Where(expression).Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = dbSet.Where(expression).Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = dbSet.Where(expression).Take(limit.Value);
				}
				else
				{
					queryable = dbSet.Where(expression);
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
					queryable = dbSet.Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = dbSet.Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = dbSet.Take(limit.Value);
				}
				else
				{
					queryable = dbSet;
				}
				
				if (withCount != null && withCount.Value)
				{
					totalCount = dbSet.LongCount();
				}
			}

			if (!string.IsNullOrEmpty(sortField))
			{
				var type = typeof(TEntity);
				var propertyInfo = type.GetProperty(sortField);
				if (propertyInfo == null)
				{
					propertyInfo = type.GetProperties().FirstOrDefault(x => x
						.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
						.Cast<JsonPropertyAttribute>()
						.FirstOrDefault(y => y.PropertyName == sortField) != null);	
				}

				if (propertyInfo != null)
				{
					Expression<Func<TEntity, dynamic>> sortExpression = x => propertyInfo.GetValue(x, null);
					if (sortDirection == SortDirection.Descending)
					{
						queryable = queryable.OrderByDescending(sortExpression);
					}
					else
					{
						queryable = queryable.OrderBy(sortExpression);
					}
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
			if (expression != null)
			{
				if (skip != null && limit != null)
				{
					queryable = dbSet.Where(expression).Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = dbSet.Where(expression).Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = dbSet.Where(expression).Take(limit.Value);
				}
				else
				{
					queryable = dbSet.Where(expression);
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
					queryable = dbSet.Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = dbSet.Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = dbSet.Take(limit.Value);
				}
				else
				{
					queryable = dbSet;
				}
				
				if (withCount != null && withCount.Value)
				{
					totalCount = await dbSet.LongCountAsync();
				}
			}

			if (!string.IsNullOrEmpty(sortField))
			{
				var type = typeof(TEntity);
				var propertyInfo = type.GetProperty(sortField);
				if (propertyInfo == null)
				{
					propertyInfo = type.GetProperties().FirstOrDefault(x => x
						.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
						.Cast<JsonPropertyAttribute>()
						.FirstOrDefault(y => y.PropertyName == sortField) != null);	
				}

				if (propertyInfo != null)
				{
					Expression<Func<TEntity, dynamic>> sortExpression = x => propertyInfo.GetValue(x, null);
					if (sortDirection == SortDirection.Descending)
					{
						queryable = queryable.OrderByDescending(sortExpression);
					}
					else
					{
						queryable = queryable.OrderBy(sortExpression);
					}
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

		public TEntity Insert(TEntity entity)
		{
			var cursor = this.database.Set<TEntity>().Add(entity);
			this.database.SaveChanges();
			return cursor?.Entity;
		}

		public async Task<TEntity> InsertAsync(TEntity entity)
		{
			var cursor = (await this.database.Set<TEntity>().AddAsync(entity));
			await this.database.SaveChangesAsync();
			return cursor?.Entity;
		}

		#endregion
		
		#region Update Methods

		public TEntity Update(TEntity entity)
		{
			if (this.TrackingEnabled)
			{
				var cursor = this.database.Set<TEntity>().Update(entity);
				this.database.SaveChanges();
				return cursor?.Entity;
			}
			else
			{
				var current = this.database.Set<TEntity>().AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
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

		public async Task<TEntity> UpdateAsync(TEntity entity)
		{
			if (this.TrackingEnabled)
			{
				var cursor = this.database.Set<TEntity>().Update(entity);
				await this.database.SaveChangesAsync();
				return cursor?.Entity;	
			}
			else
			{
				var current = await this.database.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
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

		public TEntity Upsert(TEntity entity)
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

		public async Task<TEntity> UpsertAsync(TEntity entity)
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
			this.database.Set<TEntity>().Remove(entity);
			this.database.SaveChanges();
			return true;
		}
		
		public bool Delete(int id)
		{
			var entity = this.FindOne(id);
			if (entity == null)
				return false;
			
			return this.Delete(entity);
		}

		public async Task<bool> DeleteAsync(TEntity entity)
		{
			this.database.Set<TEntity>().Remove(entity);
			await this.database.SaveChangesAsync();
			return true;
		}
		
		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await this.FindOneAsync(id);
			if (entity == null)
				return false;
			
			return await this.DeleteAsync(entity);
		}

		#endregion
		
		#region Count Methods

		public long Count()
		{
			if (this.TrackingEnabled)
			{
				return this.database.Set<TEntity>().LongCount();
			}
			else
			{
				return this.database.Set<TEntity>().AsNoTracking().LongCount();	
			}
		}

		public async Task<long> CountAsync()
		{
			if (this.TrackingEnabled)
			{
				return await this.database.Set<TEntity>().LongCountAsync();
			}
			else
			{
				return await this.database.Set<TEntity>().AsNoTracking().LongCountAsync();	
			}
		}

		public long Count(string query)
		{
			var expression = ExpressionHelper.ParseExpression<TEntity>(query);
			return this.Count(expression);
		}

		public async Task<long> CountAsync(string query)
		{
			var expression = await ExpressionHelper.ParseExpressionAsync<TEntity>(query);
			return await this.CountAsync(expression);
		}

		public long Count(Expression<Func<TEntity, bool>> expression)
		{
			if (this.TrackingEnabled)
			{
				return this.database.Set<TEntity>().Count(expression);
			}
			else
			{
				return this.database.Set<TEntity>().AsNoTracking().Count(expression);	
			}
		}

		public async Task<long> CountAsync(Expression<Func<TEntity, bool>> expression)
		{
			if (this.TrackingEnabled)
			{
				return await this.database.Set<TEntity>().CountAsync(expression);
			}
			else
			{
				return await this.database.Set<TEntity>().AsNoTracking().CountAsync(expression);	
			}
		}

		#endregion
	}
}