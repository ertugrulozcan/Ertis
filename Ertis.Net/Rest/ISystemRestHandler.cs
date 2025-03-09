using Ertis.Core.Models.Response;
using Ertis.Net.Http;

namespace Ertis.Net.Rest;

public interface ISystemRestHandler
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