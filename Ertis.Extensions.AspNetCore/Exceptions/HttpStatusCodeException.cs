using System;
using System.Net;

namespace Ertis.Extensions.AspNetCore.Exceptions
{
	public abstract class HttpStatusCodeException : Exception
	{
		#region Properties

		public HttpStatusCode StatusCode { get; }

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor 1
		/// </summary>
		/// <param name="statusCode"></param>
		protected HttpStatusCodeException(HttpStatusCode statusCode)
		{
			this.StatusCode = statusCode;
		}
		
		/// <summary>
		/// Constructor 2
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		protected HttpStatusCodeException(HttpStatusCode statusCode, string message) : base(message)
		{
			this.StatusCode = statusCode;
		}
		
		/// <summary>
		/// Constructor 3
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		protected HttpStatusCodeException(HttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException)
		{
			this.StatusCode = statusCode;
		}

		#endregion
	}
}