using System.Collections.Generic;

namespace Ertis.Net.Http
{
	public interface IQueryString : IEnumerable<object>
	{
		IQueryString Add(string key, object value);

		IQueryString Add(KeyValuePair<string, object> pair);
		
		IQueryString Remove(string key);

		IDictionary<string, object> ToDictionary();
	}
}