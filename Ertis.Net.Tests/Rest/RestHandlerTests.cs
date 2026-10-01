using System.Net;
using Ertis.Net.Http;
using Ertis.Net.Rest;
using Ertis.Net.Tests.TestHelpers;

#pragma warning disable CS0618
namespace Ertis.Net.Tests.Rest;

/// <summary>
/// The legacy (Newtonsoft) rest handler, removed with the Newtonsoft package
/// </summary>
public class RestHandlerTests
{
	#region Fields
	
	private readonly StubHttpMessageHandler _handler = new() { ResponseBody = """{"name":"Jane"}""" };
	
	#endregion
	
	#region Methods
	
	[Fact]
	public async Task ExecuteRequest_DeserializesTheData()
	{
		var restHandler = new RestHandler(this._handler.CreateFactory());
		var cancellationToken = TestContext.Current.CancellationToken;
		
		var typed = await restHandler.ExecuteRequestAsync<Dictionary<string, string>>(HttpMethod.Post, "https://api.test/a", QueryString.Add("q", 1), HeaderCollection.Add("X-A", "1"), new TextRequestBody("x"), cancellationToken: cancellationToken);
		var untyped = await restHandler.ExecuteRequestAsync(HttpMethod.Get, "https://api.test/a", QueryString.Empty, HeaderCollection.Add("Authorization", "Bearer a").Add("Content-Language", "tr"), new TextRequestBody("x"), cancellationToken);
		var syncTyped = restHandler.ExecuteRequest<Dictionary<string, string>>(HttpMethod.Get, "https://api.test/a", QueryString.Empty);
		var syncUntyped = restHandler.ExecuteRequest(HttpMethod.Get, "https://api.test/a", QueryString.Add("q", 2));
		
		Assert.Equal("Jane", typed.Data?["name"]);
		Assert.Equal("""{"name":"Jane"}""", untyped.Json);
		Assert.Equal("Jane", syncTyped.Data?["name"]);
		Assert.True(syncUntyped.IsSuccess);
		Assert.Equal(["/a?q=1", "/a", "/a", "/a?q=2"], this._handler.Requests.Select(x => x.Uri.PathAndQuery));
	}
	
	[Fact]
	public async Task ExecuteRequest_WithAnErrorResponse_ReturnsTheBodyAsTheMessage()
	{
		this._handler.StatusCode = HttpStatusCode.BadRequest;
		var restHandler = new RestHandler(this._handler.CreateFactory());
		
		var typed = await restHandler.ExecuteRequestAsync<Dictionary<string, string>>(HttpMethod.Get, "https://api.test/a", QueryString.Empty, cancellationToken: TestContext.Current.CancellationToken);
		var untyped = restHandler.ExecuteRequest(HttpMethod.Get, "https://api.test/a", QueryString.Empty);
		
		Assert.Equal("""{"name":"Jane"}""", typed.Message);
		Assert.Equal("""{"name":"Jane"}""", untyped.Message);
	}
	
	#endregion
}
