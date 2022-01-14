using System.Collections.Generic;

namespace Ertis.Net.Http
{
	public interface IHeaderCollection : IEnumerable<object>
	{
		IEnumerable<string> Keys { get; }
		
		IEnumerable<object> Values { get; }
		
		IHeaderCollection Add(string key, object value);

		IHeaderCollection Add(KeyValuePair<string, object> pair);
		
		IHeaderCollection Remove(string key);

		IDictionary<string, object> ToDictionary();

		bool ContainsKey(string key);
	}
}