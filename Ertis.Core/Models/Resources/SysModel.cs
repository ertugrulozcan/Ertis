using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Ertis.Core.Models.Resources
{
	public class SysModel
	{
		#region Properties
		
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("created_at")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? CreatedAt { get; set; }
		
		[JsonProperty("created_by", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("created_by")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string CreatedBy { get; set; }
		
		[JsonProperty("modified_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("modified_at")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? ModifiedAt { get; set; }
		
		[JsonProperty("modified_by", NullValueHandling = NullValueHandling.Ignore)]
		[JsonPropertyName("modified_by")]
		[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string ModifiedBy { get; set; }

		#endregion
	}
}