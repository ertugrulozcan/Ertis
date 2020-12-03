using System;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Ertis.Extensions.AspNetCore.Exceptions;
using Ertis.Extensions.AspNetCore.Extensions;
using Ertis.Extensions.AspNetCore.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace Ertis.Extensions.AspNetCore.Controllers
{
	public abstract class QueryControllerBase<T> : ControllerBase
	{
		#region Methods

		protected abstract Task<IPaginationCollection<T>> GetDataAsync(
			string query, 
			int? skip,
			int? limit,
			bool? withCount, 
			string sortField, 
			SortDirection? sortDirection);

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
				this.ExtractSortingParameters(out string sortField, out SortDirection? sortDirection);

				var body = await this.ExtractRequestBodyAsync();
				var whereQuery = this.ExtractWhereQuery(body);
				var result = await this.GetDataAsync(whereQuery, skip, limit, withCount, sortField, sortDirection);
				
				var selectFields = Helpers.QueryHelper.ExtractSelectFields(body);
				var projectinatedCollection = result.ExecuteSelectQuery(selectFields);
			
				return this.Ok(projectinatedCollection);
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

		#endregion
	}
}