using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ertis.Core.Collections;
using Microsoft.AspNetCore.Mvc;

namespace Ertis.Extensions.AspNetCore.Extensions
{
	public static class ControllerExtensions
	{
		#region Methods

		public static string ExtractRequestBody(this ControllerBase controller)
		{
			using var reader = new StreamReader(controller.Request.Body, Encoding.UTF8);
			return reader.ReadToEnd();
		}
		
		public static async Task<string> ExtractRequestBodyAsync(this ControllerBase controller)
		{
			using var reader = new StreamReader(controller.Request.Body, Encoding.UTF8);
			return await reader.ReadToEndAsync();
		}
		
		public static string ExtractWhereQuery(this ControllerBase controller, string query)
		{
			var whereQuery = Helpers.QueryHelper.ExtractWhereQuery(query);
			return whereQuery;
		}
		
		public static string ExtractWhereQuery(this ControllerBase controller)
		{
			string query = controller.ExtractRequestBody();
			return controller.ExtractWhereQuery(query);
		}
		
		public static async Task<string> ExtractWhereQueryAsync(this ControllerBase controller)
		{
			string query = await controller.ExtractRequestBodyAsync();
			return controller.ExtractWhereQuery(query);
		}
		
		public static void ExtractPaginationParameters(this ControllerBase controller, out int? skip, out int? limit, out bool withCount)
		{
			skip = null;
			if (controller.Request.Query.ContainsKey("skip"))
			{
				if (int.TryParse(controller.Request.Query["skip"], out int _skip))
				{
					skip = _skip;
				}
			}

			limit = null;
			if (controller.Request.Query.ContainsKey("limit"))
			{
				if (int.TryParse(controller.Request.Query["limit"], out int _limit))
				{
					limit = _limit;
				}
			}

			withCount = false;
			if (controller.Request.Query.ContainsKey("with_count"))
			{
				string withCountString = controller.Request.Query["with_count"];
				withCount = withCountString.ToLower() == "true";
			}
		}

		public static void ExtractSortingParameters(this ControllerBase controller, out string sortField, out SortDirection? sortDirection)
		{
			sortField = null;
			sortDirection = null;
			
			if (controller.Request.Query.ContainsKey("sort"))
			{
				string sortingExpression = controller.Request.Query["sort"].ToString();
				sortingExpression = sortingExpression.Replace("%20", " ");
				var parts = sortingExpression.Split(' ');
				sortField = parts.First();
				if (parts.Length > 1 && parts[1].ToLower() == "desc")
				{
					sortDirection = SortDirection.Descending;
				}
			}
		}
		
		public static void ExtractDateFilterParameters(this ControllerBase controller, out DateTime? startDate, out DateTime? endDate)
		{
			const string format = "dd-MM-yyyy";
			
			startDate = null;
			if (controller.Request.Query.ContainsKey("startdate"))
			{
				string startDateString = controller.Request.Query["startdate"];
				if (DateTime.TryParseExact(startDateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
				{
					startDate = dateTime;
				}
			}
			
			endDate = null;
			if (controller.Request.Query.ContainsKey("enddate"))
			{
				string endDateString = controller.Request.Query["enddate"];
				if (DateTime.TryParseExact(endDateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
				{
					endDate = dateTime;
				}
			}
		}
		
		#endregion
	}
}