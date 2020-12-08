using System;
using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.Extensions.AspNetCore.Exceptions
{
	public class NegativeSkipException : ErtisException
	{
		#region Constructors

		public NegativeSkipException() : base(
			HttpStatusCode.BadRequest, 
			"The skip value can not be negative",
			"NegativeSkipError")
		{ }
		
		public NegativeSkipException(Exception innerException) : base(
			HttpStatusCode.BadRequest, 
			"The skip value can not be negative",
			"NegativeSkipError",
			innerException)
		{ }

		#endregion
	}
}