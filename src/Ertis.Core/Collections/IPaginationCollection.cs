using System.Text.Json.Serialization;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Ertis.Core.Collections;

public interface IPaginationCollection<out T>
{
	#region Properties
	
	[JsonPropertyName("count")]
	long Count { get; }
	
	[JsonPropertyName("items")]
	IEnumerable<T> Items { get; }
	
	#endregion
}

public class PaginationCollection<T> : IPaginationCollection<T>
{
	#region Properties
	
	[JsonPropertyName("count")]
	public long Count { get; set; }
	
	[JsonPropertyName("items")]
	public required IEnumerable<T> Items { get; set; }
	
	#endregion
}