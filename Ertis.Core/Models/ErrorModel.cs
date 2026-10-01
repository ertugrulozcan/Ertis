using System.Text.Json.Serialization;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Core.Models;

public class ErrorModel
{
	#region Properties
	
	[JsonPropertyName("message")]
	public required string Message { get; set; }
	
	[JsonPropertyName("errorCode")]
	public required string ErrorCode { get; set; }
	
	[JsonPropertyName("statusCode")]
	public required int StatusCode { get; set; }
	
	#endregion
}

public class ErrorModel<T> : ErrorModel
{
	#region Properties
	
	[JsonPropertyName("data")]
	public T? Data { get; set; }
	
	#endregion
}