using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.Data.Repository;
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
	}
}