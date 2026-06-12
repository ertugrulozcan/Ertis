using System.Text.Json.Serialization;
using JsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;
using DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling;

namespace Ertis.Core.Models.Response;

public class ErrorModel
{
	#region Properties
	
	[JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("message")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Message { get; set; }
	
	[JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("errorCode")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ErrorCode { get; set; }
	
	[JsonProperty("statusCode", DefaultValueHandling = DefaultValueHandling.Ignore)]
	[JsonPropertyName("statusCode")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public int StatusCode { get; set; }
	
	#endregion
}

public class ErrorModel<T> : ErrorModel
{
	#region Properties
	
	[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("data")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public T? Data { get; set; }
	
	#endregion
}