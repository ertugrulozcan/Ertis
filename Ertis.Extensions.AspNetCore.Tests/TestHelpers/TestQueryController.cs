using System.Text;
using Ertis.Core.Collections;
using Ertis.Extensions.AspNetCore.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Ertis.Extensions.AspNetCore.Tests.TestHelpers;

/// <summary>
/// A query controller that records the parameters of the data request
/// </summary>
public sealed class TestQueryController : QueryControllerBase
{
	#region Properties
	
	public string? ReceivedQuery { get; private set; }
	
	public int? Skip { get; private set; }
	
	public int? Limit { get; private set; }
	
	public bool? WithCount { get; private set; }
	
	public string? SortField { get; private set; }
	
	public SortDirection? SortDirection { get; private set; }
	
	public IDictionary<string, bool>? Projection { get; private set; }
	
	/// <summary>
	/// The exception thrown by the data request (e.g. a database error)
	/// </summary>
	public Exception? DataException { get; init; }
	
	public IPaginationCollection<dynamic> Data { get; init; } = new PaginationCollection<dynamic> { Count = 1, Items = [new { name = "Jane" }] };
	
	#endregion
	
	#region Methods
	
	public static TestQueryController Create(string? body, string queryString = "")
	{
		var httpContext = new DefaultHttpContext
		{
			Request =
			{
				Method = "POST",
				QueryString = new QueryString(queryString),
				Body = new MemoryStream(Encoding.UTF8.GetBytes(body ?? string.Empty))
			}
		};
		
		return new TestQueryController
		{
			ControllerContext = new ControllerContext { HttpContext = httpContext }
		};
	}
	
	protected override Task<IPaginationCollection<dynamic>> GetDataAsync(
		string query,
		int? skip,
		int? limit,
		bool? withCount,
		string? sortField,
		SortDirection? sortDirection,
		IDictionary<string, bool> projection,
		CancellationToken cancellationToken = default)
	{
		this.ReceivedQuery = query;
		this.Skip = skip;
		this.Limit = limit;
		this.WithCount = withCount;
		this.SortField = sortField;
		this.SortDirection = sortDirection;
		this.Projection = projection;
		
		if (this.DataException != null)
		{
			throw this.DataException;
		}
		
		return Task.FromResult(this.Data);
	}
	
	#endregion
}
