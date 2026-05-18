using Ertis.Core.Models;
using Ertis.Net.Http;

// ReSharper disable UnusedMember.Global
namespace Ertis.Net.Rest;

// ReSharper disable once UnusedType.Global
public interface IRestHandler
{
	#region Methods
    
    HttpClient GetHttpClient();
    
    IResponseResult<TResult> ExecuteRequest<TResult>(HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    IResponseResult<TResult> ExecuteRequest<TResult>(HttpClient client, HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpClient client, HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    IResponseResult<TResult> ExecuteRequest<TResult>(HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    IResponseResult<TResult> ExecuteRequest<TResult>(HttpClient client, HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpClient client, HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    IResponseResult ExecuteRequest(HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    IResponseResult ExecuteRequest(HttpClient client, HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    Task<IResponseResult> ExecuteRequestAsync(HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    Task<IResponseResult> ExecuteRequestAsync(HttpClient client, HttpMethod method, string url, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    IResponseResult ExecuteRequest(HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    IResponseResult ExecuteRequest(HttpClient client, HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null);
    
    Task<IResponseResult> ExecuteRequestAsync(HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    Task<IResponseResult> ExecuteRequestAsync(HttpClient client, HttpMethod method, string url, IQueryString? queryString = null, IHeaderCollection? headers = null, IRequestBody? body = null, CancellationToken cancellationToken = default);
    
    #endregion
}