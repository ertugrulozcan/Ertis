using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.ImageProcessing.Exceptions;

public class ImageProcessingException : ErtisException
{
	#region Constructors
	
	/// <summary>
	/// Constructor 1
	/// </summary>
	/// <param name="httpStatusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	public ImageProcessingException(
		HttpStatusCode httpStatusCode,
		string message,
		string errorCode) : base(httpStatusCode, message, errorCode)
	{ }
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="httpStatusCode"></param>
	/// <param name="message"></param>
	/// <param name="errorCode"></param>
	/// <param name="exception"></param>
	public ImageProcessingException(
		HttpStatusCode httpStatusCode,
		string message,
		string errorCode,
		Exception exception) : base(httpStatusCode, message, errorCode, exception)
	{ }
	
	#endregion
}