using System.Text.Json.Serialization;

namespace Ertis.Core.Collections;

public interface IPaginationCollection<out T>
{
	[JsonPropertyName("count")]
	long Count { get; }
	
	[JsonPropertyName("items")]
	IEnumerable<T> Items { get; }
}

public class PaginationCollection<T> : IPaginationCollection<T>
{
	#region Properties
	
	[JsonPropertyName("count")]
	public long Count { get; init; }
	
	[JsonPropertyName("items")]
	public required IEnumerable<T> Items { get; init; }
	
	#endregion
}