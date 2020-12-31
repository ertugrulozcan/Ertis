using System.Collections.Generic;

namespace Ertis.Net.Http
{
	public interface IHeaderCollection : IDictionary<string, object>
	{
		new IHeaderCollection Add(string key, object value);

		new IHeaderCollection Add(KeyValuePair<string, object> pair);
		
		new IHeaderCollection Remove(string key);
	}
}