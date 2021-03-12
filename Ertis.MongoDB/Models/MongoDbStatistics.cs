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
		
		[JsonProperty("collections")]
		public int CollectionCount { get; set; }
		
		[JsonProperty("views")]
		public int ViewCount { get; set; }
		
		[JsonProperty("objects")]
		public int ObjectCount { get; set; }
		
		[JsonProperty("avgObjSize")]
		public double AverageObjectSize { get; set; }
		
		[JsonProperty("dataSize")]
		public double DataSize { get; set; }
		
		[JsonProperty("storageSize")]
		public double StorageSize { get; set; }
		
		[JsonProperty("indexes")]
		public int IndexCount { get; set; }
		
		[JsonProperty("indexSize")]
		public double IndexSize { get; set; }
		
		[JsonProperty("totalSize")]
		public double TotalSize { get; set; }
		
		[JsonProperty("scaleFactor")]
		public double ScaleFactor { get; set; }
		
		[JsonProperty("fsUsedSize")]
		public double FileStorageUsedSize { get; set; }
		
		[JsonProperty("fsTotalSize")]
		public double FileStorageTotalSize { get; set; }
		
		[JsonProperty("ok")]
		public double State { get; set; }

		#endregion
	}
}