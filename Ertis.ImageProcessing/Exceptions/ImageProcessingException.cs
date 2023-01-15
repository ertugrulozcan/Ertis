using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.ImageProcessing.Exceptions;

public class ImageProcessingException : ErtisException
{
	#region Constructors

	public ImageProcessingException(HttpStatusCode httpStatusCode, string? message = null, string? errorCode = null, Exception? exception = null) : 
		base(httpStatusCode, 
			message, 
			errorCode ?? "ImageProcessingException", 
			exception)
	{ }

	#endregion
}