using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.Data.Repository;
using Ertis.MongoDB.Client;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Helpers;
using Ertis.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using SortDirection = Ertis.Core.Collections.SortDirection;
using UpdateOptions = Ertis.Data.Models.UpdateOptions;

// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.MongoDB.Repository;

// ReSharper disable once UnusedType.Global
public abstract class DynamicMongoRepository : IDynamicMongoRepository
{
	#region Services
	
	private readonly IRepositoryActionBinder? _actionBinder;
	private readonly IDatabaseSettings _settings;
	
	#endregion
	
	#region Properties
	
	public string CollectionName { get; }
	
	protected IMongoCollection<dynamic> Collection { get; }
	
	protected IMongoCollection<BsonDocument> DocumentCollection { get; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="clientProvider"></param>
	/// <param name="settings"></param>
	/// <param name="collectionName"></param>
	/// <param name="actionBinder"></param>
	protected DynamicMongoRepository(IMongoClientProvider clientProvider, IDatabaseSettings settings, string collectionName, IRepositoryActionBinder? actionBinder = null)
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
	
	public dynamic? FindOne(string id)
	{
		return this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefault();
	}
	
	// ReSharper disable once UnusedMember.Local
	private dynamic? FindOne(ObjectId objectId)
	{
		return this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", objectId)).FirstOrDefault();
	}
	
	public async Task<dynamic?> FindOneAsync(string id, CancellationToken cancellationToken = default)
	{
		return await this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id))).FirstOrDefaultAsync(cancellationToken: cancellationToken);
	}
	
	// ReSharper disable once UnusedMember.Local
	private async Task<dynamic?> FindOneAsync(ObjectId objectId, CancellationToken cancellationToken = default)
	{
		return await this.Collection.Find(Builders<dynamic>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
	}
	
	public dynamic? FindOne(Expression<Func<dynamic, bool>> expression)
	{
		var filterDefinition = new ExpressionFilterDefinition<dynamic>(expression);
		return this.Collection.Find(filterDefinition).FirstOrDefault();
	}
	
	public async Task<dynamic?> FindOneAsync(Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default)
	{
		var filterDefinition = new ExpressionFilterDefinition<dynamic>(expression);
		return await (await this.Collection.FindAsync(filterDefinition, cancellationToken: cancellationToken)).FirstOrDefaultAsync(cancellationToken: cancellationToken);	
	}
	
	public IPaginationCollection<dynamic> Find(
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null)
	{
		return this.Find(skip, limit, withCount, orderBy, sortDirection, collationOptions: null);
	}
	
	public IPaginationCollection<dynamic> Find(
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null)
	{
		return this.Find(skip, limit, withCount, sorting, collationOptions: null);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null,
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(skip, limit, withCount, orderBy, sortDirection, collationOptions: null, cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(skip, limit, withCount, sorting, collationOptions: null, cancellationToken: cancellationToken);
	}
	
	public IPaginationCollection<dynamic> Find(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null)
	{
		return this.Find(expression, skip, limit, withCount, orderBy, sortDirection, collationOptions: null);
	}
	
	public IPaginationCollection<dynamic> Find(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null)
	{
		return this.Find(expression, skip, limit, withCount, sorting, collationOptions: null);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null,
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(expression, skip, limit, withCount, orderBy, sortDirection, collationOptions: null, cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(expression, skip, limit, withCount, sorting, collationOptions: null, cancellationToken: cancellationToken);
	}
	
	public IPaginationCollection<dynamic> Find(
		string query,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null)
	{
		return this.Find(query, skip, limit, withCount, orderBy, sortDirection, collationOptions: null);
	}
	
	public IPaginationCollection<dynamic> Find(
		string query,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null)
	{
		return this.Find(query, skip, limit, withCount, sorting, collationOptions: null);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		string query,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null,
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(query, skip, limit, withCount, orderBy, sortDirection, collationOptions: null, cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		string query,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(query, skip, limit, withCount, sorting, collationOptions: null, cancellationToken: cancellationToken);
	}
	
	[SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
	public IPaginationCollection<dynamic> Find(
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null,
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		return this.Find(
			expression: null,
			skip,
			limit,
			withCount,
			orderBy,
			sortDirection,
			indexOptions,
			collationOptions);
	}
	
	[SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
	public IPaginationCollection<dynamic> Find(
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null,
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		return this.Find(
			expression: null,
			skip,
			limit,
			withCount,
			sorting,
			indexOptions,
			collationOptions);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(
			expression: null,
			skip,
			limit,
			withCount,
			orderBy,
			sortDirection, 
			indexOptions,
			collationOptions, 
			cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		return await this.FindAsync(
			expression: null,
			skip,
			limit,
			withCount,
			sorting, 
			indexOptions,
			collationOptions, 
			cancellationToken: cancellationToken);
	}
	
	[SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
	public IPaginationCollection<dynamic> Find(
		Expression<Func<dynamic, bool>>? expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
		var sorting = string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection);
		return this.Filter(filterExpression, skip, limit, withCount, sorting, indexOptions, collationOptions);
	}
	
	[SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
	public IPaginationCollection<dynamic> Find(
		Expression<Func<dynamic, bool>>? expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
		return this.Filter(filterExpression, skip, limit, withCount, sorting, indexOptions, collationOptions);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		Expression<Func<dynamic, bool>>? expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
		var sorting = string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection);
		return await this.FilterAsync(filterExpression, skip, limit, withCount, sorting, indexOptions, collationOptions, cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		Expression<Func<dynamic, bool>>? expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
		return await this.FilterAsync(filterExpression, skip, limit, withCount, sorting, indexOptions, collationOptions, cancellationToken: cancellationToken);
	}
	
	[SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
	public IPaginationCollection<dynamic> Find(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		var sorting = string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection);
		return this.Filter(filterDefinition, skip, limit, withCount, sorting, indexOptions, collationOptions);
	}
	
	[SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
	public IPaginationCollection<dynamic> Find(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return this.Filter(filterDefinition, skip, limit, withCount, sorting, indexOptions, collationOptions);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		var sorting = string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection);
		return await this.FilterAsync(filterDefinition, skip, limit, withCount, sorting, indexOptions, collationOptions, cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> FindAsync(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return await this.FilterAsync(filterDefinition, skip, limit, withCount, sorting, indexOptions, collationOptions, cancellationToken: cancellationToken);
	}
	
	private IPaginationCollection<dynamic> Filter(
		FilterDefinition<dynamic> predicate, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		var collection = this.ExecuteFilter(predicate, skip, limit, sorting, indexOptions, collationOptions);
		
		long totalCount = 0;
		if (withCount != null && withCount.Value)
		{
			totalCount = this.Count(predicate, indexOptions, collationOptions);
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
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		var collection = this.ExecuteFilter(predicate, skip, limit, sorting, indexOptions, collationOptions);
		
		long totalCount = 0;
		if (withCount != null && withCount.Value)
		{
			totalCount = await this.CountAsync(predicate, indexOptions, collationOptions, cancellationToken);
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
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		SortDefinition<dynamic>? sortDefinition = null;
		if (sorting is { Count: > 0 })
		{
			var sortDefinitionBuilder = new SortDefinitionBuilder<dynamic>();
			var sortDefinitions = new List<SortDefinition<dynamic>>();
			foreach (var sortField in sorting)
			{
				if (!string.IsNullOrEmpty(sortField.OrderBy.Trim()))
				{
					var fieldDefinition = new StringFieldDefinition<dynamic>(sortField.OrderBy);
					sortDefinitions.Add(sortField.SortDirection is null or SortDirection.Ascending 
						? sortDefinitionBuilder.Ascending(fieldDefinition) 
						: sortDefinitionBuilder.Descending(fieldDefinition));
				}
			}
			
			sortDefinition = sortDefinitionBuilder.Combine(sortDefinitions);
		}
		
		var options = this.GetFindOptions(indexOptions, collationOptions);
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
	
	private FindOptions GetFindOptions(IndexOptions? indexOptions = null, CollationOptions? collationOptions = null)
	{
		return new FindOptions
		{
			AllowDiskUse = this._settings.AllowDiskUse,
			Collation = collationOptions?.GetCollation(),
			Hint = indexOptions?.GetIndexHint()
		};
	}
	
	#endregion
	
	#region Query Methods
	
	public IPaginationCollection<dynamic> Query(
		string query,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return this.ExecuteQuery(
			filterDefinition,
			skip,
			limit,
			withCount,
			sorting, 
			selectFields, 
			indexOptions,
			collationOptions);
	}
	
	public IPaginationCollection<dynamic> Query(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IDictionary<string, bool>? selectFields = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		return this.Query(
			query,
			skip,
			limit,
			withCount,
			string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection), 
			selectFields,
			indexOptions,
			collationOptions);
	}
	
	public IPaginationCollection<dynamic> Query(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		var filterDefinition = new ExpressionFilterDefinition<dynamic>(expression);
		return this.ExecuteQuery(
			filterDefinition,
			skip,
			limit,
			withCount,
			sorting,
			selectFields, 
			indexOptions,
			collationOptions);
	}
	
	public IPaginationCollection<dynamic> Query(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null,
		IDictionary<string, bool>? selectFields = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		return this.Query(
			expression,
			skip,
			limit,
			withCount,
			string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection), 
			selectFields,
			indexOptions,
			collationOptions);
	}
	
	public async Task<IPaginationCollection<dynamic>> QueryAsync(
		string query,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null,
		CancellationToken cancellationToken = default)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return await this.ExecuteQueryAsync(
			filterDefinition,
			skip,
			limit,
			withCount,
			sorting, 
			selectFields, 
			indexOptions,
			collationOptions, 
			cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> QueryAsync(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IDictionary<string, bool>? selectFields = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		return await this.QueryAsync(
			query,
			skip,
			limit,
			withCount,
			string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection), 
			selectFields,
			indexOptions,
			collationOptions,
			cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> QueryAsync(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		string? orderBy = null,
		SortDirection? sortDirection = null,
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null,
		CancellationToken cancellationToken = default)
	{
		return await this.QueryAsync(
			expression,
			skip,
			limit,
			withCount,
			string.IsNullOrEmpty(orderBy) ? null : new Sorting(orderBy, sortDirection),
			selectFields,
			indexOptions,
			collationOptions,
			cancellationToken: cancellationToken);
	}
	
	public async Task<IPaginationCollection<dynamic>> QueryAsync(
		Expression<Func<dynamic, bool>> expression,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		var filterDefinition = new ExpressionFilterDefinition<dynamic>(expression);
		return await this.ExecuteQueryAsync(
			filterDefinition,
			skip,
			limit,
			withCount,
			sorting,
			selectFields, 
			indexOptions,
			collationOptions, 
			cancellationToken: cancellationToken);
	}
	
	private IPaginationCollection<dynamic> ExecuteQuery(
		FilterDefinition<dynamic> filterDefinition,
		int? skip = null,
		int? limit = null,
		bool? withCount = null,
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null)
	{
		try
		{
			var filterResult = this.ExecuteFilter(filterDefinition, skip, limit, sorting, indexOptions, collationOptions);
			var projectionDefinition = ExecuteSelectQuery<dynamic>(selectFields);
			var collection = filterResult.Project(projectionDefinition);
			
			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = this.Count(filterDefinition, indexOptions, collationOptions);
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
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			var filterResult = this.ExecuteFilter(filterDefinition, skip, limit, sorting, indexOptions, collationOptions);
			var projectionDefinition = ExecuteSelectQuery<dynamic>(selectFields);
			var collection = filterResult.Project(projectionDefinition);
			
			long totalCount = 0;
			if (withCount != null && withCount.Value)
			{
				totalCount = await this.CountAsync(filterDefinition, indexOptions, collationOptions, cancellationToken);
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
	
	public TField[] Distinct<TField>(string distinctBy, string? query = null)
	{
		FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		var cursor = this.Collection.Distinct(fieldDefinition, filterDefinition);
		return cursor.ToList().ToArray();
	}
	
	public async Task<TField[]> DistinctAsync<TField>(string distinctBy, string? query = null, CancellationToken cancellationToken = default)
	{
		FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		var cursor = await this.Collection.DistinctAsync(fieldDefinition, filterDefinition, cancellationToken: cancellationToken);
		var result = await cursor.ToListAsync(cancellationToken: cancellationToken);
		return result.ToArray();
	}
	
	public TField[] Distinct<TField>(string distinctBy, Expression<Func<dynamic, bool>>? expression)
	{
		var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
		return this.DistinctCore<TField>(distinctBy, filterExpression);
	}
	
	public async Task<TField[]> DistinctAsync<TField>(string distinctBy, Expression<Func<dynamic, bool>>? expression, CancellationToken cancellationToken = default)
	{
		var filterExpression = expression != null ? new ExpressionFilterDefinition<dynamic>(expression) : FilterDefinition<dynamic>.Empty;
		return await this.DistinctCoreAsync<TField>(distinctBy, filterExpression, cancellationToken: cancellationToken);
	}
	
	private TField[] DistinctCore<TField>(string distinctBy, FilterDefinition<dynamic>? predicate)
	{
		predicate ??= new ExpressionFilterDefinition<dynamic>(item => true);
		FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
		var cursor = this.Collection.Distinct(fieldDefinition, predicate);
		return cursor.ToList().ToArray();
	}
	
	private async Task<TField[]> DistinctCoreAsync<TField>(string distinctBy, FilterDefinition<dynamic>? predicate, CancellationToken cancellationToken = default)
	{
		predicate ??= new ExpressionFilterDefinition<dynamic>(item => true);
		FieldDefinition<dynamic, TField> fieldDefinition = new StringFieldDefinition<dynamic, TField>(distinctBy);
		var cursor = await this.Collection.DistinctAsync(fieldDefinition, predicate, cancellationToken: cancellationToken);
		var result = await cursor.ToListAsync(cancellationToken: cancellationToken);
		return result.ToArray();
	}
	
	#endregion
	
	#region Select Methods
	
	private static ProjectionDefinition<T> ExecuteSelectQuery<T>(IDictionary<string, bool>? selectFields)
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
	
	public async Task<dynamic> InsertAsync(object entity, InsertOptions? options = null, CancellationToken cancellationToken = default)
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
		var items = this.BeforeBulkInsert(entities, options);
		this.InsertManyCore(items);
		this.AfterBulkInsert(items, options);
	}
	
	public async Task BulkInsertAsync(IEnumerable<object> entities, InsertOptions? options = null, CancellationToken cancellationToken = default)
	{
		var items = this.BeforeBulkInsert(entities, options);
		await this.InsertManyCoreAsync(items, cancellationToken);
		this.AfterBulkInsert(items, options);
	}
	
	// ReSharper disable once UnusedMember.Global
	public ICollection<dynamic> InsertMany(ICollection<object> entities, InsertOptions? options = null)
	{
		var items = this.BeforeBulkInsert(entities, options);
		this.InsertManyCore(items);
		return this.AfterBulkInsert(items, options);
	}
	
	// ReSharper disable once UnusedMember.Global
	public async Task<ICollection<dynamic>> InsertManyAsync(ICollection<object> entities, InsertOptions? options = null, CancellationToken cancellationToken = default)
	{
		var items = this.BeforeBulkInsert(entities, options);
		await this.InsertManyCoreAsync(items, cancellationToken);
		return this.AfterBulkInsert(items, options);
	}
	
	/// <summary>
	/// The BsonDocuments are inserted into the document collection (the dynamic collection stores them wrapped as _t/_v), like Insert does
	/// </summary>
	private void InsertManyCore(object[] entities)
	{
		var documents = entities.OfType<BsonDocument>().ToArray();
		var others = entities.Where(x => x is not BsonDocument).ToArray();
		if (documents.Length > 0)
		{
			this.DocumentCollection.InsertMany(documents);
		}
		
		if (others.Length > 0)
		{
			this.Collection.InsertMany(others);
		}
	}
	
	private async Task InsertManyCoreAsync(object[] entities, CancellationToken cancellationToken)
	{
		var documents = entities.OfType<BsonDocument>().ToArray();
		var others = entities.Where(x => x is not BsonDocument).ToArray();
		if (documents.Length > 0)
		{
			await this.DocumentCollection.InsertManyAsync(documents, cancellationToken: cancellationToken);
		}
		
		if (others.Length > 0)
		{
			await this.Collection.InsertManyAsync(others, cancellationToken: cancellationToken);
		}
	}
	
	private object[] BeforeBulkInsert(IEnumerable<object> entities, InsertOptions? options)
	{
		var actionBinder = this._actionBinder;
		if (actionBinder != null && (options ?? InsertOptions.Default).TriggerBeforeActionBinder)
		{
			return entities.Select(x => actionBinder.BeforeInsert(x)).ToArray();
		}
		
		return entities.ToArray();
	}
	
	private object[] AfterBulkInsert(object[] entities, InsertOptions? options)
	{
		var actionBinder = this._actionBinder;
		if (actionBinder != null && (options ?? InsertOptions.Default).TriggerAfterActionBinder)
		{
			return entities.Select(x => actionBinder.AfterInsert(x)).ToArray();
		}
		
		return entities;
	}
	
	#endregion
	
	#region Update Methods
	
	public dynamic Update(object entity, string? id = null, UpdateOptions? options = null)
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
	
	public async Task<dynamic> UpdateAsync(object entity, string? id = null, UpdateOptions? options = null, CancellationToken cancellationToken = default)
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
	public dynamic Upsert(dynamic entity, string? id = null)
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
	public async Task<dynamic> UpsertAsync(dynamic entity, string? id = null, CancellationToken cancellationToken = default)
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
	
	public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
	{
		var result = await this.Collection.DeleteOneAsync(Builders<dynamic>.Filter.Eq("_id", ObjectId.Parse(id)), cancellationToken: cancellationToken);
		return result.IsAcknowledged && result.DeletedCount == 1;
	}
	
	public bool BulkDelete(IEnumerable<dynamic> entities)
	{
		return entities.Aggregate(true, (current, entity) => (bool) (current & this.Delete(entity)));
	}
	
	public async Task<bool> BulkDeleteAsync(IEnumerable<dynamic> entities, CancellationToken cancellationToken = default)
	{
		var isDeletedAll = true;
		foreach (var entity in entities)
		{
			isDeletedAll &= await this.DeleteAsync(entity, cancellationToken: cancellationToken);
		}
		
		return isDeletedAll;
	}
	
	public bool DeleteMany(Expression<Func<dynamic, bool>> expression)
	{
		var filterDefinition = new ExpressionFilterDefinition<dynamic>(expression);
		var result = this.Collection.DeleteMany(filterDefinition);
		return result.IsAcknowledged;
	}
	
	public async Task<bool> DeleteManyAsync(Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default)
	{
		var filterDefinition = new ExpressionFilterDefinition<dynamic>(expression);
		var result = await this.Collection.DeleteManyAsync(filterDefinition, cancellationToken: cancellationToken);
		return result.IsAcknowledged;
	}
	
	public bool DeleteMany(string query)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		var result = this.Collection.DeleteMany(filterDefinition);
		return result.IsAcknowledged;
	}
	
	public async Task<bool> DeleteManyAsync(string query, CancellationToken cancellationToken = default)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		var result = await this.Collection.DeleteManyAsync(filterDefinition, cancellationToken: cancellationToken);
		return result.IsAcknowledged;
	}
	
	public bool Clear()
	{
		var result = this.Collection.DeleteMany(Builders<dynamic>.Filter.Empty);
		return result.IsAcknowledged;
	}
	
	public async Task<bool> ClearAsync(CancellationToken cancellationToken = default)
	{
		var result = await this.Collection.DeleteManyAsync(Builders<dynamic>.Filter.Empty, cancellationToken: cancellationToken);
		return result.IsAcknowledged;
	}
	
	#endregion
	
	#region Count Methods
	
	public long Count()
	{
		return this.Count(item => true);
	}
	
	public long Count(IndexOptions? indexOptions)
	{
		return this.Count(item => true, indexOptions);
	}
	
	public async Task<long> CountAsync(CancellationToken cancellationToken = default)
	{
		return await this.CountAsync(item => true, cancellationToken: cancellationToken);
	}
	
	public async Task<long> CountAsync(IndexOptions? indexOptions = null, CancellationToken cancellationToken = default)
	{
		return await this.CountAsync(item => true, indexOptions, cancellationToken: cancellationToken);
	}
	
	public long Count(Expression<Func<dynamic, bool>> expression)
	{
		FilterDefinition<dynamic> filterExpression = new ExpressionFilterDefinition<dynamic>(expression);
		return this.Count(filterExpression);
	}
	
	public long Count(Expression<Func<dynamic, bool>> expression, IndexOptions? indexOptions)
	{
		FilterDefinition<dynamic> filterExpression = new ExpressionFilterDefinition<dynamic>(expression);
		return this.Count(filterExpression, indexOptions);
	}
	
	public async Task<long> CountAsync(Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default)
	{
		FilterDefinition<dynamic> filterExpression = new ExpressionFilterDefinition<dynamic>(expression);
		return await this.CountAsync(filterExpression, cancellationToken: cancellationToken);
	}
	
	public async Task<long> CountAsync(Expression<Func<dynamic, bool>> expression, IndexOptions? indexOptions = null, CancellationToken cancellationToken = default)
	{
		FilterDefinition<dynamic> filterExpression = new ExpressionFilterDefinition<dynamic>(expression);
		return await this.CountAsync(filterExpression, indexOptions, cancellationToken: cancellationToken);
	}
	
	public long Count(string query)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return this.Count(filterDefinition);
	}
	
	public long Count(string query, IndexOptions? indexOptions)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return this.Count(filterDefinition, indexOptions);
	}
	
	public async Task<long> CountAsync(string query, CancellationToken cancellationToken = default)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return await this.CountAsync(filterDefinition, cancellationToken: cancellationToken);
	}
	
	public async Task<long> CountAsync(string query, IndexOptions? indexOptions = null, CancellationToken cancellationToken = default)
	{
		var filterDefinition = QueryHelper.CreateFilterDefinition<dynamic>(query);
		return await this.CountAsync(filterDefinition, indexOptions, cancellationToken: cancellationToken);
	}
	
	private long Count(FilterDefinition<dynamic> filterDefinition, IndexOptions? indexOptions = null, CollationOptions? collationOptions = null)
	{
		var countOptions = new CountOptions { Hint = indexOptions?.GetIndexHint(), Collation = collationOptions?.GetCollation() };
		return this.Collection.CountDocuments(filterDefinition, countOptions);
	}
	
	private async Task<long> CountAsync(FilterDefinition<dynamic> filterDefinition, IndexOptions? indexOptions = null, CollationOptions? collationOptions = null, CancellationToken cancellationToken = default)
	{
		var countOptions = new CountOptions { Hint = indexOptions?.GetIndexHint(), Collation = collationOptions?.GetCollation() };
		return await this.Collection.CountDocumentsAsync(filterDefinition, countOptions, cancellationToken: cancellationToken);
	}
	
	public long EstimatedCount()
	{
		return this.Collection.EstimatedDocumentCount();
	}
	
	public async Task<long> EstimatedCountAsync(CancellationToken cancellationToken = default)
	{
		return await this.Collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
	}
	
	#endregion
	
	#region Aggregation Methods
	
	public dynamic Aggregate(string query, IndexOptions? indexOptions = null, CollationOptions? collationOptions = null)
	{
		try
		{
			var pipelineDefinition = QueryHelper.CreatePipelineDefinition<dynamic>(query);
			
			var aggregationOptions = new AggregateOptions
			{
				Collation = collationOptions?.GetCollation(),
				Hint = indexOptions?.GetIndexHint()
			};
			
			var aggregationResultCursor = this.Collection.Aggregate(pipelineDefinition, aggregationOptions);
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
	
	public async Task<dynamic> AggregateAsync(string query, IndexOptions? indexOptions = null, CollationOptions? collationOptions = null, CancellationToken cancellationToken = default)
	{
		try
		{
			var pipelineDefinition = QueryHelper.CreatePipelineDefinition<dynamic>(query);
			
			var aggregationOptions = new AggregateOptions
			{
				Collation = collationOptions?.GetCollation(),
				Hint = indexOptions?.GetIndexHint()
			};
			
			var aggregationResultCursor = await this.Collection.AggregateAsync(pipelineDefinition, aggregationOptions, cancellationToken: cancellationToken);
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
	
	#region Index Methods
	
	public async Task<IEnumerable<IIndexDefinition>> GetIndexesAsync(CancellationToken cancellationToken = default)
	{
		var indexesCursor = await this.Collection.Indexes.ListAsync(cancellationToken: cancellationToken);
		var indexes = await indexesCursor.ToListAsync(cancellationToken: cancellationToken);
		return IndexHelper.ToIndexDefinitions(indexes).ToArray();
	}
	
	public async Task<string> CreateIndexAsync(IIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
	{
		return await this.Collection.Indexes.CreateOneAsync(IndexHelper.ToIndexModel<dynamic>(indexDefinition), cancellationToken: cancellationToken);
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
		return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<dynamic>(IndexHelper.GetKeys<dynamic>(fieldName, direction)), cancellationToken: cancellationToken);
	}
	
	public async Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
	{
		return await this.CreateIndexAsync(indexDefinition, cancellationToken: cancellationToken);
	}
	
	public async Task<string> CreateTTLIndexAsync(TTLIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
	{
		return await this.CreateIndexAsync(indexDefinition, cancellationToken: cancellationToken);
	}
	
	public async Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default)
	{
		var combinedIndexDefinition = Builders<dynamic>.IndexKeys.Combine(indexFieldDefinitions.Select(x => IndexHelper.GetKeys<dynamic>(x.Key, x.Value)));
		return await this.Collection.Indexes.CreateOneAsync(new CreateIndexModel<dynamic>(combinedIndexDefinition), cancellationToken: cancellationToken);
	}
	
	public async Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
	{
		return await this.CreateIndexAsync(indexDefinition, cancellationToken: cancellationToken);
	}
	
	public async Task<string> CreateTextIndexAsync(TextIndexDefinition indexDefinition, CancellationToken cancellationToken = default)
	{
		return await this.CreateIndexAsync(indexDefinition, cancellationToken: cancellationToken);
	}
	
	#endregion
}