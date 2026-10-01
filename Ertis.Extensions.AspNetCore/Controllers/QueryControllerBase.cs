using Ertis.Core.Collections;
using Ertis.Core.Exceptions;
using Ertis.Core.Models;
using Ertis.Extensions.AspNetCore.Exceptions;
using Ertis.Extensions.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ertis.Extensions.AspNetCore.Controllers;

public abstract class QueryControllerBase : ControllerBase
{
	#region Methods
	
	protected abstract Task<IPaginationCollection<dynamic>> GetDataAsync(
		string query, 
		int? skip,
		int? limit,
		bool? withCount, 
		string? sortField, 
		SortDirection? sortDirection,
		IDictionary<string, bool> projection,
		// ReSharper disable once UnusedParameter.Global
		CancellationToken cancellationToken = default);
	
	[HttpPost("_query")]
	public virtual async Task<IActionResult> Query(CancellationToken cancellationToken = default)
	{
		if (!this.ModelState.IsValid)
		{
			return this.BadRequest(this.ModelState);
		}
		
		try
		{
			this.ExtractPaginationParameters(out var skip, out var limit, out var withCount);
			this.ValidatePaginationParams(skip, limit);
			
			var body = await this.ExtractRequestBodyAsync(cancellationToken: cancellationToken);
			string? whereQuery;
			Dictionary<string, bool> projection;
			try
			{
				whereQuery = this.ExtractWhereQuery(body, body);
				projection = Helpers.QueryHelper.ExtractProjection(body);
			}
			catch (System.Text.Json.JsonException ex)
			{
				throw new InvalidQueryException(ex);
			}
			
			this.ExtractSortingParameters(out var sortField, out var sortDirection);
			var result = await this.GetDataAsync(whereQuery ?? string.Empty, skip, limit, withCount, sortField, sortDirection, projection, cancellationToken: cancellationToken);
			
			return this.Ok(result);
		}
		catch (HttpStatusCodeException ex)
		{
			if (ex is IHasErrorModel errorModelException)
			{
				return this.StatusCode((int)ex.StatusCode, errorModelException.Error);
			}
			else
			{
				return this.StatusCode((int)ex.StatusCode, ex.Message);
			}
		}
	}
	
	private void ValidatePaginationParams(int? skip, int? limit)
	{
		if (skip < 0)
		{
			throw new NegativeSkipException();
		}
		
		if (limit < 0)
		{
			throw new NegativeLimitException();
		}
	}
	
	#endregion
}