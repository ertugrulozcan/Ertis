using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ertis.Core.Models.Response;
using Ertis.Net.Http;

namespace Ertis.Net.Rest
{
	public interface IRestHandler
	{
		#region Methods

		IResponseResult<TResult> ExecuteRequest<TResult>(HttpMethod method, string url, IHeaderCollection headers = null, IRequestBody body = null);

		Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpMethod method, string url, IHeaderCollection headers = null, IRequestBody body = null, CancellationToken cancellationToken = default);
		
		IResponseResult<TResult> ExecuteRequest<TResult>(HttpMethod method, string url, IQueryString queryString = null, IHeaderCollection headers = null, IRequestBody body = null);

		Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpMethod method, string url, IQueryString queryString = null, IHeaderCollection headers = null, IRequestBody body = null, CancellationToken cancellationToken = default);
		
		IResponseResult ExecuteRequest(HttpMethod method, string url, IHeaderCollection headers = null, IRequestBody body = null);
		
		Task<IResponseResult> ExecuteRequestAsync(HttpMethod method, string url, IHeaderCollection headers = null, IRequestBody body = null, CancellationToken cancellationToken = default);
		
		IResponseResult ExecuteRequest(HttpMethod method, string url, IQueryString queryString = null, IHeaderCollection headers = null, IRequestBody body = null);
		
		Task<IResponseResult> ExecuteRequestAsync(HttpMethod method, string url, IQueryString queryString = null, IHeaderCollection headers = null, IRequestBody body = null, CancellationToken cancellationToken = default);
		
		#endregion
	}
}