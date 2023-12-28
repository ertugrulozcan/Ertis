using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.Data.Repository;
using Ertis.MongoDB.Client;
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
using UpdateOptions = Ertis.Data.Models.UpdateOptions;

namespace Ertis.MongoDB.Repository
{
    public abstract class DynamicMongoRepository : IDynamicMongoRepository
	{
		#region Services

		private readonly IRepositoryActionBinder _actionBinder;
		private readonly IDatabaseSettings _settings;

		#endregion
		
		#region Properties
		
		public string CollectionName { get; }
		
		private IMongoCollection<dynamic> Collection { get; }
		
		private IMongoCollection<BsonDocument> DocumentCollection { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="clientProvider"></param>
		/// <param name="settings"></param>
		/// <param name="collectionName"></param>
		/// <param name="actionBinder"></param>
		protected DynamicMongoRepository(IMongoClientProvider clientProvider, IDatabaseSettings settings, string collectionName, IRepositoryActionBinder actionBinder = null)
		{
			this._settings = settings;
			
			var database = clientProvider.Client.GetDatabase(settings.DefaultAuthDatabase);

			this.CollectionName = collectionName;
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
			return await this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefaultAsync(cancellationToken: cancellationToken);
		}
		
		// ReSharper disable once UnusedMember.Local
		private async ValueTask<dynamic> FindOneAsync(ObjectId objectId, CancellationToken cancellationToken = default)
		{
			return await this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
		}

		public dynamic FindOne(Expression<Func<dynamic, bool>> expression)
		{
			var filterDefinition = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return this.Collection.Find(filterDefinition).FirstOrDefault();
		}

		public async ValueTask<dynamic> FindOneAsync(Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default)
		{
			var filterDefinition = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return await (await this.Collection.FindAsync(filterDefinition, cancellationToken: cancellationToken)).FirstOrDefaultAsync(cancellationToken: cancellationToken);	
		}
		
		public IPaginationCollection<dynamic> Find(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null)
		{
			return this.Find(skip, limit, withCount, orderBy, sortDirection, locale: null);
		}
		
		public IPaginationCollection<dynamic> Find(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null)
		{
			return this.Find(skip, limit, withCount, sorting, locale: null);
		}

		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null,
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(skip, limit, withCount, orderBy, sortDirection, locale: null, cancellationToken: cancellationToken);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(skip, limit, withCount, sorting, locale: null, cancellationToken: cancellationToken);
		}

		public IPaginationCollection<dynamic> Find(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null)
		{
			return this.Find(expression, skip, limit, withCount, orderBy, sortDirection, locale: null);
		}
		
		public IPaginationCollection<dynamic> Find(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null)
		{
			return this.Find(expression, skip, limit, withCount, sorting, locale: null);
		}

		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null,
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(expression, skip, limit, withCount, orderBy, sortDirection, locale: null, cancellationToken: cancellationToken);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(expression, skip, limit, withCount, sorting, locale: null, cancellationToken: cancellationToken);
		}

		public IPaginationCollection<dynamic> Find(
			string query,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null)
		{
			return this.Find(query, skip, limit, withCount, orderBy, sortDirection, locale: null);
		}
		
		public IPaginationCollection<dynamic> Find(
			string query,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null)
		{
			return this.Find(query, skip, limit, withCount, sorting, locale: null);
		}

		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			string query,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null,
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(query, skip, limit, withCount, orderBy, sortDirection, locale: null, cancellationToken: cancellationToken);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			string query,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(query, skip, limit, withCount, sorting, locale: null, cancellationToken: cancellationToken);
		}

		public IPaginationCollection<dynamic> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null,
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			// ReSharper disable once MethodOverloadWithOptionalParameter
			Locale? locale = null)
		{
			return this.Find(
				expression: null,
				skip,
				limit,
				withCount,
				orderBy,
				sortDirection,
				locale);
		}
		
		public IPaginationCollection<dynamic> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null,
			Sorting sorting = null, 
			// ReSharper disable once MethodOverloadWithOptionalParameter
			Locale? locale = null)
		{
			return this.Find(
				expression: null,
				skip,
				limit,
				withCount,
				sorting,
				locale);
		}

		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(
				expression: null,
				skip,
				limit,
				withCount,
				orderBy,
				sortDirection, 
				locale, 
				cancellationToken: cancellationToken);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.FindAsync(
				expression: null,
				skip,
				limit,
				withCount,
				sorting, 
				locale, 
				cancellationToken: cancellationToken);
		}
		
		public IPaginationCollection<dynamic> Find(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			// ReSharper disable once MethodOverloadWithOptionalParameter
			Locale? locale = null)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return this.Filter(filterExpression, skip, limit, withCount, new Sorting(orderBy, sortDirection), locale);
		}
		
		public IPaginationCollection<dynamic> Find(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			// ReSharper disable once MethodOverloadWithOptionalParameter
			Locale? locale = null)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return this.Filter(filterExpression, skip, limit, withCount, sorting, locale);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return await this.FilterAsync(filterExpression, skip, limit, withCount, new Sorting(orderBy, sortDirection), locale, cancellationToken: cancellationToken);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return await this.FilterAsync(filterExpression, skip, limit, withCount, sorting, locale, cancellationToken: cancellationToken);
		}
		
		public IPaginationCollection<dynamic> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			// ReSharper disable once MethodOverloadWithOptionalParameter
			Locale? locale = null)
		{
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			return this.Filter(filterDefinition, skip, limit, withCount, new Sorting(orderBy, sortDirection), locale);
		}
		
		public IPaginationCollection<dynamic> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			// ReSharper disable once MethodOverloadWithOptionalParameter
			Locale? locale = null)
		{
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			return this.Filter(filterDefinition, skip, limit, withCount, sorting, locale);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			return await this.FilterAsync(filterDefinition, skip, limit, withCount, new Sorting(orderBy, sortDirection), locale, cancellationToken: cancellationToken);
		}
		
		public async ValueTask<IPaginationCollection<dynamic>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null,  
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			return await this.FilterAsync(filterDefinition, skip, limit, withCount, sorting, locale, cancellationToken: cancellationToken);
		}
		
		private IPaginationCollection<dynamic> Filter(
			FilterDefinition<dynamic> predicate, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null,  
			Locale? locale = null)
		{
			var collection = this.ExecuteFilter(predicate, skip, limit, sorting, locale);

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
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			var collection = this.ExecuteFilter(predicate, skip, limit, sorting, locale);

			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = await this.Collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
			}

			return new PaginationCollection<dynamic>
			{
				Count = totalCount,
				Items = await collection.ToListAsync(cancellationToken: cancellationToken)
			};
		}
		
		private IFindFluent<dynamic, dynamic> ExecuteFilter(
			FilterDefinition<dynamic> predicate,
			int? skip = null,
			int? limit = null,
			Sorting sorting = null, 
			Locale? locale = null)
		{
			predicate ??= new ExpressionFilterDefinition<dynamic>(item => true);

			SortDefinition<dynamic> sortDefinition = null;
			if (sorting != null && sorting.Any())
			{
				var sortDefinitionBuilder = new SortDefinitionBuilder<dynamic>();
				var sortDefinitions = new List<SortDefinition<dynamic>>();
				foreach (var sortField in sorting)
				{
					var fieldDefinition = new StringFieldDefinition<dynamic>(sortField.OrderBy);
					sortDefinitions.Add(sortField.SortDirection is null or SortDirection.Ascending 
						? sortDefinitionBuilder.Ascending(fieldDefinition) 
						: sortDefinitionBuilder.Descending(fieldDefinition));	
				}

				sortDefinition = sortDefinitionBuilder.Combine(sortDefinitions);
			}
			
			var options = this.GetFindOptions(sorting, locale);
			var collection = this.Collection.Find(predicate, options);
			if (sortDefinition != null)
			{
				collection = collection.Sort(sortDefinition);
			}
			
			if (skip != null && limit != null)
			{
				collection = collection.Skip(skip).Limit(limit);
			}
			else if (skip != null)
			{
				collection = collection.Skip(skip);
			}
			else if (limit != null)
			{
				collection = collection.Limit(limit);
			}

			return collection;
		}
		
		private FindOptions GetFindOptions(Sorting sorting = null, Locale? locale = null)
		{
			if (sorting != null && sorting.Any(x => x.OrderBy != "_id") && locale != null)
			{
				var collation = new Collation(LocaleHelper.GetLanguageCode(locale.Value));
				return new FindOptions { AllowDiskUse = this._settings.AllowDiskUse, Collation = collation };
			}
			else if (this._settings.AllowDiskUse == true)
			{
				return new FindOptions { AllowDiskUse = true };
			}

			return null;
		}
		
		#endregion

		#region Query Methods

		public IPaginationCollection<dynamic> Query(
			string query,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null)
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
					sorting, 
					selectFields, 
					locale);
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
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null, 
			Locale? locale = null)
		{
			return this.Query(
				query,
				skip,
				limit,
				withCount,
				new Sorting(orderBy, sortDirection), 
				selectFields,
				locale);
		}

		public IPaginationCollection<dynamic> Query(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null)
		{
			try
			{
				var filterDefinition = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
				return this.ExecuteQuery(
					filterDefinition,
					skip,
					limit,
					withCount,
					sorting,
					selectFields, 
					locale);
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
			string orderBy = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null, 
			Locale? locale = null)
		{
			return this.Query(
				expression,
				skip,
				limit,
				withCount,
				new Sorting(orderBy, sortDirection), 
				selectFields,
				locale);
		}

		public async ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			string query,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null,
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
					sorting, 
					selectFields, 
					locale, 
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
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			return await this.QueryAsync(
				query,
				skip,
				limit,
				withCount,
				new Sorting(orderBy, sortDirection), 
				selectFields,
				locale,
				cancellationToken: cancellationToken);
		}

		public async ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			string orderBy = null,
			SortDirection? sortDirection = null,
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null,
			CancellationToken cancellationToken = default)
		{
			return await this.QueryAsync(
				expression,
				skip,
				limit,
				withCount,
				new Sorting(orderBy, sortDirection),
				selectFields,
				locale,
				cancellationToken: cancellationToken);
		}

		public async ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			Expression<Func<dynamic, bool>> expression,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null, 
			Locale? locale = null, 
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
					sorting,
					selectFields, 
					locale, 
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
			FilterDefinition<dynamic> filterDefinition,
			int? skip = null,
			int? limit = null,
			bool? withCount = null,
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null, 
			Locale? locale = null)
		{
			try
			{
				var filterResult = this.ExecuteFilter(filterDefinition, skip, limit, sorting, locale);
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
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default)
		{
			try
			{
				var filterResult = this.ExecuteFilter(filterDefinition, skip, limit, sorting, locale);
				var projectionDefinition = ExecuteSelectQuery<dynamic>(selectFields);
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
		
		#region Distinct Methods

		public TField[] Distinct<TField>(string distinctBy, string query = null)
		{
			FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			var cursor = this.Collection.Distinct(fieldDefinition, filterDefinition);
			return cursor.Current.ToArray();
		}
		
		public async Task<TField[]> DistinctAsync<TField>(string distinctBy, string query = null, CancellationToken cancellationToken = default)
		{
			FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
			var filterDefinition = string.IsNullOrEmpty(query) ? FilterDefinition<dynamic>.Empty : new JsonFilterDefinition<dynamic>(query);
			var cursor = await this.Collection.DistinctAsync(fieldDefinition, filterDefinition, cancellationToken: cancellationToken);
			var result = await cursor.ToListAsync(cancellationToken: cancellationToken);
			return result.ToArray();
		}
		
		public TField[] Distinct<TField>(string distinctBy, Expression<Func<dynamic, bool>> expression)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return this.DistinctCore<TField>(distinctBy, filterExpression);
		}
		
		public async Task<TField[]> DistinctAsync<TField>(string distinctBy, Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default)
		{
			var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
			return await this.DistinctCoreAsync<TField>(distinctBy, filterExpression, cancellationToken: cancellationToken);
		}
		
		private TField[] DistinctCore<TField>(string distinctBy, FilterDefinition<dynamic> predicate)
		{
			predicate ??= new ExpressionFilterDefinition<dynamic>(item => true);
			FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
			var cursor = this.Collection.Distinct(fieldDefinition, predicate);
			return cursor.Current.ToArray();
		}
		
		private async Task<TField[]> DistinctCoreAsync<TField>(string distinctBy, FilterDefinition<dynamic> predicate, CancellationToken cancellationToken = default)
		{
			predicate ??= new ExpressionFilterDefinition<dynamic>(item => true);
			FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
			var cursor = await this.Collection.DistinctAsync(fieldDefinition, predicate, cancellationToken: cancellationToken);
			var result = await cursor.ToListAsync(cancellationToken: cancellationToken);
			return result.ToArray();
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

		public dynamic Insert(object entity, InsertOptions? options = null)
		{
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
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

			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
			{
				entity = this._actionBinder.AfterInsert(entity);
			}
			
			return entity;
		}
		
		public async ValueTask<dynamic> InsertAsync(object entity, InsertOptions? options = null, CancellationToken cancellationToken = default)
		{
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
			{
				entity = this._actionBinder.BeforeInsert(entity);
			}
			
			if (entity is BsonDocument document)
			{
				await this.DocumentCollection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken: cancellationToken);
			}
			else
			{
				await this.Collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken: cancellationToken);	
			}

			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
			{
				entity = this._actionBinder.AfterInsert(entity);
			}
			
			return entity;
		}

		public void BulkInsert(IEnumerable<object> entities, InsertOptions? options = null)
		{
			var enumerable = entities as object[] ?? entities.ToArray();
			
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			this.Collection.InsertMany(enumerable);

			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.AfterInsert(entity);	
				}
			}
		}

		public async ValueTask BulkInsertAsync(IEnumerable<object> entities, InsertOptions? options = null, CancellationToken cancellationToken = default)
		{
			var enumerable = entities as object[] ?? entities.ToArray();
			
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			await this.Collection.InsertManyAsync(enumerable, cancellationToken: cancellationToken);

			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
			{
				foreach (var entity in enumerable)
				{
					this._actionBinder.AfterInsert(entity);	
				}
			}
		}

		public ICollection<dynamic> InsertMany(ICollection<object> entities, InsertOptions? options = null)
		{
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
			{
				foreach (var entity in entities)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			this.Collection.InsertMany(entities);
			
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
			{
				foreach (var entity in entities)
				{
					this._actionBinder.AfterInsert(entity);	
				}
			}
			
			return entities;
		}
		
		public async Task<ICollection<dynamic>> InsertManyAsync(ICollection<object> entities, InsertOptions? options = null, CancellationToken cancellationToken = default)
		{
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
			{
				foreach (var entity in entities)
				{
					this._actionBinder.BeforeInsert(entity);	
				}
			}
			
			await this.Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
			
			if (this._actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
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

		public dynamic Update(object entity, string id = default, UpdateOptions? options = null)
		{
			if (this._actionBinder != null && (options ?? UpdateOptions.Default).TriggerBeforeActionBinder)
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

			if (this._actionBinder != null && (options ?? UpdateOptions.Default).TriggerAfterActionBinder)
			{
				entity = this._actionBinder.AfterUpdate(entity);
			}
			
			return entity;
		}
		
		public async ValueTask<dynamic> UpdateAsync(object entity, string id = default, UpdateOptions? options = null, CancellationToken cancellationToken = default)
		{
			if (this._actionBinder != null && (options ?? UpdateOptions.Default).TriggerBeforeActionBinder)
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

			if (this._actionBinder != null && (options ?? UpdateOptions.Default).TriggerAfterActionBinder)
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
				return await this.InsertAsync(entity, cancellationToken: cancellationToken);
			}
			else
			{
				var item = await this.FindOneAsync(id, cancellationToken: cancellationToken);
				return item == null ? await this.InsertAsync(entity, cancellationToken: cancellationToken) : await this.UpdateAsync(entity, id, cancellationToken: cancellationToken);
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
			var result = await this.Collection.DeleteOneAsync(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id)), cancellationToken: cancellationToken);
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
				isDeletedAll &= await this.DeleteAsync(entity, cancellationToken: cancellationToken);
			}

			return isDeletedAll;
		}

		#endregion
		
		#region Count Methods

		public long Count()
		{
			return this.DocumentCollection.CountDocuments(item => true);
		}
		
		public async ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
		{
			return await this.DocumentCollection.CountDocumentsAsync(item => true, cancellationToken: cancellationToken);
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
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
			var filterDefinition = new JsonFilterDefinition<dynamic>(query);
			return this.Collection.CountDocuments(filterDefinition);
		}
		
		public async ValueTask<long> CountAsync(string query, CancellationToken cancellationToken = default)
		{
			query = QueryHelper.EnsureObjectIdsAndISODates(query);
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
					var documents = await aggregationResultCursor.ToListAsync(cancellationToken: cancellationToken);
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
				Builders<dynamic>.IndexKeys.Descending(fieldName) :
				Builders<dynamic>.IndexKeys.Ascending(fieldName);
			
			return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<dynamic>(indexKeysDefinition), cancellationToken: cancellationToken);
		}
		
		public async Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
		{
			return await this.CreateSingleIndexAsync(indexDefinition.Field, indexDefinition.Direction, cancellationToken: cancellationToken);
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