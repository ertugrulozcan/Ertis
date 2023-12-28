using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.Data.Repository;
using Ertis.MongoDB.Models;
using Ertis.MongoDB.Queries;

namespace Ertis.MongoDB.Repository
{
	public interface IMongoRepository<TEntity> : IRepository<TEntity, string> where TEntity : IEntity<string>
	{
		string CollectionName { get; }
		
		IPaginationCollection<TEntity> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null);
		
		IPaginationCollection<TEntity> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		IPaginationCollection<TEntity> Find(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null);
		
		IPaginationCollection<TEntity> Find(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default);

		IPaginationCollection<TEntity> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null);
		
		IPaginationCollection<TEntity> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null);

		ValueTask<IPaginationCollection<TEntity>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<TEntity>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		IPaginationCollection<dynamic> Query(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null);
		
		IPaginationCollection<dynamic> Query(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null);

		IPaginationCollection<dynamic> Query(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null);
		
		IPaginationCollection<dynamic> Query(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null);
		
		ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			Expression<Func<TEntity, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		TField[] Distinct<TField>(string distinctBy, string query = null);
		
		Task<TField[]> DistinctAsync<TField>(string distinctBy, string query = null, CancellationToken cancellationToken = default);
		
		TField[] Distinct<TField>(string distinctBy, Expression<Func<TEntity, bool>> expression);
		
		Task<TField[]> DistinctAsync<TField>(string distinctBy, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
		
		IPaginationCollection<TEntity> Search(string keyword, TextSearchOptions options = null, int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null);
		
		ValueTask<IPaginationCollection<TEntity>> SearchAsync(string keyword, TextSearchOptions options = null, int? skip = null, int? limit = null, bool? withCount = null, string orderBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default);

		dynamic Aggregate(string aggregationStagesJson);
		
		ValueTask<dynamic> AggregateAsync(string aggregationStagesJson, CancellationToken cancellationToken = default);
		
		Task<IEnumerable<IIndexDefinition>> GetIndexesAsync(CancellationToken cancellationToken = default);

		Task<string> CreateIndexAsync(IIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
		
		Task<string[]> CreateManyIndexAsync(IEnumerable<IIndexDefinition> indexDefinitions, CancellationToken cancellationToken = default);

		Task<string> CreateSingleIndexAsync(string fieldName, SortDirection? direction = null, CancellationToken cancellationToken = default);
		
		Task<string> CreateSingleIndexAsync(Expression<Func<TEntity, object>> expression, SortDirection? direction = null, CancellationToken cancellationToken = default);

		Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition, CancellationToken cancellationToken = default);

		Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default);
		
		Task<string> CreateCompoundIndexAsync(IDictionary<Expression<Func<TEntity, object>>, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default);
		
		Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
	}
}