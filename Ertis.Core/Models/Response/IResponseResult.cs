using System.Net;
using System.Text.Json.Serialization;

// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Core.Models.Response;

public interface IResponseResult
{
	#region Properties
	
	[JsonPropertyName("isSuccess")]
	[Newtonsoft.Json.JsonProperty("isSuccess")]
	bool IsSuccess { get; }
	
	[JsonPropertyName("statusCode")]
	[Newtonsoft.Json.JsonProperty("statusCode")]
	HttpStatusCode? StatusCode { get; }
	
	[JsonPropertyName("headers")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("headers", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	IDictionary<string, string>? Headers { get; }
	
	[JsonPropertyName("message")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("message", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	string? Message { get; set; }
	
	[JsonPropertyName("rawData")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("rawData", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	byte[]? RawData { get; set; }
	
	[JsonPropertyName("json")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("json", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	string? Json { get; set; }
	
	[JsonPropertyName("exception")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("exception", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	Exception? Exception { get; set; }
	
	#endregion
}

public interface IResponseResult<out T> : IResponseResult
{
	#region Properties
	
	[JsonPropertyName("data")]
	[Newtonsoft.Json.JsonProperty("data")]
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
	
	[JsonPropertyName("isSuccess")]
	[Newtonsoft.Json.JsonProperty("isSuccess")]
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
	
	[JsonPropertyName("statusCode")]
	[Newtonsoft.Json.JsonProperty("statusCode")]
	public HttpStatusCode? StatusCode { get; private set; }
	
	[JsonPropertyName("headers")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("headers", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public IDictionary<string, string>? Headers { get; set; }
	
	[JsonPropertyName("message")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("message", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string? Message { get; set; }
	
	[JsonPropertyName("data")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("data", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public T? Data { get; set; }
	
	[JsonPropertyName("rawData")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("rawData", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public byte[]? RawData { get; set; }
	
	[JsonPropertyName("json")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("json", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string? Json { get; set; }
	
	[JsonPropertyName("exception")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("exception", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
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
		return this.Message ?? this.Exception?.Message ?? this.Json ?? (this.IsSuccess ? "Success" : "Failure");
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