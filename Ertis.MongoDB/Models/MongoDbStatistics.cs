using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Ertis.MongoDB.Models
{
	public class MongoDbStatistics
	{
		/*
		{
		    "db" : "ertisauth",
		    "collections" : 5,
		    "views" : 0,
		    "objects" : 9,
		    "avgObjSize" : 741.333333333333,
		    "dataSize" : 6672.0,
		    "storageSize" : 135168.0,
		    "indexes" : 5,
		    "indexSize" : 118784.0,
		    "totalSize" : 253952.0,
		    "scaleFactor" : 1.0,
		    "fsUsedSize" : 32510013440.0,
		    "fsTotalSize" : 62725623808.0,
		    "ok" : 1.0
		}
		*/
		
		#region Properties

		[JsonProperty("db")]
		[JsonPropertyName("db")]
		public string DatabaseName { get; set; }
		
		[JsonProperty("collections", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("collections")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? CollectionCount { get; set; }
		
		[JsonProperty("views", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("views")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? ViewCount { get; set; }
		
		[JsonProperty("objects", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("objects")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? ObjectCount { get; set; }
		
		[JsonProperty("avgObjSize", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("avgObjSize")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? AverageObjectSize { get; set; }
		
		[JsonProperty("dataSize", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("dataSize")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? DataSize { get; set; }
		
		[JsonProperty("storageSize", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("storageSize")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? StorageSize { get; set; }
		
		[JsonProperty("indexes", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("indexes")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? IndexCount { get; set; }
		
		[JsonProperty("indexSize", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("indexSize")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? IndexSize { get; set; }
		
		[JsonProperty("totalSize", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("totalSize")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? TotalSize { get; set; }
		
		[JsonProperty("scaleFactor", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("scaleFactor")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? ScaleFactor { get; set; }
		
		[JsonProperty("fsUsedSize", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("fsUsedSize")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? FileStorageUsedSize { get; set; }
		
		[JsonProperty("fsTotalSize", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("fsTotalSize")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? FileStorageTotalSize { get; set; }
		
		[JsonProperty("ok", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("ok")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? State { get; set; }

		#endregion
	}
}