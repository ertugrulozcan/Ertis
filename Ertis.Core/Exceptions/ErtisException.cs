using System.Net;
using System.Text.Json.Serialization;
using JsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;
using Ertis.Core.Models.Response;

namespace Ertis.Core.Exceptions;

// ReSharper disable once UnusedType.Global
public class ErtisException<T> : ErtisException
{
	#region Properties
	
	[JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("errors")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public T? Payload { get; set; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="errorCode"></param>
	protected ErtisException(HttpStatusCode statusCode, string errorCode) : base(statusCode, errorCode)
	{}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	protected ErtisException(HttpStatusCode statusCode, string message, string errorCode) : base(statusCode, message, errorCode)
	{}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	/// <param name="innerException"></param>
	protected ErtisException(HttpStatusCode statusCode, string message, string errorCode, Exception innerException) : base(statusCode, message, errorCode, innerException)
	{}
	
	#endregion
}

public class ErtisException : HttpStatusCodeException, IHasErrorModel
{
	#region Properties
	
	[JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("errorCode")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ErrorCode { get; }
	
	[JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("error")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public ErrorModel Error =>
		new()
		{
			Message = this.Message,
			ErrorCode = this.ErrorCode,
			StatusCode = (int)this.StatusCode
		};
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor 1
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="errorCode"></param>
	protected ErtisException(HttpStatusCode statusCode, string errorCode) : base(statusCode)
	{
		this.ErrorCode = errorCode;
	}
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	protected ErtisException(HttpStatusCode statusCode, string message, string errorCode) : base(statusCode, message)
	{
		this.ErrorCode = errorCode;
	}
	
	/// <summary>
	/// Constructor 3
	/// </summary>
	/// <param name="statusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	/// <param name="innerException"></param>
	protected ErtisException(HttpStatusCode statusCode, string message, string errorCode, Exception? innerException = null) : base(statusCode, message, innerException)
	{
		this.ErrorCode = errorCode;
	}
	
	#endregion
}