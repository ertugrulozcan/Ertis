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
		public string DatabaseName { get; set; }
		
		[JsonProperty("collections", NullValueHandling = NullValueHandling.Ignore)]
		public int? CollectionCount { get; set; }
		
		[JsonProperty("views", NullValueHandling = NullValueHandling.Ignore)]
		public int? ViewCount { get; set; }
		
		[JsonProperty("objects", NullValueHandling = NullValueHandling.Ignore)]
		public int? ObjectCount { get; set; }
		
		[JsonProperty("avgObjSize", NullValueHandling = NullValueHandling.Ignore)]
		public double? AverageObjectSize { get; set; }
		
		[JsonProperty("dataSize", NullValueHandling = NullValueHandling.Ignore)]
		public double? DataSize { get; set; }
		
		[JsonProperty("storageSize", NullValueHandling = NullValueHandling.Ignore)]
		public double? StorageSize { get; set; }
		
		[JsonProperty("indexes", NullValueHandling = NullValueHandling.Ignore)]
		public int? IndexCount { get; set; }
		
		[JsonProperty("indexSize", NullValueHandling = NullValueHandling.Ignore)]
		public double? IndexSize { get; set; }
		
		[JsonProperty("totalSize", NullValueHandling = NullValueHandling.Ignore)]
		public double? TotalSize { get; set; }
		
		[JsonProperty("scaleFactor", NullValueHandling = NullValueHandling.Ignore)]
		public double? ScaleFactor { get; set; }
		
		[JsonProperty("fsUsedSize", NullValueHandling = NullValueHandling.Ignore)]
		public double? FileStorageUsedSize { get; set; }
		
		[JsonProperty("fsTotalSize", NullValueHandling = NullValueHandling.Ignore)]
		public double? FileStorageTotalSize { get; set; }
		
		[JsonProperty("ok", NullValueHandling = NullValueHandling.Ignore)]
		public double? State { get; set; }

		#endregion
	}
}