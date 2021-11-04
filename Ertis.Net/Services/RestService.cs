using System.Net.Http;
using System.Threading.Tasks;
using Ertis.Core.Models.Response;
using Ertis.Net.Http;
using Ertis.Net.Rest;

namespace Ertis.Net.Services
{
	public abstract class RestService
	{
		#region Services

		private readonly IRestHandler restHandler;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="restHandler"></param>
		protected RestService(IRestHandler restHandler)
		{
			this.restHandler = restHandler;
		}

		#endregion

		#region Methods

		protected IResponseResult Get(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest(HttpMethod.Get, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult> GetAsync(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync(HttpMethod.Get, url, queryString, headers, body);
		}
		
		protected IResponseResult<TResult> Get<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest<TResult>(HttpMethod.Get, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult<TResult>> GetAsync<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync<TResult>(HttpMethod.Get, url, queryString, headers, body);
		}
		
		protected IResponseResult Post(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest(HttpMethod.Post, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult> PostAsync(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync(HttpMethod.Post, url, queryString, headers, body);
		}
		
		protected IResponseResult<TResult> Post<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest<TResult>(HttpMethod.Post, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult<TResult>> PostAsync<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync<TResult>(HttpMethod.Post, url, queryString, headers, body);
		}
		
		protected IResponseResult Put(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest(HttpMethod.Put, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult> PutAsync(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync(HttpMethod.Put, url, queryString, headers, body);
		}
		
		protected IResponseResult<TResult> Put<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest<TResult>(HttpMethod.Put, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult<TResult>> PutAsync<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync<TResult>(HttpMethod.Put, url, queryString, headers, body);
		}
		
		protected IResponseResult Delete(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest(HttpMethod.Delete, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult> DeleteAsync(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync(HttpMethod.Delete, url, queryString, headers, body);
		}
		
		protected IResponseResult<TResult> Delete<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null, 
			IRequestBody body = null)
		{
			return this.restHandler.ExecuteRequest<TResult>(HttpMethod.Delete, url, queryString, headers, body);
		}

		protected async ValueTask<IResponseResult<TResult>> DeleteAsync<TResult>(
			string url,
			IQueryString queryString = null,
			IHeaderCollection headers = null,
			IRequestBody body = null)
		{
			return await this.restHandler.ExecuteRequestAsync<TResult>(HttpMethod.Delete, url, queryString, headers, body);
		}

		#endregion
	}
}