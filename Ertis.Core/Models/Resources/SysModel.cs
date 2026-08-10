using System.Text.Json.Serialization;

namespace Ertis.Core.Models.Resources;

public class SysModel
{
	#region Properties
	
	[JsonPropertyName("created_at")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("created_at", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public DateTime? CreatedAt { get; set; }
	
	[JsonPropertyName("created_by")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("created_by", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string? CreatedBy { get; set; }
	
	[JsonPropertyName("modified_at")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("modified_at", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public DateTime? ModifiedAt { get; set; }
	
	[JsonPropertyName("modified_by")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("modified_by", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string? ModifiedBy { get; set; }
	
	#endregion
}