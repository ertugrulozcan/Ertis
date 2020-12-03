namespace Ertis.Extensions.AspNetCore.Models.Response
{
	public class ErrorModel
	{
		#region Properties

		public string Message { get; set; }
		
		public string ErrorCode { get; set; }
		
		public int StatusCode { get; set; }

		#endregion
	}
}