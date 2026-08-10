using System.Text.Json.Serialization;

namespace Ertis.MongoDB.Models;

public class MongoDbStatistics
{
	#region Properties
	
	[JsonPropertyName("db")]
	[Newtonsoft.Json.JsonProperty("db")]
	public string? DatabaseName { get; set; }
	
	[JsonPropertyName("collections")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("collections", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public int? CollectionCount { get; set; }
	
	[JsonPropertyName("views")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("views", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public int? ViewCount { get; set; }
	
	[JsonPropertyName("objects")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("objects", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public int? ObjectCount { get; set; }
	
	[JsonPropertyName("avgObjSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("avgObjSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? AverageObjectSize { get; set; }
	
	[JsonPropertyName("dataSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("dataSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? DataSize { get; set; }
	
	[JsonPropertyName("storageSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("storageSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? StorageSize { get; set; }
	
	[JsonPropertyName("indexes")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("indexes", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public int? IndexCount { get; set; }
	
	[JsonPropertyName("indexSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("indexSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? IndexSize { get; set; }
	
	[JsonPropertyName("totalSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("totalSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? TotalSize { get; set; }
	
	[JsonPropertyName("scaleFactor")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("scaleFactor", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? ScaleFactor { get; set; }
	
	[JsonPropertyName("fsUsedSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("fsUsedSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? FileStorageUsedSize { get; set; }
	
	[JsonPropertyName("fsTotalSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("fsTotalSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? FileStorageTotalSize { get; set; }
	
	[JsonPropertyName("ok")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("ok", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public double? State { get; set; }
	
	#endregion
}