using System.Linq.Expressions;
using Ertis.Core.Collections;
using Ertis.Data.Repository;
using Ertis.MongoDB.Models;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.MongoDB.Repository;

public interface IDynamicMongoRepository : IDynamicRepository<string>
{
    string CollectionName { get; }
    
    IPaginationCollection<dynamic> Find(
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
    
    IPaginationCollection<dynamic> Find(
        int? skip = null, 
        int? limit = null, 
        bool? withCount = null, 
        Sorting? sorting = null, 
        IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	Task<IPaginationCollection<dynamic>> FindAsync(
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	Task<IPaginationCollection<dynamic>> FindAsync(
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	IPaginationCollection<dynamic> Find(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	IPaginationCollection<dynamic> Find(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	Task<IPaginationCollection<dynamic>> FindAsync(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	Task<IPaginationCollection<dynamic>> FindAsync(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	IPaginationCollection<dynamic> Find(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	IPaginationCollection<dynamic> Find(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	Task<IPaginationCollection<dynamic>> FindAsync(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	Task<IPaginationCollection<dynamic>> FindAsync(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	IPaginationCollection<dynamic> Query(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	IPaginationCollection<dynamic> Query(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null,  
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	IPaginationCollection<dynamic> Query(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	IPaginationCollection<dynamic> Query(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null);
	
	Task<IPaginationCollection<dynamic>> QueryAsync(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	Task<IPaginationCollection<dynamic>> QueryAsync(
		string query, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	Task<IPaginationCollection<dynamic>> QueryAsync(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		string? orderBy = null, 
		SortDirection? sortDirection = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
	
	Task<IPaginationCollection<dynamic>> QueryAsync(
		Expression<Func<dynamic, bool>> expression, 
		int? skip = null, 
		int? limit = null, 
		bool? withCount = null, 
		Sorting? sorting = null, 
		IDictionary<string, bool>? selectFields = null,
		IndexOptions? indexOptions = null,
		CollationOptions? collationOptions = null, 
		CancellationToken cancellationToken = default);
    
	long Count(IndexOptions? indexOptions = null);
	
	Task<long> CountAsync(IndexOptions? indexOptions = null, CancellationToken cancellationToken = default);
	
	long Count(string query, IndexOptions? indexOptions = null);
	
	Task<long> CountAsync(string query, IndexOptions? indexOptions = null, CancellationToken cancellationToken = default);
	
	long Count(Expression<Func<dynamic, bool>> expression, IndexOptions? indexOptions = null);
	
	Task<long> CountAsync(Expression<Func<dynamic, bool>> expression, IndexOptions? indexOptions = null, CancellationToken cancellationToken = default);
	
	long EstimatedCount();
	
	Task<long> EstimatedCountAsync(CancellationToken cancellationToken = default);
	
    TField[] Distinct<TField>(string distinctBy, string? query = null);
	
    Task<TField[]> DistinctAsync<TField>(string distinctBy, string? query = null, CancellationToken cancellationToken = default);
	
    TField[] Distinct<TField>(string distinctBy, Expression<Func<dynamic, bool>> expression);
	
    Task<TField[]> DistinctAsync<TField>(string distinctBy, Expression<Func<dynamic, bool>> expression, CancellationToken cancellationToken = default);
    
    dynamic Aggregate(string query, IndexOptions? indexOptions = null, CollationOptions? collationOptions = null);
	
    Task<dynamic> AggregateAsync(string query, IndexOptions? indexOptions = null, CollationOptions? collationOptions = null, CancellationToken cancellationToken = default);
	
    Task<IEnumerable<IIndexDefinition>> GetIndexesAsync(CancellationToken cancellationToken = default);
    
    Task<string> CreateIndexAsync(IIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
	
    Task<string[]> CreateManyIndexAsync(IEnumerable<IIndexDefinition> indexDefinitions, CancellationToken cancellationToken = default);
	
    Task<string> CreateSingleIndexAsync(string fieldName, SortDirection? direction = null, CancellationToken cancellationToken = default);
    
    Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
	
    Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions, CancellationToken cancellationToken = default);
    
    Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
    
    Task<string> CreateTextIndexAsync(TextIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
	
	Task<string> CreateTTLIndexAsync(TTLIndexDefinition indexDefinition, CancellationToken cancellationToken = default);
}