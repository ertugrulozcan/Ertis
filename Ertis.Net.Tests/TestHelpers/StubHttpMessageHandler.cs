using System.Net;
using System.Net.Http.Headers;
using NSubstitute;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Ertis.Net.Tests.TestHelpers;

/// <summary>
/// Records the sent requests (with their content read before the request is disposed) and answers them with the given response
/// </summary>
public sealed class StubHttpMessageHandler : HttpMessageHandler
{
	#region Properties
	
	public List<RecordedRequest> Requests { get; } = [];
	
	public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
	
	public string? ResponseBody { get; set; } = "{}";
	
	public string ResponseContentType { get; set; } = "application/json";
	
	public System.Text.Encoding? ResponseEncoding { get; set; }
	
	public Dictionary<string, string> ResponseHeaders { get; } = new() { ["X-Custom-Header"] = "42" };
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// A client factory creating clients of this handler (the handler is not disposed with the clients)
	/// </summary>
	public IHttpClientFactory CreateFactory()
	{
		var factory = Substitute.For<IHttpClientFactory>();
		factory.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(this, disposeHandler: false));
		return factory;
	}
	
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		this.Requests.Add(new RecordedRequest(
			request.Method,
			request.RequestUri!,
			request.Headers.ToDictionary(x => x.Key, x => string.Join(", ", x.Value)),
			request.Content?.Headers.ToDictionary(x => x.Key, x => string.Join(", ", x.Value)) ?? [],
			request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken)));
		
		var response = new HttpResponseMessage(this.StatusCode);
		if (this.ResponseBody != null)
		{
			if (this.ResponseEncoding != null)
			{
				response.Content = new StringContent(this.ResponseBody, this.ResponseEncoding, this.ResponseContentType);
			}
			else
			{
				response.Content = new StringContent(this.ResponseBody);
				response.Content.Headers.ContentType = new MediaTypeHeaderValue(this.ResponseContentType);
			}
		}
		
		foreach (var (key, value) in this.ResponseHeaders)
		{
			response.Headers.TryAddWithoutValidation(key, value);
		}
		
		return response;
	}
	
	#endregion
}

public sealed record RecordedRequest(
	HttpMethod Method,
	Uri Uri,
	Dictionary<string, string> Headers,
	Dictionary<string, string> ContentHeaders,
	string? Content);
