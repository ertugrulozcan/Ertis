using System;
using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.MongoDB.Exceptions
{
	public class SelectQueryInclusionException : ErtisException
	{
		#region Constructors

		public SelectQueryInclusionException() : base(
			HttpStatusCode.BadRequest, 
			"A select query projection cannot contain both include and exclude specifications! (with the exception of the _id field)",
			"SelectQueryInclusionError")
		{ }
		
		public SelectQueryInclusionException(Exception innerException) : base(
			HttpStatusCode.BadRequest, 
			"A select query projection cannot contain both include and exclude specifications! (with the exception of the _id field)",
			"SelectQueryInclusionError",
			innerException)
		{ }

		#endregion
	}
}