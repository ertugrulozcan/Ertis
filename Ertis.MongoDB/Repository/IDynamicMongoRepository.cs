using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            IDictionary<string, bool> selectFields = null);

        ValueTask<IPaginationCollection<dynamic>> QueryAsync(
            Expression<Func<dynamic, bool>> expression,
            int? skip = null,
            int? limit = null,
            bool? withCount = null,
            string sortField = null,
            SortDirection? sortDirection = null,
            IDictionary<string, bool> selectFields = null);
        
        dynamic Aggregate(string query);
		
        ValueTask<dynamic> AggregateAsync(string query);

        Task<IEnumerable<IIndexDefinition>> GetIndexesAsync();
        
        Task<string> CreateIndexAsync(IIndexDefinition indexDefinition);
		
        Task<string[]> CreateManyIndexAsync(IEnumerable<IIndexDefinition> indexDefinitions);

        Task<string> CreateSingleIndexAsync(string fieldName, SortDirection? direction = null);
        
        Task<string> CreateSingleIndexAsync(SingleIndexDefinition indexDefinition);

        Task<string> CreateCompoundIndexAsync(IDictionary<string, SortDirection> indexFieldDefinitions);
        
        Task<string> CreateCompoundIndexAsync(CompoundIndexDefinition indexDefinition);
    }
}