using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.ImageProcessing.Exceptions;

public class ImageProcessingException(
	HttpStatusCode httpStatusCode,
	string? message = null,
	string? errorCode = null,
	Exception? exception = null)
	: ErtisException(httpStatusCode,
		message ?? string.Empty,
		errorCode ?? "ImageProcessingException",
		exception);