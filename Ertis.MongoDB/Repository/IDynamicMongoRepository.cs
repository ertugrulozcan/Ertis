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
        IPaginationCollection<dynamic> Query(
            string query,
            int? skip = null,
            int? limit = null,
            bool? withCount = null,
            string sortField = null,
            SortDirection? sortDirection = null,
            IDictionary<string, bool> selectFields = null);

        IPaginationCollection<dynamic> Query(
            Expression<Func<dynamic, bool>> expression,
            int? skip = null,
            int? limit = null,
            bool? withCount = null,
            string sortField = null,
            SortDirection? sortDirection = null,
            IDictionary<string, bool> selectFields = null);

        ValueTask<IPaginationCollection<dynamic>> QueryAsync(
            string query,
            int? skip = null,
            int? limit = null,
            bool? withCount = null,
            string sortField = null,
            SortDirection? sortDirection = null,
            IDictionary<string, bool> selectFields = null,
            CancellationToken cancellationToken = default);

        ValueTask<IPaginationCollection<dynamic>> QueryAsync(
            Expression<Func<dynamic, bool>> expression,
            int? skip = null,
            int? limit = null,
            bool? withCount = null,
            string sortField = null,
            SortDirection? sortDirection = null,
            IDictionary<string, bool> selectFields = null,
            CancellationToken cancellationToken = default);
        
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