using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Repository;
using Ertis.MongoDB.Models;

namespace Ertis.MongoDB.Repository
{
    public interface IDynamicMongoRepository : IDynamicRepository<string>
    {
        string CollectionName { get; }
        
        IPaginationCollection<dynamic> Find(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null);
        
        IPaginationCollection<dynamic> Find(
	        int? skip = null, 
	        int? limit = null, 
	        bool? withCount = null, 
	        Sorting sorting = null, 
	        Locale? locale = null);
		
		ValueTask<IPaginationCollection<dynamic>> FindAsync(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<dynamic>> FindAsync(
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		IPaginationCollection<dynamic> Find(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null);
		
		IPaginationCollection<dynamic> Find(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null);
		
		ValueTask<IPaginationCollection<dynamic>> FindAsync(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<dynamic>> FindAsync(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null, 
			CancellationToken cancellationToken = default);

		IPaginationCollection<dynamic> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null);
		
		IPaginationCollection<dynamic> Find(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			Locale? locale = null);

		ValueTask<IPaginationCollection<dynamic>> FindAsync(
			string query, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<dynamic>> FindAsync(
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
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null);
		
		IPaginationCollection<dynamic> Query(
			Expression<Func<dynamic, bool>> expression, 
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
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			string orderBy = null, 
			SortDirection? sortDirection = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
		
		ValueTask<IPaginationCollection<dynamic>> QueryAsync(
			Expression<Func<dynamic, bool>> expression, 
			int? skip = null, 
			int? limit = null, 
			bool? withCount = null, 
			Sorting sorting = null, 
			IDictionary<string, bool> selectFields = null,
			Locale? locale = null, 
			CancellationToken cancellationToken = default);
        
        TField[] Distinct<TField>(string distinctBy, string query = null);
		
        Task<TField[]> DistinctAsync<TField>(string distinctBy, string query = null, CancellationToken cancellationToken = default);
		
        TField[] Distinct<TField>(string distinctBy, Expression<Func<dynamic, bool>> expression);
		
        Task<TField[]> DistinctAsync<TField>(string distinctBy, Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default);
        
        dynamic Aggregate(string query);
		
        ValueTask<dynamic> AggregateAsync(string query, CancellationToken cancellationToken = default);

        Task<IEnumerable<IIndexDefinition>> GetIndexesAsync(CancellationToken cancellationToken = default);
        
        Task<string> CreateIndexAsync(IIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
		
        Task<string[]> CreateManyIndexAsync(IEnumerable<IIndexDefinition> indexDefinitions, CancellationToken cancellationToken = default);

        Task<string> CreateSingleIndexAsync(string fieldName, SortDirection? direction = null, CancellationToken cancellationToken = default);
        
        Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition, CancellationToken cancellationToken = default);

        Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default);
        
        Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
    }
}