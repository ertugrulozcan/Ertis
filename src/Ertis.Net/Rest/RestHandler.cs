using System.Collections.Frozen;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Ertis.Core.Models;
using Ertis.Net.Http;

// ReSharper disable UnusedType.Global
namespace Ertis.Net.Rest;

public class RestHandler : IRestHandler
{
	#region Constants
	
	/// <summary>
	/// The headers of the content (the other headers are the headers of the request)
	/// </summary>
	private static readonly FrozenSet<string> ContentHeaders = new[]
	{
		"Allow",
		"Content-Disposition",
		"Content-Encoding",
		"Content-Language",
		"Content-Length",
		"Content-Location",
		"Content-MD5",
		"Content-Range",
		"Content-Type",
		"Expires",
		"Last-Modified"
	}.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
	
	#endregion
	
	#region Services
	
	private readonly IHttpClientFactory _httpClientFactory;
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="httpClientFactory"></param>
	public RestHandler(IHttpClientFactory httpClientFactory)
	{
		this._httpClientFactory = httpClientFactory;
	}
	
	#endregion
	
	#region Methods
	
	public HttpClient GetHttpClient()
	{
		return this._httpClientFactory.CreateClient();
	}
	
	public IResponseResult<TResult> ExecuteRequest<TResult>(
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers = null,
		IRequestBody? body = null)
	{
		using var httpClient = this.GetHttpClient();
		return this.ExecuteRequest<TResult>(httpClient, method, url, headers, body);
	}
	
	public IResponseResult<TResult> ExecuteRequest<TResult>(
		HttpClient httpClient, 
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers = null,
		IRequestBody? body = null)
	{
		return this.ExecuteRequestAsync<TResult>(httpClient, method, url, headers, body).ConfigureAwait(false).GetAwaiter().GetResult();
	}
	
	public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
		HttpMethod method,
		string url,
		IHeaderCollection? headers = null,
		IRequestBody? body = null,
		CancellationToken cancellationToken = default)
	{
		using var httpClient = this.GetHttpClient();
		return await this.ExecuteRequestAsync<TResult>(httpClient, method, url, headers, body, cancellationToken);
	}
	
	public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
		HttpClient httpClient, 
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers = null,
		IRequestBody? body = null,
		CancellationToken cancellationToken = default)
	{
		var response = await SendAsync(httpClient, method, url, headers, body, cancellationToken);
		if (response.IsSuccess)
		{
			return new ResponseResult<TResult>(response.StatusCode)
			{
				Json = response.Json,
				RawData = response.RawData,
				Data = string.IsNullOrWhiteSpace(response.Json) ? default : JsonSerializer.Deserialize<TResult>(response.Json),
				Headers = response.Headers
			};
		}
		else
		{
			return new ResponseResult<TResult>(response.StatusCode, response.Json)
			{
				Json = response.Json,
				RawData = response.RawData,
				Headers = response.Headers
			};
		}
	}
	
	public IResponseResult<TResult> ExecuteRequest<TResult>(
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		using var httpClient = this.GetHttpClient();
		return this.ExecuteRequest<TResult>(httpClient, method, baseUrl, queryString, headers, body);
	}
	
	public IResponseResult<TResult> ExecuteRequest<TResult>(
		HttpClient httpClient, 
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		if (queryString != null && queryString.Any())
		{
			var url = AppendQueryString(baseUrl, queryString);
			return this.ExecuteRequest<TResult>(httpClient, method, url, headers, body);
		}
		else
		{
			return this.ExecuteRequest<TResult>(httpClient, method, baseUrl, headers, body);
		}
	}
	
	public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null, 
		CancellationToken cancellationToken = default)
	{
		using var httpClient = this.GetHttpClient();
		return await this.ExecuteRequestAsync<TResult>(httpClient, method, baseUrl, queryString, headers, body, cancellationToken);
	}
	
	public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
		HttpClient httpClient, 
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null, 
		CancellationToken cancellationToken = default)
	{
		if (queryString != null && queryString.Any())
		{
			var url = AppendQueryString(baseUrl, queryString);
			return await this.ExecuteRequestAsync<TResult>(httpClient, method, url, headers, body, cancellationToken: cancellationToken);
		}
		else
		{
			return await this.ExecuteRequestAsync<TResult>(httpClient, method, baseUrl, headers, body, cancellationToken: cancellationToken);
		}
	}
	
	public IResponseResult ExecuteRequest(
		HttpMethod method, 
		string url,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		using var httpClient = this.GetHttpClient();
		return this.ExecuteRequest(httpClient, method, url, headers, body);
	}
	
	public IResponseResult ExecuteRequest(
		HttpClient httpClient, 
		HttpMethod method, 
		string url,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		return this.ExecuteRequestAsync(httpClient, method, url, headers, body).ConfigureAwait(false).GetAwaiter().GetResult();
	}
	
	public async Task<IResponseResult> ExecuteRequestAsync(
		HttpMethod method,
		string url,
		IHeaderCollection? headers = null,
		IRequestBody? body = null,
		CancellationToken cancellationToken = default)
	{
		using var httpClient = this.GetHttpClient();
		return await this.ExecuteRequestAsync(httpClient, method, url, headers, body, cancellationToken);
	}
	
	public async Task<IResponseResult> ExecuteRequestAsync(
		HttpClient httpClient, 
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers = null, 
		IRequestBody? body = null,
		CancellationToken cancellationToken = default)
	{
		var response = await SendAsync(httpClient, method, url, headers, body, cancellationToken);
		if (response.IsSuccess)
		{
			return new ResponseResult(response.StatusCode)
			{
				Json = response.Json,
				RawData = response.RawData,
				Headers = response.Headers
			};
		}
		else
		{
			return new ResponseResult(response.StatusCode, response.Json)
			{
				Json = response.Json,
				RawData = response.RawData,
				Headers = response.Headers
			};
		}
	}
	
	public IResponseResult ExecuteRequest(
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		using var httpClient = this.GetHttpClient();
		return this.ExecuteRequest(httpClient, method, baseUrl, queryString, headers, body);
	}
	
	public IResponseResult ExecuteRequest(
		HttpClient httpClient, 
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		if (queryString != null && queryString.Any())
		{
			var url = AppendQueryString(baseUrl, queryString);
			return this.ExecuteRequest(httpClient, method, url, headers, body);
		}
		else
		{
			return this.ExecuteRequest(httpClient, method, baseUrl, headers, body);
		}
	}
	
	public async Task<IResponseResult> ExecuteRequestAsync(
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null, 
		IHeaderCollection? headers = null, 
		IRequestBody? body = null,
		CancellationToken cancellationToken = default)
	{
		using var httpClient = this.GetHttpClient();
		return await this.ExecuteRequestAsync(httpClient, method, baseUrl, queryString, headers, body, cancellationToken);
	}
	
	public async Task<IResponseResult> ExecuteRequestAsync(
		HttpClient httpClient, 
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null, 
		IHeaderCollection? headers = null, 
		IRequestBody? body = null,
		CancellationToken cancellationToken = default)
	{
		if (queryString != null && queryString.Any())
		{
			var url = AppendQueryString(baseUrl, queryString);
			return await this.ExecuteRequestAsync(httpClient, method, url, headers, body, cancellationToken: cancellationToken);
		}
		else
		{
			return await this.ExecuteRequestAsync(httpClient, method, baseUrl, headers, body, cancellationToken: cancellationToken);
		}
	}
	
	private static async Task<Response> SendAsync(
		HttpClient httpClient, 
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers, 
		IRequestBody? body,
		CancellationToken cancellationToken)
	{
		using var request = CreateRequest(method, url, headers, body);
		using var response = await httpClient.SendAsync(request, cancellationToken: cancellationToken);
		var rawData = await response.Content.ReadAsByteArrayAsync(cancellationToken: cancellationToken);
		var json = ReadString(rawData, response.Content.Headers.ContentType?.CharSet);
		var responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (var (key, values) in response.Headers)
		{
			var value = values.FirstOrDefault(x => !string.IsNullOrEmpty(x));
			if (value != null)
			{
				responseHeaders[key] = value;
			}
		}
		
		return new Response(response.IsSuccessStatusCode, response.StatusCode, rawData, json, responseHeaders);
	}
	
	/// <summary>
	/// The headers are added to the request (not to the client, which may be shared between the requests)
	/// </summary>
	private static HttpRequestMessage CreateRequest(HttpMethod method, string url, IHeaderCollection? headers, IRequestBody? body)
	{
		var request = new HttpRequestMessage(method, url);
		var httpContent = body?.GetHttpContent();
		if (httpContent != null)
		{
			request.Content = httpContent;
		}
		
		if (headers != null)
		{
			foreach (var (key, value) in headers.ToDictionary())
			{
				var text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
				if (ContentHeaders.Contains(key))
				{
					// The content headers are dropped for a request without content
					request.Content?.Headers.Remove(key);
					request.Content?.Headers.Add(key, text);
				}
				else
				{
					request.Headers.Add(key, text);
				}
			}
		}
		
		return request;
	}
	
	private static string ReadString(byte[] rawData, string? charSet)
	{
		var encoding = Encoding.UTF8;
		if (!string.IsNullOrEmpty(charSet))
		{
			try
			{
				encoding = Encoding.GetEncoding(charSet.Trim('"'));
			}
			catch (ArgumentException)
			{
				// An unknown charset is read as UTF-8
			}
		}
		
		using var reader = new StreamReader(new MemoryStream(rawData), encoding, detectEncodingFromByteOrderMarks: true);
		return reader.ReadToEnd();
	}
	
	private static string AppendQueryString(string baseUrl, IQueryString queryString)
	{
		return $"{baseUrl}{(baseUrl.Contains('?') ? '&' : '?')}{queryString}";
	}
	
	#endregion
	
	#region Helper Types
	
	private sealed record Response(bool IsSuccess, HttpStatusCode StatusCode, byte[] RawData, string Json, Dictionary<string, string> Headers);
	
	#endregion
}