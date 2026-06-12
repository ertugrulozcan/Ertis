using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.MongoDB.Exceptions;

// ReSharper disable once UnusedType.Global
public class WhereQueryExceedSizeException : ErtisException
{
	#region Constructors
	
	public WhereQueryExceedSizeException(string message) : base(
		HttpStatusCode.BadRequest,
		message,
		"WhereQueryExceedSizeError")
	{}
	
	public WhereQueryExceedSizeException(string message, Exception innerException) : base(
		HttpStatusCode.BadRequest,
		message,
		"WhereQueryExceedSizeError",
		innerException)
	{}
	
	#endregion
}