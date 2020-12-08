using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Core.Exceptions;
using Ertis.Core.Models.Response;
using Ertis.Extensions.AspNetCore.Exceptions;
using Ertis.Extensions.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ertis.Extensions.AspNetCore.Controllers
{
	public abstract class QueryControllerBase : ControllerBase
	{
		#region Methods

		protected abstract Task<IPaginationCollection<dynamic>> GetDataAsync(
			string query, 
			int? skip,
			int? limit,
			bool? withCount, 
			string sortField, 
			SortDirection? sortDirection,
			IDictionary<string, bool> selectFields);

		[HttpPost("_query")]
		public async Task<IActionResult> Query()
		{
			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(this.ModelState);
			}

			try
			{
				this.ExtractPaginationParameters(out int? skip, out int? limit, out bool withCount);
				this.ValidatePaginationParams(skip, limit);
				
				var body = await this.ExtractRequestBodyAsync();
				var whereQuery = this.ExtractWhereQuery(body, defaultValue: "{}");
				var selectFields = Helpers.QueryHelper.ExtractSelectFields(body);
				this.ExtractSortingParameters(out string sortField, out SortDirection? sortDirection);
				var result = await this.GetDataAsync(whereQuery, skip, limit, withCount, sortField, sortDirection, selectFields);

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
			catch (Exception ex)
			{
				return this.StatusCode(500, ex.Message);
			}
		}

		private void ValidatePaginationParams(int? skip, int? limit)
		{
			if (skip != null && skip < 0)
			{
				throw new NegativeSkipException();
			}
			
			if (limit != null && limit < 0)
			{
				throw new NegativeLimitException();
			}
		}

		#endregion
	}
}