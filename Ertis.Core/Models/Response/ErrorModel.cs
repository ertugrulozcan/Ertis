using System.Text.Json.Serialization;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Core.Models.Response;

public class ErrorModel
{
	#region Properties
	
	[JsonPropertyName("message")]
	[Newtonsoft.Json.JsonProperty("message")]
	public required string Message { get; set; }
	
	[JsonPropertyName("errorCode")]
	[Newtonsoft.Json.JsonProperty("errorCode")]
	public required string ErrorCode { get; set; }
	
	[JsonPropertyName("statusCode")]
	[Newtonsoft.Json.JsonProperty("statusCode")]
	public required int StatusCode { get; set; }
	
	#endregion
}

public class ErrorModel<T> : ErrorModel
{
	#region Properties
	
	[JsonPropertyName("data")]
	[Newtonsoft.Json.JsonProperty("data")]
	public T? Data { get; set; }
	
	#endregion
}