using System;
using System.Net;
using Ertis.Core.Models.Response;

namespace Ertis.Core.Exceptions
{
	public class ErtisException : HttpStatusCodeException, IHasErrorModel
	{
		#region Properties

		public string ErrorCode { get; }
		
		public ErrorModel Error =>
			new ErrorModel
			{
				Message = this.Message,
				ErrorCode = this.ErrorCode,
				StatusCode = (int)this.StatusCode
			};

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor 1
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="errorCode"></param>
		protected ErtisException(HttpStatusCode statusCode, string errorCode) : base(statusCode)
		{
			this.ErrorCode = errorCode;
		}

		/// <summary>
		/// Constructor 2
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		/// <param name="errorCode"></param>
		protected ErtisException(HttpStatusCode statusCode, string message, string errorCode) : base(statusCode, message)
		{
			this.ErrorCode = errorCode;
		}

		/// <summary>
		/// Constructor 3
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		/// <param name="errorCode"></param>
		/// <param name="innerException"></param>
		protected ErtisException(HttpStatusCode statusCode, string message, string errorCode, Exception innerException) : base(statusCode, message, innerException)
		{
			this.ErrorCode = errorCode;
		}

		#endregion
	}
}