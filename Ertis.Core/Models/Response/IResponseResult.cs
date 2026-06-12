using System.Net;
using System.Text.Json.Serialization;
using JsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;

namespace Ertis.Core.Models.Response;

public interface IResponseResult
{
	#region Properties
	
	[JsonProperty("isSuccess")]
	[JsonPropertyName("isSuccess")]
	bool IsSuccess { get; }
	
	[JsonProperty("statusCode", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("statusCode")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	HttpStatusCode? StatusCode { get; }
	
	[JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("headers")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	IDictionary<string, string>? Headers { get; }
	
	[JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("message")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	string? Message { get; set; }
	
	[JsonProperty("rawData", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("rawData")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	byte[]? RawData { get; set; }
	
	[JsonProperty("json", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("json")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	string? Json { get; set; }
	
	[JsonProperty("exception", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("exception")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	Exception? Exception { get; set; }
	
	#endregion
}

public interface IResponseResult<out T> : IResponseResult
{
	#region Properties
	
	[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("data")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
	
	[JsonProperty("isSuccess")]
	[JsonPropertyName("isSuccess")]
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
	
	[JsonProperty("statusCode", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("statusCode")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public HttpStatusCode? StatusCode { get; private set; }
	
	[JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("headers")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public IDictionary<string, string>? Headers { get; set; }
	
	[JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("message")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Message { get; set; }
	
	[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("data")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public T? Data { get; set; }
	
	[JsonProperty("rawData", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("rawData")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public byte[]? RawData { get; set; }
	
	[JsonProperty("json", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("json")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Json { get; set; }
	
	[JsonProperty("exception", NullValueHandling = NullValueHandling.Ignore)]
	[JsonPropertyName("exception")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
	
	public override string? ToString()
	{
		return this.Message;
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
	{}
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="isSuccess"></param>
	/// <param name="message"></param>
	public ResponseResult(bool isSuccess, string message) : base(isSuccess, message)
	{}
	
	/// <summary>
	/// Constructor 3
	/// </summary>
	/// <param name="httpCode"></param>
	public ResponseResult(HttpStatusCode httpCode) : base(httpCode)
	{}
	
	/// <summary>
	/// Constructor 4
	/// </summary>
	/// <param name="httpCode"></param>
	/// <param name="message"></param>
	public ResponseResult(HttpStatusCode httpCode, string message) : base(httpCode, message)
	{}
	
	#endregion
}