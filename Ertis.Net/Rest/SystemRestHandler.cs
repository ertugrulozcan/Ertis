using Ertis.Core.Models.Response;
using Ertis.Net.Http;

namespace Ertis.Net.Rest;

// ReSharper disable once UnusedType.Global
public class SystemRestHandler : ISystemRestHandler
{
    #region Constants
    
	private static readonly string[] DefaultHeaders = 
	{
		"Accept",
		"Accept-Charset",
		"Accept-Encoding",
		"Accept-Language",
		"Authorization",
		"Cache-Control",
		"Connection",
		"Date",
		"Expect",
		"From",
		"Host",
		"If-Match",
		"If-Modified-Since",
		"If-None-Match",
		"If-Range",
		"If-Unmodified-Since",
		"Max-Forwards",
		"Pragma",
		"Proxy-Authorization",
		"Referrer",
		"Range",
		"Transfer-Encoding",
		"Trailer",
		"TE",
		"Upgrade",
		"Via",
		"Warning"
	};
	
	private static readonly string[] ContentHeaders = 
	{
		"Allow",
		"Content-Disposition",
		"Content-Encoding",
		"Content-Language",
		"Content-Length",
		"Content-Location",
		"Content-Range",
		"Content-Type",
		"Expires",
		"Last-Modified"
	};
	
	#endregion
	
	#region Services
	
	private readonly IHttpClientFactory _httpClientFactory;
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="httpClientFactory"></param>
	public SystemRestHandler(IHttpClientFactory httpClientFactory)
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
		var request = new HttpRequestMessage(method, url);
		if (headers != null)
		{
			foreach (var (key, value) in headers.ToDictionary())
			{
				if (DefaultHeaders.Contains(key))
				{
					httpClient.DefaultRequestHeaders.Add(key, value.ToString());
				}
				else if (ContentHeaders.Contains(key))
				{
					request.Content?.Headers.Add(key, value.ToString());
				}
				else
				{
					request.Headers.Add(key, value.ToString());
				}
			}
		}
		
		var httpContent = body?.GetHttpContent();
		if (httpContent != null)
		{
			request.Content = httpContent;
		}
		
		var response = await httpClient.SendAsync(request, cancellationToken: cancellationToken);
		var rawData = await response.Content.ReadAsByteArrayAsync(cancellationToken: cancellationToken);
		var json = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
		var responseHeaders = response.Headers
			.Where(x => x.Value.Any(y => !string.IsNullOrEmpty(y)))
			.ToDictionary(x => x.Key, y => y.Value.FirstOrDefault());
		
		if (response.IsSuccessStatusCode)
		{
			return new ResponseResult<TResult>(response.StatusCode)
			{
				Json = json,
				RawData = rawData,
				Data = System.Text.Json.JsonSerializer.Deserialize<TResult>(json)!,
				Headers = responseHeaders
			};
		}
		else
		{
			return new ResponseResult<TResult>(response.StatusCode, json)
			{
				Json = json,
				RawData = rawData,
				Headers = responseHeaders
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
			var url = $"{baseUrl}?{queryString}";
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
			var url = $"{baseUrl}?{queryString}";
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
				if (DefaultHeaders.Contains(key))
				{
					httpClient.DefaultRequestHeaders.Add(key, value.ToString());
				}
				else if (ContentHeaders.Contains(key))
				{
					request.Content?.Headers.Add(key, value.ToString());
				}
				else
				{
					request.Headers.Add(key, value.ToString());	
				}
			}
		}
		
		var response = await httpClient.SendAsync(request, cancellationToken: cancellationToken);
		var rawData = await response.Content.ReadAsByteArrayAsync(cancellationToken: cancellationToken);
		var json = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
		var responseHeaders = response.Headers
			.Where(x => x.Value.Any(y => !string.IsNullOrEmpty(y)))
			.ToDictionary(x => x.Key, y => y.Value.FirstOrDefault());
		
		if (response.IsSuccessStatusCode)
		{
			return new ResponseResult(response.StatusCode)
			{
				Json = json,
				RawData = rawData,
				Headers = responseHeaders
			};
		}
		else
		{
			return new ResponseResult(response.StatusCode, json)
			{
				Json = json,
				RawData = rawData,
				Headers = responseHeaders
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
			var url = $"{baseUrl}?{queryString}";
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
			var url = $"{baseUrl}?{queryString}";
			return await this.ExecuteRequestAsync(httpClient, method, url, headers, body, cancellationToken: cancellationToken);
		}
		else
		{
			return await this.ExecuteRequestAsync(httpClient, method, baseUrl, headers, body, cancellationToken: cancellationToken);
		}
	}
	
	#endregion
}