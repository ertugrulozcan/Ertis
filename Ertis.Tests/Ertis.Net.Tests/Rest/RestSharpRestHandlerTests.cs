using System.Net.Http;
using System.Threading.Tasks;
using Ertis.Net.Http;
using Ertis.Net.Rest;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.Net.Tests.Rest
{
	public class RestSharpRestHandlerTests
	{
		#region Services

		private IRestHandler restHandler;

		#endregion
		
		#region Initialize

		[SetUp]
		public void Setup()
		{
			this.restHandler = new SystemRestHandler();
		}

		#endregion
		
		#region Methods

		[Test]
		public async Task GetMembershipTestAsync()
		{
			const string membershipId = "6049f26d3510dfe328774d63";
			const string applicationId = "604a6d663510dfe328774d6c";
			const string secret = "RLCUYNFBBMBUBIYMQAGKOPMWYUZOYOCT";
			
			var getMembershipResponse = await this.restHandler.ExecuteRequestAsync(
				HttpMethod.Get, 
				$"http://localhost:9716/api/v1/memberships/{membershipId}", 
				HeaderCollection.Add("Authorization", $"Basic {applicationId}:{secret}"));
				
			if (getMembershipResponse.IsSuccess)
			{
				var statusCode = getMembershipResponse.StatusCode;
			}
			else
			{
				var statusCode = getMembershipResponse.StatusCode;
			}
		}

		#endregion
	}
}