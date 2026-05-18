using System.Net;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Core.Models;

public interface IResponseResult
{
	#region Properties
	
	bool IsSuccess { get; }
	
	HttpStatusCode? StatusCode { get; }
	
	IDictionary<string, string>? Headers { get; }
	
	string? Message { get; set; }
	
	byte[]? RawData { get; set; }
	
	string? Json { get; set; }
	
	Exception? Exception { get; set; }
	
	#endregion
}

public interface IResponseResult<out T> : IResponseResult
{
	#region Properties
	
	T? Data { get; }
	
	#endregion
}

[Serializable]
public class ResponseResult<T> : IResponseResult<T>
{
	#region Fields
	
	private bool isSuccess;
	
	#endregion
	
	#region Properties
	
	public bool IsSuccess
	{
		get
		{
			if (this.StatusCode != null)
			{
				var code = (int)this.StatusCode;
				return code is >= 200 and < 300;
			}
			else
			{
				return this.isSuccess;
			}
		}
		private set
		{
			this.isSuccess = value;
			if (value)
			{
				this.StatusCode = HttpStatusCode.OK;
			}
			else
			{
				this.StatusCode = null;
			}
		}
	}
	
	public HttpStatusCode? StatusCode { get; private set; }
	
	public IDictionary<string, string>? Headers { get; set; }
	
	public string? Message { get; set; }
	
	public T? Data { get; set; }
	
	public byte[]? RawData { get; set; }
	
	public string? Json { get; set; }
	
	public Exception? Exception { get; set; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor 1
	/// </summary>
	/// <param name="isSuccess"></param>
	public ResponseResult(bool isSuccess)
	{
		this.IsSuccess = isSuccess;
	}
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="isSuccess"></param>
	/// <param name="message"></param>
	public ResponseResult(bool isSuccess, string message)
	{
		this.IsSuccess = isSuccess;
		this.Message = message;
	}
	
	/// <summary>
	/// Constructor 3
	/// </summary>
	/// <param name="httpCode"></param>
	public ResponseResult(HttpStatusCode httpCode)
	{
		this.StatusCode = httpCode;
	}
	
	/// <summary>
	/// Constructor 4
	/// </summary>
	/// <param name="httpCode"></param>
	/// <param name="message"></param>
	public ResponseResult(HttpStatusCode httpCode, string message)
	{
		this.StatusCode = httpCode;
		this.Message = message;
	}
	
	#endregion
	
	#region Methods
	
	public override string ToString()
	{
		return this.Message ?? $"No error message (Status {this.StatusCode?.ToString() ?? "No status code"})";
	}
	
	#endregion
}

[Serializable]
public class ResponseResult : ResponseResult<object>
{
	#region Constructors
	
	/// <summary>
	/// Constructor 1
	/// </summary>
	/// <param name="isSuccess"></param>
	public ResponseResult(bool isSuccess) : base(isSuccess)
	{ }
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="isSuccess"></param>
	/// <param name="message"></param>
	public ResponseResult(bool isSuccess, string message) : base(isSuccess, message)
	{ }
	
	/// <summary>
	/// Constructor 3
	/// </summary>
	/// <param name="httpCode"></param>
	public ResponseResult(HttpStatusCode httpCode) : base(httpCode)
	{ }
	
	/// <summary>
	/// Constructor 4
	/// </summary>
	/// <param name="httpCode"></param>
	/// <param name="message"></param>
	public ResponseResult(HttpStatusCode httpCode, string message) : base(httpCode, message)
	{ }
	
	#endregion
}