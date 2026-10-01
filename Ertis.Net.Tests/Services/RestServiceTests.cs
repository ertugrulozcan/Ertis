using Ertis.Core.Models.Response;
using Ertis.Net.Http;
using Ertis.Net.Rest;
using Ertis.Net.Services;
using NSubstitute;

namespace Ertis.Net.Tests.Services;

public class RestServiceTests
{
	#region Test Types
	
	private sealed class TestRestService(IRestHandler restHandler) : RestService(restHandler)
	{
		public async Task CallAllAsync(CancellationToken cancellationToken)
		{
			var query = QueryString.Add("q", 1);
			this.Get("u", query);
			this.Get<int>("u", query);
			this.Post("u", query);
			this.Post<int>("u", query);
			this.Put("u", query);
			this.Put<int>("u", query);
			this.Delete("u", query);
			this.Delete<int>("u", query);
			await this.GetAsync("u", query, cancellationToken: cancellationToken);
			await this.GetAsync<int>("u", query, cancellationToken: cancellationToken);
			await this.PostAsync("u", query, cancellationToken: cancellationToken);
			await this.PostAsync<int>("u", query, cancellationToken: cancellationToken);
			await this.PutAsync("u", query, cancellationToken: cancellationToken);
			await this.PutAsync<int>("u", query, cancellationToken: cancellationToken);
			await this.DeleteAsync("u", query, cancellationToken: cancellationToken);
			await this.DeleteAsync<int>("u", query, cancellationToken: cancellationToken);
		}
	}
	
	#endregion
	
	#region Methods
	
	[Fact]
	public async Task Methods_DelegateToTheRestHandlerWithTheirHttpMethods()
	{
		var restHandler = Substitute.For<IRestHandler>();
		
		await new TestRestService(restHandler).CallAllAsync(TestContext.Current.CancellationToken);
		
		var methods = restHandler.ReceivedCalls().Select(x => ((HttpMethod) x.GetArguments()[0]!).Method).ToArray();
		Assert.Equal(["GET", "GET", "POST", "POST", "PUT", "PUT", "DELETE", "DELETE", "GET", "GET", "POST", "POST", "PUT", "PUT", "DELETE", "DELETE"], methods);
		Assert.All(restHandler.ReceivedCalls(), x => Assert.Equal("u", x.GetArguments()[1]));
	}
	
	#endregion
}
