using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Repository;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Helpers;
using Ertis.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using SortDirection = Ertis.Core.Collections.SortDirection;
using MongoDriver = MongoDB.Driver;

namespace Ertis.MongoDB.Repository
{
    public abstract class DynamicMongoRepository : IDynamicMongoRepository
	{
		#region Services

		private readonly IRepositoryActionBinder _actionBinder;

		#endregion
		
		#region Properties
		
		private IMongoCollection<dynamic> Collection { get; }
		
		private IMongoCollection<BsonDocument> DocumentCollection { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="collectionName"></param>
		/// <param name="actionBinder"></param>
		protected DynamicMongoRepository(IDatabaseSettings settings, string collectionName, IRepositoryActionBinder actionBinder = null)
		{
			var connectionString = ConnectionStringHelper.GenerateConnectionString(settings);
			var client = new MongoClient(connectionString);
			var database = client.GetDatabase(settings.DefaultAuthDatabase);

			this.Collection = database.GetCollection<dynamic>(collectionName);
			this.DocumentCollection = database.GetCollection<BsonDocument>(collectionName);

			this._actionBinder = actionBinder;
		}

		#endregion
		
		#region Find Methods

		public dynamic FindOne(string id)
		{
			return this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefault();
		}
		
		// ReSharper disable once UnusedMember.Local
		private dynamic FindOne(ObjectId objectId)
		{
			return this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", objectId)).FirstOrDefault();
		}
		
		public async ValueTask<dynamic> FindOneAsync(string id, CancellationToken cancellationToken = default)
		{
			return await this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefaultAsync(cancellationToken);
		}
		
		// ReSharper disable once UnusedMember.Local
		private async ValueTask<dynamic> FindOneAsync(ObjectId objectId, CancellationToken cancellationToken = default)
		{
			return await this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync(cancellationToken);
		}

		public dynamic FindOne(Expression<Func<dynamic, bool>> expression)
		{
			var filterDefinition = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return this.Collection.Find(filterDefinition).FirstOrDefault();
		}

		public async ValueTask<dynamic> FindOneAsync(Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default)
		{
			var filterDefinition = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return await (await this.Collection.FindAsync(filterDefinition, cancellationToken: cancellationToken)).FirstOrDefaultAsync(cancellationToken);	
		}

		public IPaginationCollection<dynamic> Find(
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

		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(
				expression: null,
				skip,
				limit,
				withCount,
				sortField,
				sortDirection,
				cancellationToken);
		}
		
		public IPaginationCollection<dynamic> Find(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return this.Filter(filterExpression, skip, limit, withCount, sortField, sortDirection);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return await this.FilterAsync(filterExpression, skip, limit, withCount, sortField, sortDirection, cancellationToken);
		}
		
		public IPaginationCollection<dynamic> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			return this.Filter(filterDefinition, skip, limit, withCount, sortField, sortDirection);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			return await this.FilterAsync(filterDefinition, skip, limit, withCount, sortField, sortDirection, cancellationToken);
		}
		
		private IPaginationCollection<dynamic> Filter(
			FilterDefinition<dynamic> predicate, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null)
		{
			var collection = this.ExecuteFilter(predicate, skip, limit, orderBy, sortDirection);

			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = this.Collection.CountDocuments(predicate);
			}

			return new PaginationCollection<dynamic>
			{
				Count = totalCount,
				Items = collection.ToList()
			};
		}
		
		private async Task<IPaginationCollection<dynamic>> FilterAsync(
			FilterDefinition<dynamic> predicate, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var collection = this.ExecuteFilter(predicate, skip, limit, orderBy, sortDirection);

			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = await this.Collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
			}

			return new PaginationCollection<dynamic>
			{
				Count = totalCount,
				Items = await collection.ToListAsync(cancellationToken)
			};
		}
		
		private IFindFluent<dynamic, dynamic> ExecuteFilter(
			FilterDefinition<dynamic> predicate,
			int? skip = null,
			int? limit = null,
			string orderBy = null,
			SortDirection? sortDirection = null)
		{
			predicate ??= new ExpressionFilterDefinition<dynamic>(item => true);

			SortDefinition<dynamic> sortDefinition = null;
			if (!string.IsNullOrEmpty(orderBy))
			{
				var builder = new SortDefinitionBuilder<dynamic>();
				FieldDefinition<dynamic> fieldDefinition = new StringFieldDefinition<dynamic>(orderBy);
				sortDefinition = sortDirection == null || sortDirection.Value == SortDirection.Ascending ? builder.Ascending(fieldDefinition) : builder.Descending(fieldDefinition);	
			}

			IFindFluent<dynamic, dynamic> collection;
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
				query = QueryHelper.EnsureObjectIdsAndISODates(query);
				var filterDefinition = new JsonFilterDefinition<dynamic>(query);
				return this.ExecuteQuery(
					filterDefinition,
					skip,
					limit,
					withCount,
					sortField,
					sortDirection,
					selectFields);
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

		public IPaginationCollection<dynamic> Query(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null)
		{
			try
			{
				var filterDefinition = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
				return this.ExecuteQuery(
					filterDefinition,
					skip,
					limit,
					withCount,
					sortField,
					sortDirection,
					selectFields);
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
		
		public async ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null, 
			CancellationToken cancellationToken = default)
		{
			try
			{
				query = QueryHelper.EnsureObjectIdsAndISODates(query);
				var filterDefinition = new JsonFilterDefinition<dynamic>(query);
				return await this.ExecuteQueryAsync(
					filterDefinition,
					skip,
					limit,
					withCount,
					sortField,
					sortDirection,
					selectFields,
					cancellationToken);
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

		public async ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null, 
			CancellationToken cancellationToken = default)
		{
			try
			{
				var filterDefinition = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
				return await this.ExecuteQueryAsync(
					filterDefinition,
					skip,
					limit,
					withCount,
					sortField,
					sortDirection,
					selectFields,
					cancellationToken);
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

		private IPaginationCollection<dynamic> ExecuteQuery(
			FilterDefinition<dynamic> filterDefinition,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null)
		{
			try
			{
				var filterResult = this.ExecuteFilter(filterDefinition, skip, limit, sortField, sortDirection);
				var projectionDefinition = ExecuteSelectQuery<dynamic>(selectFields);
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
		
		private async Task<IPaginationCollection<dynamic>> ExecuteQueryAsync(
			FilterDefinition<dynamic> filterDefinition,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null, 
			CancellationToken cancellationToken = default)
		{
			try
			{
				var filterResult = this.ExecuteFilter(filterDefinition, skip, limit, sortField, sortDirection);
				var projectionDefinition = ExecuteSelectQuery<dynamic>(selectFields);
				var collection = filterResult.Project(projectionDefinition);
			
				long totalCount = 0;
				if (withCount != null && withCount.Value)
				{
					totalCount = await this.Collection.CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken);
				}

				var documents = await collection.ToListAsync(cancellationToken);
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

		public dynamic Insert(object entity)
		{
			if (this._actionBinder != null)
			{
				entity = this._actionBinder.BeforeInsert(entity);
			}

			if (entity is BsonDocument document)
			{
				this.DocumentCollection.InsertOne(document);
			}
			else
			{
				this.Collection.InsertOne(entity);	
			}

			if (this._actionBinder != null)
			{
				entity = this._actionBinder.AfterInsert(entity);
			}
			
			return entity;
		}
		
		public async ValueTask<dynamic> InsertAsync(object entity, CancellationToken cancellationToken = default)
		{
			if (this._actionBinder != null)
			{
				entity = this._actionBinder.BeforeInsert(entity);
			}
			
			if (entity is BsonDocument document)
			{
				await this.DocumentCollection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);
			}
			else
			{
				await this.Collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);	
			}

			if (this._actionBinder != null)
			{
				entity = this._actionBinder.AfterInsert(entity);
			}
			
			return entity;
		}

		public void BulkInsert(IEnumerable<object> entities)
		{
			var enumerable = entities as object[] ?? entities.ToArray();
			
			if (this._actionBinder != null)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			this.Collection.InsertMany(enumerable);

			if (this._actionBinder != null)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.AfterInsert(entity);	
				}
			}
		}

		public async ValueTask BulkInsertAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
		{
			var enumerable = entities as object[] ?? entities.ToArray();
			
			if (this._actionBinder != null)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			await this.Collection.InsertManyAsync(enumerable, cancellationToken: cancellationToken);

			if (this._actionBinder != null)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.AfterInsert(entity);	
				}
			}
		}

		public ICollection<dynamic> InsertMany(ICollection<object> entities)
		{
			if (this._actionBinder != null)
			{
				foreach (var entity in entities)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			this.Collection.InsertMany(entities);
			
			if (this._actionBinder != null)
			{
				foreach (var entity in entities)
				{
					this._actionBinder.AfterInsert(entity);	
				}
			}
			
			return entities;
		}
		
		public async Task<ICollection<dynamic>> InsertManyAsync(ICollection<object> entities, CancellationToken cancellationToken = default)
		{
			if (this._actionBinder != null)
			{
				foreach (var entity in entities)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			await this.Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
			
			if (this._actionBinder != null)
			{
				foreach (var entity in entities)
				{
					this._actionBinder.AfterInsert(entity);	
				}
			}
			
			return entities;
		}

		#endregion
		
		#region Update Methods

		public dynamic Update(object entity, string id = default)
		{
			if (this._actionBinder != null)
			{
				entity = this._actionBinder.BeforeUpdate(entity);
			}

			if (entity is BsonDocument document)
			{
				this.DocumentCollection.ReplaceOne(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id)), document);
			}
			else
			{
				this.Collection.ReplaceOne(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id)), entity);	
			}

			if (this._actionBinder != null)
			{
				entity = this._actionBinder.AfterUpdate(entity);
			}
			
			return entity;
		}
		
		public async ValueTask<dynamic> UpdateAsync(object entity, string id = default, CancellationToken cancellationToken = default)
		{
			if (this._actionBinder != null)
			{
				entity = this._actionBinder.BeforeUpdate(entity);
			}

			if (entity is BsonDocument document)
			{
				await this.DocumentCollection.ReplaceOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id)), document, cancellationToken: cancellationToken);
			}
			else
			{
				await this.Collection.ReplaceOneAsync(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id)), entity, cancellationToken: cancellationToken);	
			}

			if (this._actionBinder != null)
			{
				entity = this._actionBinder.AfterUpdate(entity);
			}
			
			return entity;
		}

		[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
		public dynamic Upsert(dynamic entity, string id = default)
		{
			if (string.IsNullOrEmpty(id))
			{
				return this.Insert(entity);
			}
			else
			{
				var item = this.FindOne(id);
				return item == null ? this.Insert(entity) : this.Update(entity, id);
			}
		}
		
		[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
		public async ValueTask<dynamic> UpsertAsync(dynamic entity, string id = default, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(id))
			{
				return await this.InsertAsync(entity, cancellationToken);
			}
			else
			{
				var item = await this.FindOneAsync(id, cancellationToken);
				return item == null ? await this.InsertAsync(entity, cancellationToken) : await this.UpdateAsync(entity, id, cancellationToken);
			}
		}

		#endregion
		
		#region Delete Methods

		public bool Delete(string id)
		{
			var result = this.Collection.DeleteOne(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id)));
			return result.IsAcknowledged && result.DeletedCount == 1;
		}
		
		public async ValueTask<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
		{
			var result = await this.Collection.DeleteOneAsync(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id)), cancellationToken);
			return result.IsAcknowledged && result.DeletedCount == 1;
		}

		public bool BulkDelete(IEnumerable<dynamic> entities)
		{
			return entities.Aggregate(true, (current, entity) => (bool) (current & this.Delete(entity)));
		}

		[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
		public async ValueTask<bool> BulkDeleteAsync(IEnumerable<dynamic> entities, CancellationToken cancellationToken = default)
		{
			var isDeletedAll = true;
			foreach (var entity in entities)
			{
				isDeletedAll &= await this.DeleteAsync(entity, cancellationToken);
			}

			return isDeletedAll;
		}

		#endregion
		
		#region Count Methods

		public long Count()
		{
			return this.Collection.CountDocuments(item => true);
		}
		
		public async ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
		{
			return await this.Collection.CountDocumentsAsync(item => true, cancellationToken: cancellationToken);
		}
		
		public long Count(Expression<Func<dynamic, bool>> expression)
		{
			FilterDefinition<dynamic> filterExpression = new ExpressionFilterDefinition<dynamic>(expression);
			return this.Collection.CountDocuments(filterExpression);
		}
		
		public async ValueTask<long> CountAsync(Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default)
		{
			FilterDefinition<dynamic> filterExpression = new ExpressionFilterDefinition<dynamic>(expression);
			return await this.Collection.CountDocumentsAsync(filterExpression, cancellationToken: cancellationToken);
		}
		
		public long Count(string query)
		{
			var filterDefinition = new JsonFilterDefinition<dynamic>(query);
			return this.Collection.CountDocuments(filterDefinition);
		}
		
		public async ValueTask<long> CountAsync(string query, CancellationToken cancellationToken = default)
		{
			var filterDefinition = new JsonFilterDefinition<dynamic>(query);
			return await this.Collection.CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken);
		}
		
		#endregion

		#region Aggregation Methods

		public dynamic Aggregate(string query)
		{
			try
			{
				query = QueryHelper.EnsureObjectIdsAndISODates(query);
				
				using (var jsonReader = new JsonReader(query))
				{
					var serializer = new BsonArraySerializer();
					var bsonArray = serializer.Deserialize(BsonDeserializationContext.CreateRoot(jsonReader));
					var bsonDocuments = bsonArray.Select(x => BsonDocument.Parse(x.ToString()));
					var pipelineDefinition = PipelineDefinition<dynamic, BsonDocument>.Create(bsonDocuments);
					var aggregationResultCursor = this.Collection.Aggregate(pipelineDefinition);
					var documents = aggregationResultCursor.ToList();
					var objects = documents.Select(BsonTypeMapper.MapToDotNetValue);
					return objects;
				}
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
		
		public async ValueTask<dynamic> AggregateAsync(string query, CancellationToken cancellationToken = default)
		{
			try
			{
				query = QueryHelper.EnsureObjectIdsAndISODates(query);
				
				using (var jsonReader = new JsonReader(query))
				{
					var serializer = new BsonArraySerializer();
					var bsonArray = serializer.Deserialize(BsonDeserializationContext.CreateRoot(jsonReader));
					var bsonDocuments = bsonArray.Select(x => BsonDocument.Parse(x.ToString()));
					var pipelineDefinition = PipelineDefinition<dynamic, BsonDocument>.Create(bsonDocuments);
					var aggregationResultCursor = await this.Collection.AggregateAsync(pipelineDefinition, cancellationToken: cancellationToken);
					var documents = await aggregationResultCursor.ToListAsync(cancellationToken);
					var objects = documents.Select(BsonTypeMapper.MapToDotNetValue);
					return objects;
				}
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
		
		#region Index Methods

		public async Task<IEnumerable<IIndexDefinition>> GetIndexesAsync(CancellationToken cancellationToken = default)
		{
			var indexesCursor = await this.Collection.Indexes.ListAsync(cancellationToken);
			var indexes = await indexesCursor.ToListAsync(cancellationToken);
			var indexDefinitions = new List<IIndexDefinition>();
			foreach (var index in indexes)
			{
				if (index.Contains("key") && index["key"].IsBsonDocument)
				{
					var nodes = index["key"].AsBsonDocument.Elements.ToArray();
					if (nodes.Any())
					{
						if (nodes.Length == 1)
						{
							// Single
							var node = nodes[0];
							indexDefinitions.Add(new SingleIndexDefinition(node.Name,
								node.Value.IsInt32
									? (node.Value.AsInt32 == -1
										? SortDirection.Descending
										: SortDirection.Ascending)
									: null));
						}
						else
						{
							// Compound
							var subIndexDefinitions = new List<SingleIndexDefinition>();
							foreach (var node in nodes)
							{
								subIndexDefinitions.Add(new SingleIndexDefinition(node.Name,
									node.Value.IsInt32
										? (node.Value.AsInt32 == -1
											? SortDirection.Descending
											: SortDirection.Ascending)
										: null));
							}
							
							indexDefinitions.Add(new CompoundIndexDefinition(subIndexDefinitions.ToArray()));
						}
					}
					else
					{
						return Enumerable.Empty<IIndexDefinition>();
					}
				}
			}
			
			return indexDefinitions;
		}
		
		public async Task<string> CreateIndexAsync(IIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
		{
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			// ReSharper disable once ConvertSwitchStatementToSwitchExpression
			switch (indexDefinition.Type)
			{
				case IndexType.Single:
					return await this.CreateSingleIndexAsync(indexDefinition as SingleIndexDefinition, cancellationToken);
				case IndexType.Compound:
					return await this.CreateCompoundIndexAsync(indexDefinition as CompoundIndexDefinition, cancellationToken);
				default:
					throw new NotImplementedException("Not implemented yet for this index type");
			}
		}

		public async Task<string[]> CreateManyIndexAsync(IEnumerable<IIndexDefinition> indexDefinitions, CancellationToken cancellationToken = default)
		{
			var results = new List<string>();
			foreach (var indexDefinition in indexDefinitions)
			{
				results.Add(await this.CreateIndexAsync(indexDefinition, cancellationToken));
			}

			return results.ToArray();
		}
		
		public async Task<string> CreateSingleIndexAsync(string fieldName, SortDirection? direction = null, CancellationToken cancellationToken = default)
		{
			var indexKeysDefinition = direction is SortDirection.Descending ?
				Builders<dynamic>.IndexKeys.Descending(fieldName) :
				Builders<dynamic>.IndexKeys.Ascending(fieldName);
			
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<dynamic>(indexKeysDefinition), cancellationToken: cancellationToken);
		}
		
		public async Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
		{
			return await this.CreateSingleIndexAsync(indexDefinition.Field, indexDefinition.Direction, cancellationToken);
		}
		
		public async Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default)
		{
			var indexKeyDefinitions = new List<IndexKeysDefinition<dynamic>>();
			foreach (var (fieldName, direction) in indexFieldDefinitions)
			{
				indexKeyDefinitions.Add(direction is SortDirection.Descending ?
					Builders<dynamic>.IndexKeys.Descending(fieldName) :
					Builders<dynamic>.IndexKeys.Ascending(fieldName));
			}
			
			var combinedIndexDefinition = Builders<dynamic>.IndexKeys.Combine(indexKeyDefinitions);
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<dynamic>(combinedIndexDefinition), cancellationToken: cancellationToken);
		}
		
		public async Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
		{
			var indexKeyDefinitions = new List<IndexKeysDefinition<dynamic>>();
			foreach (var index in indexDefinition.Indexes)
			{
				indexKeyDefinitions.Add(index.Direction is SortDirection.Descending ?
					Builders<dynamic>.IndexKeys.Descending(index.Field) :
					Builders<dynamic>.IndexKeys.Ascending(index.Field));
			}
			
			var combinedIndexDefinition = Builders<dynamic>.IndexKeys.Combine(indexKeyDefinitions);
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<dynamic>(combinedIndexDefinition), cancellationToken: cancellationToken);
		}

		#endregion
	}
}