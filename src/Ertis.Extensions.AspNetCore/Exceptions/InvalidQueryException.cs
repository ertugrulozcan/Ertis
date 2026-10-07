using System.Net;
using Ertis.Core.Exceptions;

// ReSharper disable UnusedType.Global
namespace Ertis.Extensions.AspNetCore.Exceptions;

public class InvalidQueryException : ErtisException
{
	#region Constructors
	
	public InvalidQueryException() : base(
		HttpStatusCode.BadRequest, 
		"The request body is not a valid JSON document",
		"InvalidQuery")
	{ }
	
	public InvalidQueryException(Exception innerException) : base(
		HttpStatusCode.BadRequest, 
		"The request body is not a valid JSON document",
		"InvalidQuery",
		innerException)
	{ }
	
	#endregion
}