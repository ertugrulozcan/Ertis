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

		public TEntity FindOne(int id)
		{
			return this.database.Set<TEntity>().Find(id);
		}

		public async Task<TEntity> FindOneAsync(int id)
		{
			return await this.database.Set<TEntity>().FindAsync(id);
		}

		public TEntity FindOne(Expression<Func<TEntity, bool>> expression)
		{
			return this.database.Set<TEntity>().FirstOrDefault(expression);
		}

		public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression)
		{
			return await this.database.Set<TEntity>().FirstOrDefaultAsync(expression);
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
			
			if (expression != null)
			{
				if (skip != null && limit != null)
				{
					queryable = this.database.Set<TEntity>().Where(expression).Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = this.database.Set<TEntity>().Where(expression).Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = this.database.Set<TEntity>().Where(expression).Take(limit.Value);
				}
				else
				{
					queryable = this.database.Set<TEntity>().Where(expression);
				}

				if (withCount != null && withCount.Value)
				{
					totalCount = this.database.Set<TEntity>().LongCount(expression);
				}
			}
			else
			{
				if (skip != null && limit != null)
				{
					queryable = this.database.Set<TEntity>().Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = this.database.Set<TEntity>().Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = this.database.Set<TEntity>().Take(limit.Value);
				}
				else
				{
					queryable = this.database.Set<TEntity>();
				}
				
				if (withCount != null && withCount.Value)
				{
					totalCount = this.database.Set<TEntity>().LongCount();
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
			
			if (expression != null)
			{
				if (skip != null && limit != null)
				{
					queryable = this.database.Set<TEntity>().Where(expression).Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = this.database.Set<TEntity>().Where(expression).Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = this.database.Set<TEntity>().Where(expression).Take(limit.Value);
				}
				else
				{
					queryable = this.database.Set<TEntity>().Where(expression);
				}

				if (withCount != null && withCount.Value)
				{
					totalCount = await this.database.Set<TEntity>().LongCountAsync(expression);
				}
			}
			else
			{
				if (skip != null && limit != null)
				{
					queryable = this.database.Set<TEntity>().Skip(skip.Value).Take(limit.Value);
				}
				else if (skip != null)
				{
					queryable = this.database.Set<TEntity>().Skip(skip.Value);
				}
				else if (limit != null)
				{
					queryable = this.database.Set<TEntity>().Take(limit.Value);
				}
				else
				{
					queryable = this.database.Set<TEntity>();
				}
				
				if (withCount != null && withCount.Value)
				{
					totalCount = await this.database.Set<TEntity>().LongCountAsync();
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
			var cursor = this.database.Set<TEntity>().Update(entity);
			this.database.SaveChanges();
			return cursor?.Entity;
		}

		public async Task<TEntity> UpdateAsync(TEntity entity)
		{
			var cursor = this.database.Set<TEntity>().Update(entity);
			await this.database.SaveChangesAsync();
			return cursor?.Entity;
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
			return this.database.Set<TEntity>().LongCount();
		}

		public async Task<long> CountAsync()
		{
			return await this.database.Set<TEntity>().LongCountAsync();
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
			return this.database.Set<TEntity>().Count(expression);
		}

		public async Task<long> CountAsync(Expression<Func<TEntity, bool>> expression)
		{
			return await this.database.Set<TEntity>().CountAsync(expression);
		}

		#endregion
	}
}