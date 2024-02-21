using Ertis.Core.Models.Response;
using Ertis.Net.Http;
using Newtonsoft.Json;

namespace Ertis.Net.Rest;

public class RestHandler : IRestHandler
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
		"Warning",
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
		"Last-Modified",
	};

	#endregion
	
	#region Methods

	public IResponseResult<TResult> ExecuteRequest<TResult>(
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers = null,
		IRequestBody? body = null, 
		JsonConverter[]? converters = null)
	{
		return this.ExecuteRequestAsync<TResult>(method, url, headers, body, converters).ConfigureAwait(false).GetAwaiter().GetResult();
	}

	public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers = null,
		IRequestBody? body = null,
		JsonConverter[]? converters = null, 
		CancellationToken cancellationToken = default)
	{
		// ReSharper disable once ConvertToUsingDeclaration
		using (var httpClient = new HttpClient())
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
				
			if (response.IsSuccessStatusCode)
			{
				return new ResponseResult<TResult>(response.StatusCode)
				{
					Json = json,
					RawData = rawData,
					Data = JsonConvert.DeserializeObject<TResult>(json, converters ?? Array.Empty<JsonConverter>())!,
				};
			}
			else
			{
				return new ResponseResult<TResult>(response.StatusCode, json)
				{
					Json = json,
					RawData = rawData
				};
			}
		}
	}

	public IResponseResult<TResult> ExecuteRequest<TResult>(
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null, 
		JsonConverter[]? converters = null)
	{
		if (queryString != null && queryString.Any())
		{
			var url = $"{baseUrl}?{queryString}";
			return this.ExecuteRequest<TResult>(method, url, headers, body, converters);
		}
		else
		{
			return this.ExecuteRequest<TResult>(method, baseUrl, headers, body, converters);
		}
	}

	public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null, 
		JsonConverter[]? converters = null,
		CancellationToken cancellationToken = default)
	{
		if (queryString != null && queryString.Any())
		{
			var url = $"{baseUrl}?{queryString}";
			return await this.ExecuteRequestAsync<TResult>(method, url, headers, body, converters, cancellationToken: cancellationToken);
		}
		else
		{
			return await this.ExecuteRequestAsync<TResult>(method, baseUrl, headers, body, converters, cancellationToken: cancellationToken);
		}
	}

	public IResponseResult ExecuteRequest(
		HttpMethod method, 
		string url,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		return this.ExecuteRequestAsync(method, url, headers, body).ConfigureAwait(false).GetAwaiter().GetResult();
	}

	public async Task<IResponseResult> ExecuteRequestAsync(
		HttpMethod method, 
		string url, 
		IHeaderCollection? headers = null, 
		IRequestBody? body = null,
		CancellationToken cancellationToken = default)
	{
		// ReSharper disable once ConvertToUsingDeclaration
		using (var httpClient = new HttpClient())
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
				
			if (response.IsSuccessStatusCode)
			{
				return new ResponseResult(response.StatusCode)
				{
					Json = json,
					RawData = rawData
				};
			}
			else
			{
				return new ResponseResult(response.StatusCode, json)
				{
					Json = json,
					RawData = rawData
				};
			}
		}
	}

	public IResponseResult ExecuteRequest(
		HttpMethod method, 
		string baseUrl, 
		IQueryString? queryString = null,
		IHeaderCollection? headers = null, 
		IRequestBody? body = null)
	{
		if (queryString != null && queryString.Any())
		{
			var url = $"{baseUrl}?{queryString}";
			return this.ExecuteRequest(method, url, headers, body);
		}
		else
		{
			return this.ExecuteRequest(method, baseUrl, headers, body);
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
		if (queryString != null && queryString.Any())
		{
			var url = $"{baseUrl}?{queryString}";
			return await this.ExecuteRequestAsync(method, url, headers, body, cancellationToken: cancellationToken);
		}
		else
		{
			return await this.ExecuteRequestAsync(method, baseUrl, headers, body, cancellationToken: cancellationToken);
		}
	}
	
	#endregion
}