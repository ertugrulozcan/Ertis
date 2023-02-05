using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.Data.Repository;
using Ertis.MongoDB.Attributes;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Helpers;
using Ertis.MongoDB.Models;
using Ertis.MongoDB.Queries;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SortDirection = Ertis.Core.Collections.SortDirection;
using MongoDriver = MongoDB.Driver;
using UpdateOptions = Ertis.Data.Models.UpdateOptions;

namespace Ertis.MongoDB.Repository
{
	public abstract class MongoRepositoryBase<TEntity> : IMongoRepository<TEntity> where TEntity : IEntity<string>
	{
		#region Services

		private readonly IRepositoryActionBinder actionBinder;

		#endregion
		
		#region Properties

		private IMongoCollection<TEntity> Collection { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="collectionName"></param>
		/// <param name="actionBinder"></param>
		protected MongoRepositoryBase(IDatabaseSettings settings, string collectionName, IRepositoryActionBinder actionBinder = null)
		{
			var connectionString = ConnectionStringHelper.GenerateConnectionString(settings);
			var client = new MongoClient(connectionString);
			var database = client.GetDatabase(settings.DefaultAuthDatabase);

			this.Collection = database.GetCollection<TEntity>(collectionName);
			this.CreateSearchIndexesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

			this.actionBinder = actionBinder;
		}

		#endregion

		#region Index Methods
		
		public async Task<IEnumerable<IIndexDefinition>> GetIndexesAsync(CancellationToken cancellationToken = default)
		{
			var indexesCursor = await this.Collection.Indexes.ListAsync(cancellationToken: cancellationToken);
			var indexes = await indexesCursor.ToListAsync(cancellationToken: cancellationToken);
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
					return await this.CreateSingleIndexAsync(indexDefinition as SingleIndexDefinition, cancellationToken: cancellationToken);
				case IndexType.Compound:
					return await this.CreateCompoundIndexAsync(indexDefinition as CompoundIndexDefinition, cancellationToken: cancellationToken);
				default:
					throw new NotImplementedException("Not implemented yet for this index type");
			}
		}

		public async Task<string[]> CreateManyIndexAsync(IEnumerable<IIndexDefinition> indexDefinitions, CancellationToken cancellationToken = default)
		{
			var results = new List<string>();
			foreach (var indexDefinition in indexDefinitions)
			{
				results.Add(await this.CreateIndexAsync(indexDefinition, cancellationToken: cancellationToken));
			}

			return results.ToArray();
		}
		
		public async Task<string> CreateSingleIndexAsync(string fieldName, SortDirection? direction = null, CancellationToken cancellationToken = default)
		{
			var indexKeysDefinition = direction is SortDirection.Descending ?
				Builders<TEntity>.IndexKeys.Descending(fieldName) :
				Builders<TEntity>.IndexKeys.Ascending(fieldName);
			
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<TEntity>(indexKeysDefinition), cancellationToken: cancellationToken);
		}
		
		public async Task<string> CreateSingleIndexAsync(Expression<Func<TEntity, object>> expression, SortDirection? direction = null, CancellationToken cancellationToken = default)
		{
			var indexKeysDefinition = direction is SortDirection.Descending ?
				Builders<TEntity>.IndexKeys.Descending(expression) :
				Builders<TEntity>.IndexKeys.Ascending(expression);
			
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<TEntity>(indexKeysDefinition), cancellationToken: cancellationToken);
		}

		public async Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
		{
			return await this.CreateSingleIndexAsync(indexDefinition.Field, indexDefinition.Direction, cancellationToken: cancellationToken);
		}
		
		public async Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default)
		{
			var indexKeyDefinitions = new List<IndexKeysDefinition<TEntity>>();
			foreach (var (fieldName, direction) in indexFieldDefinitions)
			{
				indexKeyDefinitions.Add(direction is SortDirection.Descending ?
					Builders<TEntity>.IndexKeys.Descending(fieldName) :
					Builders<TEntity>.IndexKeys.Ascending(fieldName));
			}
			
			var combinedIndexDefinition = Builders<TEntity>.IndexKeys.Combine(indexKeyDefinitions);
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<TEntity>(combinedIndexDefinition), cancellationToken: cancellationToken);
		}
		
		public async Task<string> CreateCompoundIndexAsync(IDictionary<Expression<Func<TEntity, object>>, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default)
		{
			var indexKeyDefinitions = new List<IndexKeysDefinition<TEntity>>();
			foreach (var (expression, direction) in indexFieldDefinitions)
			{
				indexKeyDefinitions.Add(direction is SortDirection.Descending ?
					Builders<TEntity>.IndexKeys.Descending(expression) :
					Builders<TEntity>.IndexKeys.Ascending(expression));
			}
			
			var combinedIndexDefinition = Builders<TEntity>.IndexKeys.Combine(indexKeyDefinitions);
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<TEntity>(combinedIndexDefinition), cancellationToken: cancellationToken);
		}

		public async Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
		{
			var indexKeyDefinitions = new List<IndexKeysDefinition<TEntity>>();
			foreach (var index in indexDefinition.Indexes)
			{
				indexKeyDefinitions.Add(index.Direction is SortDirection.Descending ?
					Builders<TEntity>.IndexKeys.Descending(index.Field) :
					Builders<TEntity>.IndexKeys.Ascending(index.Field));
			}
			
			var combinedIndexDefinition = Builders<TEntity>.IndexKeys.Combine(indexKeyDefinitions);
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<TEntity>(combinedIndexDefinition), cancellationToken: cancellationToken);
		}

		private async Task CreateSearchIndexesAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				var currentIndexesCursor = await this.Collection.Indexes.ListAsync(cancellationToken: cancellationToken);
				var currentIndexes = await currentIndexesCursor.ToListAsync(cancellationToken: cancellationToken);
				var currentTextIndexes = currentIndexes.Where(x =>
					x.Contains("key") &&
					x["key"].IsBsonDocument &&
					x["key"].AsBsonDocument.Contains("_fts") &&
					x["key"].AsBsonDocument["_fts"].IsString &&
					x["key"].AsBsonDocument["_fts"].AsString == "text");

				var indexedPropertyNames = currentTextIndexes.SelectMany(x => x["weights"].AsBsonDocument.Names).ToArray();
				var nonIndexedPropertyNames = new List<string>();
			
				var propertyInfos = typeof(TEntity).GetProperties();
				foreach (var propertyInfo in propertyInfos)
				{
					var searchableAttribute = propertyInfo.GetCustomAttribute(typeof(SearchableAttribute), true);
					if (searchableAttribute is SearchableAttribute)
					{
						var attribute = propertyInfo.GetCustomAttribute(typeof(BsonElementAttribute), true);
						if (attribute is BsonElementAttribute bsonElementAttribute)
						{
							if (!indexedPropertyNames.Contains(bsonElementAttribute.ElementName))
							{
								nonIndexedPropertyNames.Add(bsonElementAttribute.ElementName);
							}
						}
					}
				}

				if (nonIndexedPropertyNames.Any())
				{
					var combinedTextIndexDefinition = Builders<TEntity>.IndexKeys.Combine(
						nonIndexedPropertyNames.Select(x => Builders<TEntity>.IndexKeys.Text(x)));
				
					await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<TEntity>(combinedTextIndexDefinition), cancellationToken: cancellationToken);	
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occured while creating search indexes for '{typeof(TEntity).Name}' entity type;");
				Console.WriteLine(ex);
			}
		}

		#endregion
		
		#region Find Methods

		public TEntity FindOne(string id)
		{
			return this.Collection.Find(item => item.Id == id).FirstOrDefault();
		}
		
		public async ValueTask<TEntity> FindOneAsync(string id, CancellationToken cancellationToken = default)
		{
			return await this.Collection.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
		}

		public TEntity FindOne(Expression<Func<TEntity, bool>> expression)
		{
			FilterDefinition<TEntity> filterDefinition = expression != null ? new ExpressionFilterDefinition<TEntity>(expression) : FilterDefinition<TEntity>.Empty;
			return this.Collection.Find(filterDefinition).FirstOrDefault();
		}

		public async ValueTask<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
		{
			FilterDefinition<TEntity> filterDefinition = expression != null ? new ExpressionFilterDefinition<TEntity>(expression) : FilterDefinition<TEntity>.Empty;
			return await (await this.Collection.FindAsync(filterDefinition, cancellationToken: cancellationToken)).FirstOrDefaultAsync(cancellationToken: cancellationToken);	
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

		public async ValueTask<IPaginationCollection<TEntity>> FindAsync(
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
				cancellationToken: cancellationToken);
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
		
		public async ValueTask<IPaginationCollection<TEntity>> FindAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			FilterDefinition<TEntity> filterExpression = expression != null ? new ExpressionFilterDefinition<TEntity>(expression) : FilterDefinition<TEntity>.Empty;
			return await this.FilterAsync(filterExpression, skip, limit, withCount, sortField, sortDirection, cancellationToken: cancellationToken);
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
		
		public async ValueTask<IPaginationCollection<TEntity>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<TEntity>.Empty : new JsonFilterDefinition<TEntity>(query);
			return await this.FilterAsync(filterDefinition, skip, limit, withCount, sortField, sortDirection, cancellationToken: cancellationToken);
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
		
		private async ValueTask<IPaginationCollection<TEntity>> FilterAsync(
			FilterDefinition<TEntity> predicate, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var collection =
				this.ExecuteFilter(predicate, skip, limit, orderBy, sortDirection);

			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = await this.Collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
			}

			return new PaginationCollection<TEntity>
			{
				Count = totalCount,
				Items = await collection.ToListAsync(cancellationToken: cancellationToken)
			};
		}
		
		private IFindFluent<TEntity, TEntity> ExecuteFilter(
			FilterDefinition<TEntity> predicate,
			int? skip = null,
			int? limit = null,
			string orderBy = null,
			SortDirection? sortDirection = null)
		{
			predicate ??= new ExpressionFilterDefinition<TEntity>(item => true);

			SortDefinition<TEntity> sortDefinition = null;
			if (!string.IsNullOrEmpty(orderBy))
			{
				SortDefinitionBuilder<TEntity> builder = new SortDefinitionBuilder<TEntity>();
				FieldDefinition<TEntity> fieldDefinition = new StringFieldDefinition<TEntity>(orderBy);
				sortDefinition = sortDirection is null or SortDirection.Ascending ? builder.Ascending(fieldDefinition) : builder.Descending(fieldDefinition);	
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
				query = QueryHelper.EnsureObjectIdsAndISODates(query);
				var filterDefinition = new JsonFilterDefinition<TEntity>(query);
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
			Expression<Func<TEntity, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null)
		{
			try
			{
				FilterDefinition<TEntity> filterDefinition = expression != null ? new ExpressionFilterDefinition<TEntity>(expression) : FilterDefinition<TEntity>.Empty;
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
				var filterDefinition = new JsonFilterDefinition<TEntity>(query);
				return await this.ExecuteQueryAsync(
					filterDefinition,
					skip,
					limit,
					withCount,
					sortField,
					sortDirection,
					selectFields,
					cancellationToken: cancellationToken);
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
			Expression<Func<TEntity, bool>> expression,
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
				FilterDefinition<TEntity> filterDefinition = expression != null ? new ExpressionFilterDefinition<TEntity>(expression) : FilterDefinition<TEntity>.Empty;
				return await this.ExecuteQueryAsync(
					filterDefinition,
					skip,
					limit,
					withCount,
					sortField,
					sortDirection,
					selectFields,
					cancellationToken: cancellationToken);
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
			FilterDefinition<TEntity> filterDefinition,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string sortField = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null)
		{
			try
			{
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
		
		private async ValueTask<IPaginationCollection<dynamic>> ExecuteQueryAsync(
			FilterDefinition<TEntity> filterDefinition,
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
				var filterResult =
					this.ExecuteFilter(filterDefinition, skip, limit, sortField, sortDirection);

				var projectionDefinition = ExecuteSelectQuery<TEntity>(selectFields);
				var collection = filterResult.Project(projectionDefinition);
			
				long totalCount = 0;
				if (withCount != null && withCount.Value)
				{
					totalCount = await this.Collection.CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken);
				}

				var documents = await collection.ToListAsync(cancellationToken: cancellationToken);
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

		#region Aggregation Methods

		public dynamic Aggregate(string aggregationStagesJson)
		{
			try
			{
				var jArray = Newtonsoft.Json.Linq.JArray.Parse(aggregationStagesJson);
				var bsonDocuments = jArray.Select(x => BsonDocument.Parse(x.ToString()));
				var pipelineDefinition = PipelineDefinition<TEntity, BsonDocument>.Create(bsonDocuments);
				var aggregationResultCursor = this.Collection.Aggregate(pipelineDefinition);
				var documents = aggregationResultCursor.ToList();
				var objects = documents.Select(BsonTypeMapper.MapToDotNetValue);
				return objects;
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
		
		public async ValueTask<dynamic> AggregateAsync(string aggregationStagesJson, CancellationToken cancellationToken = default)
		{
			try
			{
				var jArray = Newtonsoft.Json.Linq.JArray.Parse(aggregationStagesJson);
				var bsonDocuments = jArray.Select(x => BsonDocument.Parse(x.ToString()));
				var pipelineDefinition = PipelineDefinition<TEntity, BsonDocument>.Create(bsonDocuments);
				var aggregationResultCursor = await this.Collection.AggregateAsync(pipelineDefinition, cancellationToken: cancellationToken);
				var documents = await aggregationResultCursor.ToListAsync(cancellationToken: cancellationToken);
				var objects = documents.Select(BsonTypeMapper.MapToDotNetValue);
				return objects;
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

		#region Search Methods

		public IPaginationCollection<TEntity> Search(
			string keyword, 
			Queries.TextSearchOptions options = null,
			int? skip = null, 
			int? limit = null,
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null)
		{
			var query = options != null ? QueryBuilder.FullTextSearch(keyword, options.Language.ISO6391Code, options.IsCaseSensitive, options.IsDiacriticSensitive) : QueryBuilder.FullTextSearch(keyword);
			var queryResults = this.Query(query.ToString(), skip, limit, withCount, sortField, sortDirection);
			return new PaginationCollection<TEntity>
			{
				Count = queryResults.Count,
				Items = queryResults.Items
					.Cast<Dictionary<string, object>>()
					.Select(x => new BsonDocument(x))
					.Select(x => BsonSerializer.Deserialize<TEntity>(x))
			};
		}

		public async ValueTask<IPaginationCollection<TEntity>> SearchAsync(
			string keyword, 
			Queries.TextSearchOptions options = null,
			int? skip = null,
			int? limit = null, 
			bool? withCount = null, 
			string sortField = null, 
			SortDirection? sortDirection = null, 
			CancellationToken cancellationToken = default)
		{
			var query = options != null ? QueryBuilder.FullTextSearch(keyword, options.Language.ISO6391Code, options.IsCaseSensitive, options.IsDiacriticSensitive) : QueryBuilder.FullTextSearch(keyword);
			var queryResults = await this.QueryAsync(query.ToString(), skip, limit, withCount, sortField, sortDirection, cancellationToken: cancellationToken);
			return new PaginationCollection<TEntity>
			{
				Count = queryResults.Count,
				Items = queryResults.Items
					.Cast<Dictionary<string, object>>()
					.Select(x => new BsonDocument(x))
					.Select(x => BsonSerializer.Deserialize<TEntity>(x))
			};
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

		public TEntity Insert(TEntity entity, InsertOptions? options = null)
		{
			if (this.actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
			{
				entity = this.actionBinder.BeforeInsert(entity);
			}
			
			this.Collection.InsertOne(entity);
			
			if (this.actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
			{
				entity = this.actionBinder.AfterInsert(entity);
			}
			
			return entity;
		}
		
		public async ValueTask<TEntity> InsertAsync(TEntity entity, InsertOptions? options = null, CancellationToken cancellationToken = default)
		{
			if (this.actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
			{
				entity = this.actionBinder.BeforeInsert(entity);
			}
			
			await this.Collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken: cancellationToken);
			
			if (this.actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
			{
				entity = this.actionBinder.AfterInsert(entity);
			}
			
			return entity;
		}

		public void BulkInsert(IEnumerable<TEntity> entities, InsertOptions? options = null)
		{
			this.Collection.InsertMany(entities);
		}

		public async ValueTask BulkInsertAsync(IEnumerable<TEntity> entities, InsertOptions? options = null, CancellationToken cancellationToken = default)
		{
			await this.Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
		}

		#endregion
		
		#region Update Methods

		public TEntity Update(TEntity entity, string id = default, UpdateOptions? options = null)
		{
			if (this.actionBinder != null && (options ?? UpdateOptions.Default).TriggerBeforeActionBinder)
			{
				entity = this.actionBinder.BeforeUpdate(entity);
			}

			var updatedId = string.IsNullOrEmpty(id) ? entity.Id : id;
			this.Collection.ReplaceOne(item => item.Id == updatedId, entity);

			if (this.actionBinder != null && (options ?? UpdateOptions.Default).TriggerAfterActionBinder)
			{
				entity = this.actionBinder.AfterUpdate(entity);
			}
			
			return entity;
		}
		
		public async ValueTask<TEntity> UpdateAsync(TEntity entity, string id = default, UpdateOptions? options = null, CancellationToken cancellationToken = default)
		{
			if (this.actionBinder != null && (options ?? UpdateOptions.Default).TriggerBeforeActionBinder)
			{
				entity = this.actionBinder.BeforeUpdate(entity);
			}

			var updatedId = string.IsNullOrEmpty(id) ? entity.Id : id;
			await this.Collection.ReplaceOneAsync(item => item.Id == updatedId, entity, cancellationToken: cancellationToken);

			if (this.actionBinder != null && (options ?? UpdateOptions.Default).TriggerAfterActionBinder)
			{
				entity = this.actionBinder.AfterUpdate(entity);
			}
			
			return entity;
		}

		public TEntity Upsert(TEntity entity, string id = default)
		{
			var item = this.FindOne(id ?? entity.Id);
			if (item == null)
			{
				return this.Insert(entity);
			}
			else
			{
				return this.Update(entity, id);
			}
		}
		
		public async ValueTask<TEntity> UpsertAsync(TEntity entity, string id = default, CancellationToken cancellationToken = default)
		{
			var item = await this.FindOneAsync(id ?? entity.Id, cancellationToken: cancellationToken);
			if (item == null)
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

		public bool Delete(string id)
		{
			var result = this.Collection.DeleteOne(item => item.Id == id);
			return result.IsAcknowledged && result.DeletedCount == 1;
		}
		
		public async ValueTask<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
		{
			var result = await this.Collection.DeleteOneAsync(item => item.Id == id, cancellationToken: cancellationToken);
			return result.IsAcknowledged && result.DeletedCount == 1;
		}

		public bool BulkDelete(IEnumerable<TEntity> entities)
		{
			var array = entities.ToArray();
			var result = this.Collection.DeleteMany(Builders<TEntity>.Filter.In(d => d.Id, array.Select(x => x.Id)));
			return result.IsAcknowledged && result.DeletedCount == array.Length;
		}

		public async ValueTask<bool> BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		{
			var array = entities.ToArray();
			var result = await this.Collection.DeleteManyAsync(Builders<TEntity>.Filter.In(d => d.Id, array.Select(x => x.Id)), cancellationToken: cancellationToken);
			return result.IsAcknowledged && result.DeletedCount == array.Length;
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
		
		public long Count(Expression<Func<TEntity, bool>> expression)
		{
			FilterDefinition<TEntity> filterExpression = new ExpressionFilterDefinition<TEntity>(expression);
			return this.Collection.CountDocuments(filterExpression);
		}
		
		public async ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
		{
			FilterDefinition<TEntity> filterExpression = new ExpressionFilterDefinition<TEntity>(expression);
			return await this.Collection.CountDocumentsAsync(filterExpression, cancellationToken: cancellationToken);
		}
		
		public long Count(string query)
		{
			var filterDefinition = new JsonFilterDefinition<TEntity>(query);
			return this.Collection.CountDocuments(filterDefinition);
		}
		
		public async ValueTask<long> CountAsync(string query, CancellationToken cancellationToken = default)
		{
			var filterDefinition = new JsonFilterDefinition<TEntity>(query);
			return await this.Collection.CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken);
		}
		
		#endregion
	}
}