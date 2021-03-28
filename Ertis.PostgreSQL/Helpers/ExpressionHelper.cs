using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Ertis.PostgreSQL.Helpers
{
	public static class ExpressionHelper
	{
		public static Expression<Func<T, bool>> ParseExpression<T>(string query) => ParseExpressionAsync<T>(query).ConfigureAwait(false).GetAwaiter().GetResult();
		
		public static async Task<Expression<Func<T, bool>>> ParseExpressionAsync<T>(string query)
		{
			var options = ScriptOptions.Default.AddReferences(typeof(T).Assembly);
			var func = await CSharpScript.EvaluateAsync<Func<T, bool>>(query, options);
			var expression = Expression.Lambda<Func<T, bool>>(Expression.Call(func.Method));
			return expression;
		}
	}
}