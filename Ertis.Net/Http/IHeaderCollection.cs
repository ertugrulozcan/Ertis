using System.Collections.Generic;

namespace Ertis.Net.Http
{
	public interface IHeaderCollection : IEnumerable<object>
	{
		IHeaderCollection Add(string key, object value);

		IHeaderCollection Add(KeyValuePair<string, object> pair);
		
		IHeaderCollection Remove(string key);

		IDictionary<string, object> ToDictionary();
	}
}