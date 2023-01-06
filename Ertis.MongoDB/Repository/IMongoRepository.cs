using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
		IPaginationCollection<dynamic> Query(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, IDictionary<string, bool> selectFields = null);

		IPaginationCollection<dynamic> Query(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, IDictionary<string, bool> selectFields = null);
		
		ValueTask<IPaginationCollection<dynamic>> QueryAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, IDictionary<string, bool> selectFields = null);
		
		ValueTask<IPaginationCollection<dynamic>> QueryAsync(Expression<Func<TEntity, bool>> expression, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, IDictionary<string, bool> selectFields = null);
		
		IPaginationCollection<TEntity> Search(string keyword, TextSearchOptions options = null, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);
		
		ValueTask<IPaginationCollection<TEntity>> SearchAsync(string keyword, TextSearchOptions options = null, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null);

		dynamic Aggregate(string aggregationStagesJson);
		
		ValueTask<dynamic> AggregateAsync(string aggregationStagesJson);
		
		Task<IEnumerable<IIndexDefinition>> GetIndexesAsync();

		Task<string> CreateIndexAsync(IIndexDefinition indexDefinition);
		
		Task<string[]> CreateManyIndexAsync(IEnumerable<IIndexDefinition> indexDefinitions);

		Task<string> CreateSingleIndexAsync(string fieldName, SortDirection? direction = null);
		
		Task<string> CreateSingleIndexAsync(Expression<Func<TEntity, object>> expression, SortDirection? direction = null);

		Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition);

		Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions);
		
		Task<string> CreateCompoundIndexAsync(IDictionary<Expression<Func<TEntity, object>>, SortDirection> indexFieldDefinitions);
		
		Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition);
	}
}