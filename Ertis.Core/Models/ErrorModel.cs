// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Core.Models;

public interface IHasErrorModel
{
	ErrorModel Error { get; }
}

public class ErrorModel
{
	#region Properties
	
	public required string Message { get; set; }
	
	public required string ErrorCode { get; set; }
	
	public int StatusCode { get; set; }
	
	#endregion
}

public class ErrorModel<T> : ErrorModel
{
	#region Properties
	
	public T? Data { get; set; }
	
	#endregion
}