using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using SortDirection = Ertis.Core.Collections.SortDirection;

namespace Ertis.MongoDB.Repository
{
	public abstract class MongoRepositoryBase<TEntity> : IMongoRepository<TEntity> where TEntity : IEntity<string>
	{
		#region Properties

		private IMongoCollection<TEntity> Collection { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="collectionName"></param>
		protected MongoRepositoryBase(IDatabaseSettings settings, string collectionName)
		{
			string connectionString = ConnectionStringHelper.GenerateConnectionString(settings);
			var client = new MongoClient(connectionString);
			var database = client.GetDatabase(settings.DatabaseName);

			this.Collection = database.GetCollection<TEntity>(collectionName);
		}

		#endregion
		
		#region Find Methods

		public TEntity FindOne(string id)
		{
			return this.Collection.Find(item => item.Id == id).FirstOrDefault();
		}
		
		public async Task<TEntity> FindOneAsync(string id)
		{
			return await this.Collection.Find(item => item.Id == id).FirstOrDefaultAsync();
		}

		public IPaginationCollection<TEntity> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null,
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			return this.Find(
				expression: null,
				skip,
				limit,
				withCount,
				sortField,
				sortDirection);
		}

		public async Task<IPaginationCollection<TEntity>> FindAsync(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null)
		{
			return await this.FindAsync(
				expression: null,
				skip,
				limit,
				withCount,
				sortField,
				sortDirection);
		}
		
		public IPaginationCollection<TEntity> Find(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			FilterDefinition<TEntity> filterExpression = expression != null ? new ExpressionFilterDefinition<TEntity>(expression) : FilterDefinition<TEntity>.Empty;
			return this.Filter(filterExpression, skip, limit, withCount, sortField, sortDirection);
		}
		
		public async Task<IPaginationCollection<TEntity>> FindAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			FilterDefinition<TEntity> filterExpression = expression != null ? new ExpressionFilterDefinition<TEntity>(expression) : FilterDefinition<TEntity>.Empty;
			return await this.FilterAsync(filterExpression, skip, limit, withCount, sortField, sortDirection);
		}
		
		public IPaginationCollection<TEntity> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<TEntity>.Empty : new JsonFilterDefinition<TEntity>(query);
			return this.Filter(filterDefinition, skip, limit, withCount, sortField, sortDirection);
		}
		
		public async Task<IPaginationCollection<TEntity>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<TEntity>.Empty : new JsonFilterDefinition<TEntity>(query);
			return await this.FilterAsync(filterDefinition, skip, limit, withCount, sortField, sortDirection);
		}
		
		private IPaginationCollection<TEntity> Filter(
			FilterDefinition<TEntity> predicate, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null)
		{
			var collection =
				this.ExecuteFilter(predicate, skip, limit, orderBy, sortDirection);

			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = this.Collection.CountDocuments(predicate);
			}

			return new PaginationCollection<TEntity>
			{
				Count = totalCount,
				Items = collection.ToList()
			};
		}
		
		private async Task<IPaginationCollection<TEntity>> FilterAsync(
			FilterDefinition<TEntity> predicate, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null)
		{
			var collection =
				this.ExecuteFilter(predicate, skip, limit, orderBy, sortDirection);

			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = await this.Collection.CountDocumentsAsync(predicate);
			}

			return new PaginationCollection<TEntity>
			{
				Count = totalCount,
				Items = await collection.ToListAsync()
			};
		}
		
		private IFindFluent<TEntity, TEntity> ExecuteFilter(
			FilterDefinition<TEntity> predicate,
			int? skip = null,
			int? limit = null,
			string orderBy = null,
			SortDirection? sortDirection = null)
		{
			if (predicate == null)
			{
				predicate = new ExpressionFilterDefinition<TEntity>(item => true);
			}

			SortDefinition<TEntity> sortDefinition = null;
			if (!string.IsNullOrEmpty(orderBy))
			{
				SortDefinitionBuilder<TEntity> builder = new SortDefinitionBuilder<TEntity>();
				FieldDefinition<TEntity> fieldDefinition = new StringFieldDefinition<TEntity>(orderBy);
				sortDefinition = sortDirection == null || sortDirection.Value == SortDirection.Ascending ? builder.Ascending(fieldDefinition) : builder.Descending(fieldDefinition);	
			}

			IFindFluent<TEntity, TEntity> collection;
			if (sortDefinition == null)
			{
				if (skip != null && limit != null)
				{
					collection = this.Collection.Find(predicate).Skip(skip).Limit(limit);
				}
				else if (skip != null)
				{
					collection = this.Collection.Find(predicate).Skip(skip);
				}
				else if (limit != null)
				{
					collection = this.Collection.Find(predicate).Limit(limit);
				}
				else
				{
					collection = this.Collection.Find(predicate);	
				}
			}
			else
			{
				if (skip != null && limit != null)
				{
					collection = this.Collection.Find(predicate).Sort(sortDefinition).Skip(skip).Limit(limit);
				}
				else if (skip != null)
				{
					collection = this.Collection.Find(predicate).Sort(sortDefinition).Skip(skip);
				}
				else if (limit != null)
				{
					collection = this.Collection.Find(predicate).Sort(sortDefinition).Limit(limit);
				}
				else
				{
					collection = this.Collection.Find(predicate).Sort(sortDefinition);	
				}
			}

			return collection;
		}
		
		#endregion

		#region Query Methods

		public IPaginationCollection<dynamic> Query(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null)
		{
			try
			{
				var filterDefinition = new JsonFilterDefinition<TEntity>(query);
			
				var filterResult =
					this.ExecuteFilter(filterDefinition, skip, limit, sortField, sortDirection);

				var projectionDefinition = ExecuteSelectQuery<TEntity>(selectFields);
				var collection = filterResult.Project(projectionDefinition);
			
				long totalCount = 0;
				if (withCount != null && withCount.Value)
				{
					totalCount = this.Collection.CountDocuments(filterDefinition);
				}

				var documents = collection.ToList();
				var objects = documents.Select(BsonTypeMapper.MapToDotNetValue);

				return new PaginationCollection<dynamic>
				{
					Count = totalCount,
					Items = objects
				};	
			}
			catch (MongoCommandException ex)
			{
				switch (ex.Code)
				{
					case 31249:
						throw new SelectQueryPathCollisionException(ex);
					case 31254:
						throw new SelectQueryInclusionException(ex);
					default:
						throw;
				}
			}
		}
		
		public async Task<IPaginationCollection<dynamic>> QueryAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null)
		{
			try
			{
				var filterDefinition = new JsonFilterDefinition<TEntity>(query);
			
				var filterResult =
					this.ExecuteFilter(filterDefinition, skip, limit, sortField, sortDirection);

				var projectionDefinition = ExecuteSelectQuery<TEntity>(selectFields);
				var collection = filterResult.Project(projectionDefinition);
			
				long totalCount = 0;
				if (withCount != null && withCount.Value)
				{
					totalCount = await this.Collection.CountDocumentsAsync(filterDefinition);
				}

				var documents = await collection.ToListAsync();
				var objects = documents.Select(BsonTypeMapper.MapToDotNetValue);

				return new PaginationCollection<dynamic>
				{
					Count = totalCount,
					Items = objects
				};	
			}
			catch (MongoCommandException ex)
			{
				switch (ex.Code)
				{
					case 31249:
						throw new SelectQueryPathCollisionException(ex);
					case 31254:
						throw new SelectQueryInclusionException(ex);
					default:
						throw;
				}
			}
		}

		#endregion
		
		#region Select Methods

		private static ProjectionDefinition<T> ExecuteSelectQuery<T>(IDictionary<string, bool> selectFields)
		{
			if (selectFields != null && selectFields.Any())
			{
				var selectDefinition = Builders<T>.Projection.Include("_id");
				var includedFields = selectFields.Where(x => x.Value);
				selectDefinition = includedFields.Aggregate(selectDefinition, (current, field) => current.Include(field.Key));
				var excludedFields = selectFields.Where(x => !x.Value);
				selectDefinition = excludedFields.Aggregate(selectDefinition, (current, field) => current.Exclude(field.Key));
				
				return selectDefinition;
			}
			
			return new ObjectProjectionDefinition<T>(new object());
		}

		#endregion
		
		#region Insert Methods

		public TEntity Insert(TEntity entity)
		{
			this.Collection.InsertOne(entity);
			return entity;
		}
		
		public async Task<TEntity> InsertAsync(TEntity entity)
		{
			await this.Collection.InsertOneAsync(entity);
			return entity;
		}

		#endregion
		
		#region Update Methods

		public TEntity Update(TEntity entity)
		{
			this.Collection.ReplaceOne(item => item.Id == entity.Id, entity);
			return entity;
		}
		
		public async Task<TEntity> UpdateAsync(TEntity entity)
		{
			await this.Collection.ReplaceOneAsync(item => item.Id == entity.Id, entity);
			return entity;
		}

		public TEntity Upsert(TEntity entity)
		{
			var item = this.Find(entity.Id);
			if (item == null)
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
			var item = await this.FindAsync(entity.Id);
			if (item == null)
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

		public bool Delete(string id)
		{
			var result = this.Collection.DeleteOne(item => item.Id == id);
			return result.IsAcknowledged && result.DeletedCount == 1;
		}
		
		public async Task<bool> DeleteAsync(string id)
		{
			var result = await this.Collection.DeleteOneAsync(item => item.Id == id);
			return result.IsAcknowledged && result.DeletedCount == 1;
		}

		#endregion
		
		#region Count Methods

		public long Count()
		{
			return this.Collection.CountDocuments(item => true);
		}
		
		public async Task<long> CountAsync()
		{
			return await this.Collection.CountDocumentsAsync(item => true);
		}
		
		public long Count(Expression<Func<TEntity, bool>> expression)
		{
			FilterDefinition<TEntity> filterExpression = new ExpressionFilterDefinition<TEntity>(expression);
			return this.Collection.CountDocuments(filterExpression);
		}
		
		public async Task<long> CountAsync(Expression<Func<TEntity, bool>> expression)
		{
			FilterDefinition<TEntity> filterExpression = new ExpressionFilterDefinition<TEntity>(expression);
			return await this.Collection.CountDocumentsAsync(filterExpression);
		}
		
		public long Count(string query)
		{
			var filterDefinition = new JsonFilterDefinition<TEntity>(query);
			return this.Collection.CountDocuments(filterDefinition);
		}
		
		public async Task<long> CountAsync(string query)
		{
			var filterDefinition = new JsonFilterDefinition<TEntity>(query);
			return await this.Collection.CountDocumentsAsync(filterDefinition);
		}
		
		#endregion
	}
}