using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ertis.Core.Models.Response;
using Ertis.Net.Http;

namespace Ertis.Net.Rest
{
	public class SystemRestHandler : IRestHandler
	{
		#region Methods

		public IResponseResult<TResult> ExecuteRequest<TResult>(
			HttpMethod method, 
			string url, 
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return this.ExecuteRequestAsync<TResult>(method, url, headers, body).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
			HttpMethod method, 
			string url, 
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			using (var httpClient = new HttpClient())
			{
				var request = new HttpRequestMessage(method, url);
				if (headers != null)
				{
					foreach (var header in headers)
					{
						request.Headers.Add(header.Key, header.Value.ToString());	
					}
				}

				var httpContent = body?.GetHttpContent();
				if (httpContent != null)
				{
					request.Content = httpContent;
				}

				var response = await httpClient.SendAsync(request);
				if (response != null)
				{
					if (response.IsSuccessStatusCode)
					{
						var rawData = await response.Content.ReadAsByteArrayAsync();
						var json = await response.Content.ReadAsStringAsync();
						return new ResponseResult<TResult>(response.StatusCode)
						{
							Json = json,
							RawData = rawData,
							Data = Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(json),
						};
					}
					else
					{
						return new ResponseResult<TResult>(response.StatusCode, await response.Content.ReadAsStringAsync());
					}
				}
				else
				{
					return new ResponseResult<TResult>(false, "Response is null!");
				}
			}
		}

		public IResponseResult<TResult> ExecuteRequest<TResult>(
			HttpMethod method, 
			string baseUrl, 
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			if (queryString != null && queryString.Any())
			{
				var url = $"{baseUrl}?{queryString}";
				return this.ExecuteRequest<TResult>(method, url, headers, body);
			}
			else
			{
				return this.ExecuteRequest<TResult>(method, baseUrl, headers, body);
			}
		}

		public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
			HttpMethod method, 
			string baseUrl, 
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			if (queryString != null && queryString.Any())
			{
				var url = $"{baseUrl}?{queryString}";
				return await this.ExecuteRequestAsync<TResult>(method, url, headers, body);
			}
			else
			{
				return await this.ExecuteRequestAsync<TResult>(method, baseUrl, headers, body);
			}
		}

		public IResponseResult ExecuteRequest(
			HttpMethod method, 
			string url,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.ExecuteRequestAsync(method, url, headers, body).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public async Task<IResponseResult> ExecuteRequestAsync(
			HttpMethod method, 
			string url, 
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return await this.ExecuteRequestAsync<object>(method, url, headers, body);
		}

		public IResponseResult ExecuteRequest(
			HttpMethod method, 
			string baseUrl, 
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
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
			IQueryString queryString = null, 
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			if (queryString != null && queryString.Any())
			{
				var url = $"{baseUrl}?{queryString}";
				return await this.ExecuteRequestAsync(method, url, headers, body);
			}
			else
			{
				return await this.ExecuteRequestAsync(method, baseUrl, headers, body);
			}
		}
		
		#endregion
	}
}