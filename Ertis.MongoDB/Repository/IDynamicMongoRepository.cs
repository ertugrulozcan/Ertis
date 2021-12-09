using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Repository;

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
    }
}