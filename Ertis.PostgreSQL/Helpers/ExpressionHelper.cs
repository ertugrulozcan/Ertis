using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace Ertis.PostgreSQL.Helpers
{
	internal static class ExpressionHelper
	{
		internal static Expression<Func<T, bool>> ParseExpression<T>(string query) => ParseExpressionAsync<T>(query).ConfigureAwait(false).GetAwaiter().GetResult();
		
		internal static async ValueTask<Expression<Func<T, bool>>> ParseExpressionAsync<T>(string query)
		{
			var options = ScriptOptions.Default.AddReferences(typeof(T).Assembly);
			var func = await CSharpScript.EvaluateAsync<Func<T, bool>>(query, options);
			var expression = Expression.Lambda<Func<T, bool>>(Expression.Call(func.Method));
			return expression;
		}
		
		internal static Expression<Func<TEntity, object>> ConvertSortExpression<TEntity>(string sortField)
		{
			Expression<Func<TEntity, object>> sortExpression = null;
			if (!string.IsNullOrEmpty(sortField))
			{
				var type = typeof(TEntity);
				var propertyInfo = type.GetProperty(sortField);
				if (propertyInfo == null)
				{
					propertyInfo = type.GetProperties().FirstOrDefault(x => x
						.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
						.Cast<JsonPropertyAttribute>()
						.FirstOrDefault(y => y.PropertyName == sortField) != null);	
				}

				if (propertyInfo != null)
				{
					var param = Expression.Parameter(typeof(TEntity), "item");
					sortExpression = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(Expression.Property(param, propertyInfo), typeof(object)), param);
				}
				else
				{
					Console.WriteLine($"Unknown property as sorting field ('{sortField}')");
				}
			}

			return sortExpression;
		}
	}
}