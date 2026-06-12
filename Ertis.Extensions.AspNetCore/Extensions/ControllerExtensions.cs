using System.Globalization;
using Ertis.Core.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Extensions.AspNetCore.Extensions;

public static class ControllerExtensions
{
	#region Methods
	
	extension(ControllerBase controller)
	{
		public string ExtractRequestBody()
		{
			var requestBodyStream = new MemoryStream();
			
			try
			{
				controller.Request.EnableBuffering();
				controller.Request.Body.CopyTo(requestBodyStream);
				requestBodyStream.Seek(0, SeekOrigin.Begin);
				var body = new StreamReader(requestBodyStream).ReadToEnd();
				return body;
			}
			finally
			{
				requestBodyStream.Seek(0, SeekOrigin.Begin);
				controller.Request.Body = requestBodyStream;
			}
		}
		
		public async Task<string> ExtractRequestBodyAsync(CancellationToken cancellationToken = default)
		{
			var requestBodyStream = new MemoryStream();
			
			try
			{
				controller.Request.EnableBuffering();
				await controller.Request.Body.CopyToAsync(requestBodyStream, cancellationToken: cancellationToken);
				requestBodyStream.Seek(0, SeekOrigin.Begin);
				var body = await new StreamReader(requestBodyStream).ReadToEndAsync(cancellationToken: cancellationToken);
				return body;
			}
			finally
			{
				requestBodyStream.Seek(0, SeekOrigin.Begin);
				controller.Request.Body = requestBodyStream;
			}
		}
		
		public string? ExtractWhereQuery(string query, string? defaultValue = null)
		{
			var whereQuery = Helpers.QueryHelper.ExtractWhereQuery(query);
			if (string.IsNullOrEmpty(whereQuery) && defaultValue != null)
			{
				whereQuery = defaultValue;
			}
			
			return whereQuery;
		}
		
		public string? ExtractWhereQuery()
		{
			var query = controller.ExtractRequestBody();
			return controller.ExtractWhereQuery(query);
		}
		
		public async Task<string?> ExtractWhereQueryAsync(CancellationToken cancellationToken = default)
		{
			var query = await controller.ExtractRequestBodyAsync(cancellationToken: cancellationToken);
			return controller.ExtractWhereQuery(query);
		}
		
		public Dictionary<string, bool> ExtractSelectFieldsFromQuery(char separator = ',')
		{
			var selectFields = new Dictionary<string, bool>();
			if (controller.Request.Query.TryGetValue("include", out var includeValues))
			{
				var includeFields = includeValues.ToString().Split(separator);
				foreach (var field in includeFields)
				{
					selectFields.Add(field, true);
				}
			}
			
			if (controller.Request.Query.TryGetValue("exclude", out var excludeValues))
			{
				var excludeFields = excludeValues.ToString().Split(separator);
				foreach (var field in excludeFields)
				{
					// ReSharper disable once RedundantDictionaryContainsKeyBeforeAdding
					if (selectFields.ContainsKey(field))
					{
						selectFields[field] = false;
					}
					else
					{
						selectFields.Add(field, false);
					}
				}
			}
			
			return selectFields;
		}
		
		public void ExtractPaginationParameters(out int? skip, out int? limit, out bool withCount)
		{
			skip = null;
			if (controller.Request.Query.ContainsKey("skip"))
			{
				if (int.TryParse(controller.Request.Query["skip"], out var _skip))
				{
					skip = _skip;
				}
			}
			
			limit = null;
			if (controller.Request.Query.ContainsKey("limit"))
			{
				if (int.TryParse(controller.Request.Query["limit"], out var _limit))
				{
					limit = _limit;
				}
			}
			
			withCount = false;
			if (controller.Request.Query.ContainsKey("with_count"))
			{
				string? withCountString = controller.Request.Query["with_count"];
				withCount = withCountString != null && withCountString.Equals("true", StringComparison.CurrentCultureIgnoreCase);
			}
		}
		
		public void ExtractSortingParameters(out string? sortField, out SortDirection? sortDirection)
		{
			ExtractSortingParameters(controller, out var sorting);
			
			if (sorting != null && sorting.Any())
			{
				sortField = sorting.First().OrderBy;
				sortDirection = sorting.First().SortDirection;
			}
			else
			{
				sortField = null;
				sortDirection = null;
			}
		}
		
		public void ExtractSortingParameters(out Sorting? sorting)
		{
			sorting = null;
			
			if (controller.Request.Query.TryGetValue("sort", out var sortQueryString))
			{
				var sortingExpression = sortQueryString.ToString().Trim();
				if (!string.IsNullOrEmpty(sortingExpression))
				{
					var sortFields = new List<SortField>();
					var sortingExpressions = sortingExpression.Split(';');
					foreach (var sortingParam in sortingExpressions)
					{
						if (!string.IsNullOrEmpty(sortingParam))
						{
							var parts = sortingParam.Replace("%20", " ").Replace("+", " ").Split(' ');
							var sortField = parts.First();
							sortFields.Add(new SortField(sortField, parts.Length > 1 && parts[1].ToLower() == "desc" ? SortDirection.Descending : SortDirection.Ascending));
						}
					}
					
					sorting = new Sorting(sortFields);
				}
			}
		}
		
		public void ExtractDateFilterParameters(out DateTime? startDate, out DateTime? endDate)
		{
			const string format = "dd-MM-yyyy";
			
			startDate = null;
			// ReSharper disable once StringLiteralTypo
			if (controller.Request.Query.ContainsKey("startdate"))
			{
				// ReSharper disable once StringLiteralTypo
				string? startDateString = controller.Request.Query["startdate"];
				if (DateTime.TryParseExact(startDateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
				{
					startDate = dateTime;
				}
			}
			
			endDate = null;
			// ReSharper disable once StringLiteralTypo
			if (controller.Request.Query.ContainsKey("enddate"))
			{
				// ReSharper disable once StringLiteralTypo
				string? endDateString = controller.Request.Query["enddate"];
				if (DateTime.TryParseExact(endDateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
				{
					endDate = dateTime;
				}
			}
		}
	}
	
	#endregion
}