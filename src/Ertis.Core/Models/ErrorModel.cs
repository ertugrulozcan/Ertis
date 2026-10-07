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
	
	/// <summary>
	/// Additional error data; omitted from the JSON when null.
	/// </summary>
	/// <remarks>
	/// Use a nullable type argument for value types (e.g. <c>ErrorModel&lt;int?&gt;</c>): for an unconstrained
	/// <typeparamref name="T"/>, <c>T?</c> of a value type is the type itself, so <c>ErrorModel&lt;int&gt;</c>
	/// throws an <see cref="System.InvalidOperationException"/> when serialized with System.Text.Json.
	/// </remarks>
	[JsonPropertyName("data")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public T? Data { get; set; }
	
	#endregion
}