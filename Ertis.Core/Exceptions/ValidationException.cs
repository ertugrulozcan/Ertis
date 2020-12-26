using System;
using System.Collections.Generic;
using System.Net;

namespace Ertis.Core.Exceptions
{
	public class ValidationException : ErtisException
	{
		#region Properties

		public IEnumerable<string> Errors { get; set; }

		#endregion
		
		#region Constructors

		public ValidationException(HttpStatusCode statusCode, string errorCode) : base(statusCode, errorCode)
		{
		}

		public ValidationException(HttpStatusCode statusCode, string message, string errorCode) : base(statusCode, message, errorCode)
		{
		}

		public ValidationException(HttpStatusCode statusCode, string message, string errorCode, Exception innerException) : base(statusCode, message, errorCode, innerException)
		{
		}

		#endregion
	}
}