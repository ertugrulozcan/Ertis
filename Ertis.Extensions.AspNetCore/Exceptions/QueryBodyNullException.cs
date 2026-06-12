using System.Net;
using Ertis.Core.Exceptions;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace Ertis.Extensions.AspNetCore.Exceptions;

public class QueryBodyNullException : ErtisException
{
	#region Constructors
	
	public QueryBodyNullException() : base(
		HttpStatusCode.BadRequest,
		"The request body is a required field for _query endpoint",
		"QueryBodyNullError")
	{}
	
	public QueryBodyNullException(Exception innerException) : base(
		HttpStatusCode.BadRequest,
		"The request body is a required field for _query endpoint",
		"QueryBodyNullError",
		innerException)
	{}
	
	#endregion
}