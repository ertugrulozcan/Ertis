using System;
using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.Extensions.AspNetCore.Exceptions
{
	public class NegativeLimitException : ErtisException
	{
		#region Constructors

		public NegativeLimitException() : base(
			HttpStatusCode.BadRequest, 
			"The limit value can not be negative",
			"NegativeLimitError")
		{ }
		
		public NegativeLimitException(Exception innerException) : base(
			HttpStatusCode.BadRequest, 
			"The limit value can not be negative",
			"NegativeLimitError",
			innerException)
		{ }

		#endregion
	}
}