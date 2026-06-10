using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Ertis.Core.Collections;

public interface IPaginationCollection<out T>
{
	#region Properties
	
	[JsonProperty("count")]
	[JsonPropertyName("count")]
	long Count { get; }
	
	[JsonProperty("items")]
	[JsonPropertyName("items")]
	IEnumerable<T> Items { get; }
	
	#endregion
}

public class PaginationCollection<T> : IPaginationCollection<T>
{
	#region Properties
	
	[JsonProperty("count")]
	[JsonPropertyName("count")]
	public long Count { get; set; }
	
	[JsonProperty("items")]
	[JsonPropertyName("items")]
	public IEnumerable<T> Items { get; set; }
	
	#endregion
}