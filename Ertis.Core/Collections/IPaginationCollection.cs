using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ertis.Core.Collections
{
	public interface IPaginationCollection<out T>
	{
		[JsonProperty("count")]
		long Count { get; }
		
		[JsonProperty("items")]
		IEnumerable<T> Items { get; }
	}

	public class PaginationCollection<T> : IPaginationCollection<T>
	{
		[JsonProperty("count")]
		public long Count { get; set; }
		
		[JsonProperty("items")]
		public IEnumerable<T> Items { get; set; }
	}
}