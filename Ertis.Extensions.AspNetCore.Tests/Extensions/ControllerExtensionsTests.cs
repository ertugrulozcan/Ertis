using Ertis.Core.Collections;
using Ertis.Extensions.AspNetCore.Extensions;
using Ertis.Extensions.AspNetCore.Tests.TestHelpers;

namespace Ertis.Extensions.AspNetCore.Tests.Extensions;

public class ControllerExtensionsTests
{
	#region Request Body Methods
	
	[Fact]
	public void ExtractRequestBody_ReturnsTheBodyAndKeepsItReadable()
	{
		var controller = TestQueryController.Create("""{ "a": 1 }""");
		
		var first = controller.ExtractRequestBody();
		var second = controller.ExtractRequestBody();
		
		Assert.Equal("""{ "a": 1 }""", first);
		Assert.Equal(first, second);
	}
	
	[Fact]
	public async Task ExtractRequestBodyAsync_ReturnsTheBodyAndKeepsItReadable()
	{
		var controller = TestQueryController.Create("""{ "a": 1 }""");
		
		var first = await controller.ExtractRequestBodyAsync(TestContext.Current.CancellationToken);
		var second = await controller.ExtractRequestBodyAsync(TestContext.Current.CancellationToken);
		
		Assert.Equal("""{ "a": 1 }""", first);
		Assert.Equal(first, second);
	}
	
	[Fact]
	public async Task ExtractWhereQuery_ReturnsTheWhereNode()
	{
		var controller = TestQueryController.Create("""{ "where": { "a": 1 } }""");
		
		// ReSharper disable once MethodHasAsyncOverload
		Assert.Equal(1, System.Text.Json.Nodes.JsonNode.Parse(controller.ExtractWhereQuery()!)!["a"]!.GetValue<int>());
		Assert.Equal(1, System.Text.Json.Nodes.JsonNode.Parse((await controller.ExtractWhereQueryAsync(TestContext.Current.CancellationToken))!)!["a"]!.GetValue<int>());
	}
	
	[Fact]
	public void ExtractWhereQuery_WithoutAWhereNode_ReturnsTheDefault()
	{
		var controller = TestQueryController.Create("{}");
		
		Assert.Null(controller.ExtractWhereQuery("""{ "a": 1 }"""));
		Assert.Equal("fallback", controller.ExtractWhereQuery("""{ "a": 1 }""", "fallback"));
	}
	
	#endregion
	
	#region Query String Methods
	
	[Fact]
	public void ExtractSelectFieldsFromQuery_ReadsIncludeAndExclude()
	{
		var controller = TestQueryController.Create("{}", "?include=name,age&exclude=secret,age");
		
		var selectFields = controller.ExtractSelectFieldsFromQuery();
		
		Assert.Equal(new Dictionary<string, bool> { ["name"] = true, ["age"] = false, ["secret"] = false }, selectFields);
	}
	
	[Theory]
	[InlineData("", null, null, false)]
	[InlineData("?skip=5&limit=10&with_count=true", 5, 10, true)]
	[InlineData("?skip=x&limit=&with_count=yes", null, null, false)]
	public void ExtractPaginationParameters_ReadsTheParameters(string queryString, int? expectedSkip, int? expectedLimit, bool expectedWithCount)
	{
		var controller = TestQueryController.Create("{}", queryString);
		
		controller.ExtractPaginationParameters(out var skip, out var limit, out var withCount);
		
		Assert.Equal(expectedSkip, skip);
		Assert.Equal(expectedLimit, limit);
		Assert.Equal(expectedWithCount, withCount);
	}
	
	[Fact]
	public void ExtractSortingParameters_ReadsEverySortField()
	{
		var controller = TestQueryController.Create("{}", "?sort=name+desc;age;created_at%20DESC");
		
		controller.ExtractSortingParameters(out Sorting? sorting);
		
		Assert.NotNull(sorting);
		Assert.Equal(["name", "age", "created_at"], sorting.Select(x => x.OrderBy));
		Assert.Equal([SortDirection.Descending, SortDirection.Ascending, SortDirection.Descending], sorting.Select(x => x.SortDirection));
	}
	
	[Fact]
	public void ExtractSortingParameters_WithoutSort_ReturnsNull()
	{
		var controller = TestQueryController.Create("{}");
		
		controller.ExtractSortingParameters(out string? sortField, out SortDirection? sortDirection);
		
		Assert.Null(sortField);
		Assert.Null(sortDirection);
	}
	
	[Theory]
	[InlineData("?startdate=01-02-2026&enddate=28-02-2026", "2026-02-01", "2026-02-28")]
	[InlineData("?startdate=2026-02-01", null, null)]
	public void ExtractDateFilterParameters_ReadsTheDates(string queryString, string? expectedStart, string? expectedEnd)
	{
		var controller = TestQueryController.Create("{}", queryString);
		
		controller.ExtractDateFilterParameters(out var startDate, out var endDate);
		
		Assert.Equal(expectedStart == null ? null : DateTime.Parse(expectedStart), startDate);
		Assert.Equal(expectedEnd == null ? null : DateTime.Parse(expectedEnd), endDate);
	}
	
	#endregion
}
