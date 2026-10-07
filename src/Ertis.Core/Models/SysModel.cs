using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Core.Models;

public class SysModel
{
	#region Properties
	
	[JsonPropertyName("created_at")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public DateTime? CreatedAt { get; set; }
	
	[JsonPropertyName("created_by")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? CreatedBy { get; set; }
	
	[JsonPropertyName("modified_at")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public DateTime? ModifiedAt { get; set; }
	
	[JsonPropertyName("modified_by")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ModifiedBy { get; set; }
	
	#endregion
}