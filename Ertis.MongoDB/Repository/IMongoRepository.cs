using System.Collections.Generic;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Data.Models;
using Ertis.Data.Repository;

namespace Ertis.MongoDB.Repository
{
	public interface IMongoRepository<TEntity> : IRepository<TEntity, string> where TEntity : IEntity<string>
	{
		IPaginationCollection<dynamic> Query(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, IDictionary<string, bool> selectFields = null);

		Task<IPaginationCollection<dynamic>> QueryAsync(string query, int? skip = null, int? limit = null, bool? withCount = null, string sortField = null, SortDirection? sortDirection = null, IDictionary<string, bool> selectFields = null);
	}
}