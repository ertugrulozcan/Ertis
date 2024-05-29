using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Ertis.Core.Collections
{
	public interface IPaginationCollection<out T>
	{
		[JsonProperty("count")]
		[JsonPropertyName("count")]
		long Count { get; }
		
		[JsonProperty("items")]
		[JsonPropertyName("items")]
		IEnumerable<T> Items { get; }
	}

	public class PaginationCollection<T> : IPaginationCollection<T>
	{
		[JsonProperty("count")]
		[JsonPropertyName("count")]
		public long Count { get; set; }
		
		[JsonProperty("items")]
		[JsonPropertyName("items")]
		public IEnumerable<T> Items { get; set; }
	}
}