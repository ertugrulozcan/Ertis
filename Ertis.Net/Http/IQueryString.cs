using System.Collections.Generic;

namespace Ertis.Net.Http
{
	public interface IQueryString : IDictionary<string, object>
	{
		new IQueryString Add(string key, object value);

		new IQueryString Add(KeyValuePair<string, object> pair);
		
		new IQueryString Remove(string key);
	}
}