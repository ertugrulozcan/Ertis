using System.Net;
using System.Text.Json.Serialization;
using JsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;

namespace Ertis.Core.Exceptions;

// ReSharper disable once UnusedType.Global
public class ValidationException : ErtisException
{
	#region Properties
	
	[JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("errors")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public IEnumerable<string>? Errors { get; set; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="errorCode"></param>
	public ValidationException(HttpStatusCode statusCode, string errorCode) : base(statusCode, errorCode)
	{}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	public ValidationException(HttpStatusCode statusCode, string message, string errorCode) : base(statusCode, message, errorCode)
	{}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	/// <param name="innerException"></param>
	public ValidationException(HttpStatusCode statusCode, string message, string errorCode, Exception innerException) : base(statusCode, message, errorCode, innerException)
	{}
	
	#endregion
}