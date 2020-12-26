namespace Ertis.Core.Models.Response
{
	public class ErrorModel
	{
		#region Properties

		public string Message { get; set; }
		
		public string ErrorCode { get; set; }
		
		public int StatusCode { get; set; }

		#endregion
	}
	
	public class ErrorModel<T> : ErrorModel
	{
		#region Properties

		public T Data { get; set; }

		#endregion
	}
}