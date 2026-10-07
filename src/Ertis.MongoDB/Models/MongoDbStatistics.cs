using System.Text.Json.Serialization;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.MongoDB.Models;

public class MongoDbStatistics
{
	#region Properties
	
	[JsonPropertyName("db")]
	public string? DatabaseName { get; set; }
	
	[JsonPropertyName("collections")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public long? CollectionCount { get; set; }
	
	[JsonPropertyName("views")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public long? ViewCount { get; set; }
	
	[JsonPropertyName("objects")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public long? ObjectCount { get; set; }
	
	[JsonPropertyName("avgObjSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? AverageObjectSize { get; set; }
	
	[JsonPropertyName("dataSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? DataSize { get; set; }
	
	[JsonPropertyName("storageSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? StorageSize { get; set; }
	
	[JsonPropertyName("indexes")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public long? IndexCount { get; set; }
	
	[JsonPropertyName("indexSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? IndexSize { get; set; }
	
	[JsonPropertyName("totalSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? TotalSize { get; set; }
	
	[JsonPropertyName("scaleFactor")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? ScaleFactor { get; set; }
	
	[JsonPropertyName("fsUsedSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? FileStorageUsedSize { get; set; }
	
	[JsonPropertyName("fsTotalSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? FileStorageTotalSize { get; set; }
	
	[JsonPropertyName("ok")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public double? State { get; set; }
	
	#endregion
}