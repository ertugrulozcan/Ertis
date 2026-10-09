using System.Net;
using System.Text.Json.Serialization;
using Ertis.Net.Http;
using Ertis.Net.Rest;
using Ertis.Net.Tests.TestHelpers;

namespace Ertis.Net.Tests.Rest;

public class RestHandlerTests
{
	#region Fields
	
	private readonly StubHttpMessageHandler _handler = new() { ResponseBody = """{"name":"Jane","age":30}""" };
	
	#endregion
	
	#region Properties
	
	private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;
	
	#endregion
	
	#region Methods
	
	private RestHandler CreateRestHandler()
	{
		return new RestHandler(this._handler.CreateFactory());
	}
	
	#endregion
	
	#region Response Methods
	
	[Fact]
	public async Task ExecuteRequest_WithASuccessResponse_DeserializesTheData()
	{
		var result = await this.CreateRestHandler().ExecuteRequestAsync<Person>(HttpMethod.Get, "https://api.test/people/1", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		
		Assert.True(result.IsSuccess);
		Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		Assert.Equal("Jane", result.Data?.Name);
		Assert.Equal(30, result.Data?.Age);
		Assert.Equal("""{"name":"Jane","age":30}""", result.Json);
		Assert.Equal(result.Json, System.Text.Encoding.UTF8.GetString(result.RawData!));
		Assert.Equal("42", result.Headers?["X-Custom-Header"]);
		Assert.Null(result.Message);
	}
	
	[Fact]
	public async Task ExecuteRequest_WithAnErrorResponse_ReturnsTheBodyAsTheMessage()
	{
		this._handler.StatusCode = HttpStatusCode.NotFound;
		this._handler.ResponseBody = """{"message":"Not found"}""";
		
		var result = await this.CreateRestHandler().ExecuteRequestAsync<Person>(HttpMethod.Get, "https://api.test/people/1", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		var untypedResult = await this.CreateRestHandler().ExecuteRequestAsync(HttpMethod.Get, "https://api.test/people/1", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		
		Assert.False(result.IsSuccess);
		Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
		Assert.Null(result.Data);
		Assert.Equal("""{"message":"Not found"}""", result.Message);
		Assert.Equal(result.Message, untypedResult.Message);
		Assert.False(untypedResult.IsSuccess);
	}
	
	[Fact]
	public async Task ExecuteRequest_Untyped_ReturnsTheJson()
	{
		var result = await this.CreateRestHandler().ExecuteRequestAsync(HttpMethod.Delete, "https://api.test/people/1", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		
		Assert.True(result.IsSuccess);
		Assert.Equal("""{"name":"Jane","age":30}""", result.Json);
		Assert.Equal("42", result.Headers?["X-Custom-Header"]);
		Assert.Equal(HttpMethod.Delete, Assert.Single(this._handler.Requests).Method);
	}
	
	[Fact]
	public async Task ExecuteRequest_ReadsTheResponseHeadersCaseInsensitively()
	{
		var result = await this.CreateRestHandler().ExecuteRequestAsync(HttpMethod.Get, "https://api.test/a", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		
		Assert.Equal("42", result.Headers?["x-custom-header"]);
	}
	
	[Fact]
	public async Task ExecuteRequest_WithAnEmptySuccessResponse_ReturnsTheDefaultData()
	{
		this._handler.StatusCode = HttpStatusCode.NoContent;
		this._handler.ResponseBody = null;
		
		var result = await this.CreateRestHandler().ExecuteRequestAsync<Person>(HttpMethod.Delete, "https://api.test/people/1", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		
		Assert.True(result.IsSuccess);
		Assert.Null(result.Data);
	}
	
	[Fact]
	public void ExecuteRequest_Synchronous_SendsTheRequest()
	{
		var restHandler = this.CreateRestHandler();
		using var httpClient = restHandler.GetHttpClient();
		
		var results = new[]
		{
			restHandler.ExecuteRequest<Person>(HttpMethod.Get, "https://api.test/a", (IHeaderCollection?) null).Data?.Name,
			restHandler.ExecuteRequest<Person>(HttpMethod.Get, "https://api.test/a", QueryString.Add("q", "1")).Data?.Name,
			restHandler.ExecuteRequest<Person>(httpClient, HttpMethod.Get, "https://api.test/a", QueryString.Empty).Data?.Name,
			restHandler.ExecuteRequest(HttpMethod.Get, "https://api.test/a", (IHeaderCollection?) null).IsSuccess.ToString(),
			restHandler.ExecuteRequest(HttpMethod.Get, "https://api.test/a", QueryString.Add("q", "2")).IsSuccess.ToString(),
			restHandler.ExecuteRequest(httpClient, HttpMethod.Get, "https://api.test/a", QueryString.Empty).IsSuccess.ToString()
		};
		
		Assert.Equal(new[] { "Jane", "Jane", "Jane", "True", "True", "True" }, results);
		Assert.Equal(["/a", "/a?q=1", "/a", "/a", "/a?q=2", "/a"], this._handler.Requests.Select(x => x.Uri.PathAndQuery));
	}
	
	#endregion
	
	#region Request Methods
	
	[Fact]
	public async Task ExecuteRequest_DeserializesTheData()
	{
		this._handler.ResponseBody = """{"name":"Jane"}""";
		var restHandler = new RestHandler(this._handler.CreateFactory());
		var cancellationToken = TestContext.Current.CancellationToken;
		
		var typed = await restHandler.ExecuteRequestAsync<Dictionary<string, string>>(HttpMethod.Post, "https://api.test/a", QueryString.Add("q", 1), HeaderCollection.Add("X-A", "1"), new TextRequestBody("x"), cancellationToken: cancellationToken);
		var untyped = await restHandler.ExecuteRequestAsync(HttpMethod.Get, "https://api.test/a", QueryString.Empty, HeaderCollection.Add("Authorization", "Bearer a").Add("Content-Language", "tr"), new TextRequestBody("x"), cancellationToken);
		var syncTyped = restHandler.ExecuteRequest<Dictionary<string, string>>(HttpMethod.Get, "https://api.test/a", QueryString.Empty);
		
		// ReSharper disable once MethodHasAsyncOverloadWithCancellation
		var syncUntyped = restHandler.ExecuteRequest(HttpMethod.Get, "https://api.test/a", QueryString.Add("q", 2));
		
		Assert.Equal("Jane", typed.Data?["name"]);
		Assert.Equal("""{"name":"Jane"}""", untyped.Json);
		Assert.Equal("Jane", syncTyped.Data?["name"]);
		Assert.True(syncUntyped.IsSuccess);
		Assert.Equal(["/a?q=1", "/a", "/a", "/a?q=2"], this._handler.Requests.Select(x => x.Uri.PathAndQuery));
	}
	
	[Fact]
	public async Task ExecuteRequest_WithAQueryString_AppendsIt()
	{
		var restHandler = this.CreateRestHandler();
		
		await restHandler.ExecuteRequestAsync<Person>(HttpMethod.Get, "https://api.test/people", QueryString.Add("name", "Jane Doe").Add("page", 2), cancellationToken: CancellationToken);
		await restHandler.ExecuteRequestAsync(HttpMethod.Get, "https://api.test/people", QueryString.Add("a", "&"), cancellationToken: CancellationToken);
		await restHandler.ExecuteRequestAsync(HttpMethod.Get, "https://api.test/people", QueryString.Empty, cancellationToken: CancellationToken);
		
		Assert.Equal(["/people?name=Jane%20Doe&page=2", "/people?a=%26", "/people"], this._handler.Requests.Select(x => x.Uri.PathAndQuery));
	}
	
	[Fact]
	public async Task ExecuteRequest_WithAUrlWithAQuery_AppendsTheQueryString()
	{
		await this.CreateRestHandler().ExecuteRequestAsync(HttpMethod.Get, "https://api.test/people?a=1", QueryString.Add("b", 2), cancellationToken: CancellationToken);
		
		Assert.Equal("/people?a=1&b=2", Assert.Single(this._handler.Requests).Uri.PathAndQuery);
	}
	
	[Fact]
	public async Task ExecuteRequest_SendsTheHeadersAndTheBody()
	{
		var headers = HeaderCollection.Add("Authorization", "Bearer token").Add("X-Custom", "value");
		
		await this.CreateRestHandler().ExecuteRequestAsync<Person>(HttpMethod.Post, "https://api.test/people", headers, new JsonRequestBody(new Person { Name = "Jane" }), CancellationToken);
		
		var request = Assert.Single(this._handler.Requests);
		Assert.Equal(HttpMethod.Post, request.Method);
		Assert.Equal("Bearer token", request.Headers["Authorization"]);
		Assert.Equal("value", request.Headers["X-Custom"]);
		Assert.Equal("""{"name":"Jane","age":0}""", request.Content);
		Assert.Equal("application/json", request.ContentHeaders["Content-Type"]);
	}
	
	[Fact]
	public async Task ExecuteRequest_Untyped_SendsTheContentHeaders()
	{
		var headers = HeaderCollection.Add("Content-Language", "tr");
		
		await this.CreateRestHandler().ExecuteRequestAsync(HttpMethod.Post, "https://api.test/people", headers, new TextRequestBody("x"), CancellationToken);
		
		Assert.Equal("tr", Assert.Single(this._handler.Requests).ContentHeaders["Content-Language"]);
	}
	
	[Fact]
	public async Task ExecuteRequest_Typed_SendsTheContentHeaders()
	{
		var headers = HeaderCollection.Add("Content-Language", "tr");
		
		await this.CreateRestHandler().ExecuteRequestAsync<Person>(HttpMethod.Post, "https://api.test/people", headers, new TextRequestBody("x"), CancellationToken);
		
		Assert.Equal("tr", Assert.Single(this._handler.Requests).ContentHeaders["Content-Language"]);
	}
	
	[Fact]
	public async Task ExecuteRequest_WithAContentTypeHeader_ReplacesTheContentType()
	{
		var headers = HeaderCollection.Add("Content-Type", "application/merge-patch+json");
		
		await this.CreateRestHandler().ExecuteRequestAsync<Person>(HttpMethod.Patch, "https://api.test/people/1", headers, new JsonRequestBody(new { name = "x" }), CancellationToken);
		
		Assert.Equal("application/merge-patch+json", Assert.Single(this._handler.Requests).ContentHeaders["Content-Type"]);
	}
	
	[Fact]
	public async Task ExecuteRequest_WithACharSet_ReadsTheBodyWithIt()
	{
		this._handler.ResponseBody = """{"name":"Şule"}""";
		this._handler.ResponseEncoding = System.Text.Encoding.Unicode;
		var restHandler = this.CreateRestHandler();
		
		var result = await restHandler.ExecuteRequestAsync<Person>(HttpMethod.Get, "https://api.test/a", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		
		Assert.Equal("Şule", result.Data?.Name);
		Assert.Equal("""{"name":"Şule"}""", result.Json);
	}
	
	[Fact]
	public async Task ExecuteRequest_WithLowercaseHeaderNames_SendsThem()
	{
		var headers = HeaderCollection.Add("content-language", "tr").Add("authorization", "Bearer a");
		
		await this.CreateRestHandler().ExecuteRequestAsync(HttpMethod.Post, "https://api.test/people", headers, new TextRequestBody("x"), CancellationToken);
		
		var request = Assert.Single(this._handler.Requests);
		Assert.Equal("tr", request.ContentHeaders["Content-Language"]);
		Assert.Equal("Bearer a", request.Headers["Authorization"]);
	}
	
	[Fact]
	public async Task ExecuteRequest_WithASharedClient_SendsOnlyTheHeadersOfTheRequest()
	{
		var restHandler = this.CreateRestHandler();
		using var httpClient = restHandler.GetHttpClient();
		
		await restHandler.ExecuteRequestAsync<Person>(httpClient, HttpMethod.Get, "https://api.test/a", HeaderCollection.Add("Authorization", "Bearer first"), cancellationToken: CancellationToken);
		await restHandler.ExecuteRequestAsync(httpClient, HttpMethod.Get, "https://api.test/a", HeaderCollection.Add("Authorization", "Bearer second"), cancellationToken: CancellationToken);
		await restHandler.ExecuteRequestAsync(httpClient, HttpMethod.Get, "https://api.test/a", (IHeaderCollection?) null, cancellationToken: CancellationToken);
		
		Assert.Equal("Bearer first", this._handler.Requests[0].Headers["Authorization"]);
		Assert.Equal("Bearer second", this._handler.Requests[1].Headers["Authorization"]);
		Assert.False(this._handler.Requests[2].Headers.ContainsKey("Authorization"));
		Assert.Empty(httpClient.DefaultRequestHeaders);
	}
	
	#endregion
	
	#region Test Types
	
	// ReSharper disable once MemberCanBePrivate.Global
	public sealed class Person
	{
		[JsonPropertyName("name")]
		// ReSharper disable once PropertyCanBeMadeInitOnly.Global
		public string? Name { get; set; }
		
		[JsonPropertyName("age")]
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public int Age { get; set; }
	}
	
	#endregion
}
