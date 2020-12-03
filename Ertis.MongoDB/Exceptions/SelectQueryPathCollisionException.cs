using System;
using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.MongoDB.Exceptions
{
	public class SelectQueryPathCollisionException : ErtisException
	{
		#region Constructors

		public SelectQueryPathCollisionException(string innerMessage) : base(
			HttpStatusCode.BadRequest, 
			"Path collision error, you can not attempts to project both nested fields. " + $"({innerMessage})",
			"SelectQueryPathCollisionError")
		{ }
		
		public SelectQueryPathCollisionException(Exception innerException) : base(
			HttpStatusCode.BadRequest, 
			"Path collision error, you can not attempts to project both nested fields. " + $"({innerException.Message})",
			"SelectQueryPathCollisionError",
			innerException)
		{ }

		#endregion
	}
}