using System.Net;

namespace Ertis.Core.Exceptions;

public class ValidationException : ErtisException
{
	#region Properties
	
	public IEnumerable<string> Errors { get; set; }
	
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