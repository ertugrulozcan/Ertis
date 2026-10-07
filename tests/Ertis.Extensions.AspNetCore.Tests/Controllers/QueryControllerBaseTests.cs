using System.Net;
using System.Text.Json.Nodes;
using Ertis.Core.Collections;
using Ertis.Core.Exceptions;
using Ertis.Core.Models;
using Ertis.Extensions.AspNetCore.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace Ertis.Extensions.AspNetCore.Tests.Controllers;

public class QueryControllerBaseTests
{
	#region Query Body Methods
	
	[Fact]
	public async Task Query_WithAWhereNode_PassesTheWhereQuery()
	{
		var controller = TestQueryController.Create("""{ "where": { "name": "Jane", "age": { "$gt": 18 } } }""");
		
		var result = await controller.Query(TestContext.Current.CancellationToken);
		
		Assert.IsType<OkObjectResult>(result);
		AssertJsonEquals("""{ "name": "Jane", "age": { "$gt": 18 } }""", controller.ReceivedQuery);
	}
	
	[Fact]
	public async Task Query_WithoutAWhereNode_PassesTheWholeBody()
	{
		const string body = """{ "name": "Jane" }""";
		var controller = TestQueryController.Create(body);
		
		await controller.Query(TestContext.Current.CancellationToken);
		
		Assert.Equal(body, controller.ReceivedQuery);
	}
	
	/// <summary>
	/// Characterization: an empty body is an empty query (all the documents)
	/// </summary>
	[Fact]
	public async Task Query_WithAnEmptyBody_PassesAnEmptyQuery()
	{
		var controller = TestQueryController.Create(string.Empty);
		
		var result = await controller.Query(TestContext.Current.CancellationToken);
		
		Assert.IsType<OkObjectResult>(result);
		Assert.Equal(string.Empty, controller.ReceivedQuery);
	}
	
	[Fact]
	public async Task Query_WithASelectNode_PassesTheProjection()
	{
		var controller = TestQueryController.Create("""{ "where": {}, "select": { "name": 1, "age": true, "secret": 0, "other": false, "ignored": "x" } }""");
		
		await controller.Query(TestContext.Current.CancellationToken);
		
		Assert.Equal(new Dictionary<string, bool> { ["name"] = true, ["age"] = true, ["secret"] = false, ["other"] = false }, controller.Projection);
	}
	
	[Fact]
	public async Task Query_ReturnsTheData()
	{
		var controller = TestQueryController.Create("{}");
		
		var result = Assert.IsType<OkObjectResult>(await controller.Query(TestContext.Current.CancellationToken));
		
		Assert.Same(controller.Data, result.Value);
	}
	
	#endregion
	
	#region Query String Methods
	
	[Fact]
	public async Task Query_WithPaginationAndSorting_PassesTheParameters()
	{
		var controller = TestQueryController.Create("{}", "?skip=10&limit=20&with_count=TRUE&sort=name%20desc");
		
		await controller.Query(TestContext.Current.CancellationToken);
		
		Assert.Equal(10, controller.Skip);
		Assert.Equal(20, controller.Limit);
		Assert.Equal(true, controller.WithCount);
		Assert.Equal("name", controller.SortField);
		Assert.Equal(SortDirection.Descending, controller.SortDirection);
	}
	
	[Theory]
	[InlineData("?skip=-1", "NegativeSkipError")]
	[InlineData("?limit=-1", "NegativeLimitError")]
	public async Task Query_WithNegativePagination_ReturnsBadRequest(string queryString, string expectedErrorCode)
	{
		var controller = TestQueryController.Create("{}", queryString);
		
		var result = Assert.IsType<ObjectResult>(await controller.Query(TestContext.Current.CancellationToken));
		
		Assert.Equal(400, result.StatusCode);
		Assert.Equal(expectedErrorCode, Assert.IsType<ErrorModel>(result.Value).ErrorCode);
		Assert.Null(controller.ReceivedQuery);
	}
	
	#endregion
	
	#region Error Methods
	
	[Fact]
	public async Task Query_WithAnInvalidModelState_ReturnsBadRequest()
	{
		var controller = TestQueryController.Create("{}");
		controller.ModelState.AddModelError("field", "invalid");
		
		var result = await controller.Query(TestContext.Current.CancellationToken);
		
		Assert.IsType<BadRequestObjectResult>(result);
	}
	
	[Fact]
	public async Task Query_WhenTheDataThrowsAnErtisException_ReturnsItsErrorModel()
	{
		var controller = TestQueryController.Create("{}");
		controller = new TestQueryController { ControllerContext = controller.ControllerContext, DataException = new TestErtisException() };
		
		var result = Assert.IsType<ObjectResult>(await controller.Query(TestContext.Current.CancellationToken));
		
		Assert.Equal(409, result.StatusCode);
		var error = Assert.IsType<ErrorModel>(result.Value);
		Assert.Equal("TestConflict", error.ErrorCode);
		Assert.Equal("A conflict", error.Message);
	}
	
	[Theory]
	[InlineData("{ invalid")]
	[InlineData("""{ "where": { "a": } }""")]
	[InlineData("""{ where: { age: { $gt: 18 } } }""")]
	public async Task Query_WithAnInvalidJsonBody_ReturnsBadRequest(string body)
	{
		var controller = TestQueryController.Create(body);
		
		var result = Assert.IsType<ObjectResult>(await controller.Query(TestContext.Current.CancellationToken));
		
		Assert.Equal(400, result.StatusCode);
		Assert.Equal("InvalidQuery", Assert.IsType<ErrorModel>(result.Value).ErrorCode);
		Assert.Null(controller.ReceivedQuery);
	}
	
	[Fact]
	public async Task Query_WhenTheDataThrows_PropagatesTheException()
	{
		var controller = TestQueryController.Create("{}");
		controller = new TestQueryController { ControllerContext = controller.ControllerContext, DataException = new InvalidOperationException("database error") };
		
		await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Query(TestContext.Current.CancellationToken));
	}
	
	private static void AssertJsonEquals(string expected, string? actual)
	{
		Assert.NotNull(actual);
		Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)), actual);
	}
	
	#endregion
	
	#region Test Types
	
	private sealed class TestErtisException() : ErtisException(HttpStatusCode.Conflict, "A conflict", "TestConflict");
	
	#endregion
}
