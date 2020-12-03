using System.Net;

namespace Ertis.Extensions.AspNetCore.Exceptions
{
	public class SelectQueryProjectionException : ErtisException
	{
		#region Constructors

		public SelectQueryProjectionException() : base(
			HttpStatusCode.BadRequest, 
			"A select query projection cannot contain both include and exclude specifications! (with the exception of the _id field)",
			"SelectQueryProjectionError")
		{ }

		#endregion
	}
}