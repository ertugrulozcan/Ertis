using System.Text.Json.Serialization;

namespace Ertis.Core.Collections;

public interface IPaginationCollection<out T>
{
	#region Properties
	
	[JsonPropertyName("count")]
	[Newtonsoft.Json.JsonProperty("count")]
	long Count { get; }
	
	[JsonPropertyName("items")]
	[Newtonsoft.Json.JsonProperty("items")]
	IEnumerable<T> Items { get; }
	
	#endregion
}

public class PaginationCollection<T> : IPaginationCollection<T>
{
	#region Properties
	
	[JsonPropertyName("count")]
	[Newtonsoft.Json.JsonProperty("count")]
	public long Count { get; set; }
	
	[JsonPropertyName("items")]
	[Newtonsoft.Json.JsonProperty("items")]
	public required IEnumerable<T> Items { get; set; }
	
	#endregion
}