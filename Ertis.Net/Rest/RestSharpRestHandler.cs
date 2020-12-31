using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ertis.Core.Models.Response;
using Ertis.Net.Extensions;
using Ertis.Net.Http;
using RestSharp;

namespace Ertis.Net.Rest
{
	public class RestSharpRestHandler : IRestHandler
	{
		#region Properties

		private RestClient Client { get; }

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public RestSharpRestHandler()
		{
			this.Client = new RestClient();
		}

		#endregion
		
		#region Methods

		public IResponseResult<TResult> ExecuteRequest<TResult>(
			HttpMethod method, 
			string url, 
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			var restSharpMethod = method.ConvertToRestSharpMethod();
			var dataFormat = body.ConvertToRestSharpDataFormat();
			var request = new RestRequest(url, restSharpMethod, dataFormat);
			if (headers != null)
			{
				foreach (var header in headers)
				{
					request.AddHeader(header.Key, header.Value?.ToString() ?? "");
				}
			}
			
			var response = this.Client.Execute<TResult>(request);
			if (response.IsSuccessful)
			{
				return new ResponseResult<TResult>(response.StatusCode)
				{
					Json = response.Content,
					RawData = response.RawBytes,
					Data = response.Data
				};
			}
			else
			{
				return new ResponseResult<TResult>(response.StatusCode, response.Content);
			}
		}

		public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(
			HttpMethod method, 
			string url, 
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			var restSharpMethod = method.ConvertToRestSharpMethod();
			var dataFormat = body.ConvertToRestSharpDataFormat();
			var request = new RestRequest(url, restSharpMethod, dataFormat);
			if (headers != null)
			{
				foreach (var header in headers)
				{
					request.AddHeader(header.Key, header.Value?.ToString() ?? "");
				}
			}
			
			var response = await this.Client.ExecuteAsync<TResult>(request);
			if (response.IsSuccessful)
			{
				return new ResponseResult<TResult>(response.StatusCode)
				{
					Json = response.Content,
					RawData = response.RawBytes,
					Data = response.Data
				};
			}
			else
			{
				return new ResponseResult<TResult>(response.StatusCode, response.Content);
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
			var restSharpMethod = method.ConvertToRestSharpMethod();
			var dataFormat = body.ConvertToRestSharpDataFormat();
			var request = new RestRequest(url, restSharpMethod, dataFormat);
			if (headers != null)
			{
				foreach (var header in headers)
				{
					request.AddHeader(header.Key, header.Value?.ToString() ?? "");
				}
			}
			
			var response = this.Client.Execute(request);
			if (response.IsSuccessful)
			{
				return new ResponseResult(response.StatusCode)
				{
					Json = response.Content,
					RawData = response.RawBytes
				};
			}
			else
			{
				return new ResponseResult(response.StatusCode, response.Content);
			}
		}

		public async Task<IResponseResult> ExecuteRequestAsync(
			HttpMethod method, 
			string url, 
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			var restSharpMethod = method.ConvertToRestSharpMethod();
			var dataFormat = body.ConvertToRestSharpDataFormat();
			var request = new RestRequest(url, restSharpMethod, dataFormat);
			if (headers != null)
			{
				foreach (var header in headers)
				{
					request.AddHeader(header.Key, header.Value?.ToString() ?? "");
				}
			}
			
			var response = await this.Client.ExecuteAsync(request);
			if (response.IsSuccessful)
			{
				return new ResponseResult(response.StatusCode)
				{
					Json = response.Content,
					RawData = response.RawBytes
				};
			}
			else
			{
				return new ResponseResult(response.StatusCode, response.Content);
			}
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