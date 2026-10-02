using System.Globalization;
using System.Text;

namespace Ertis.MongoDB.Queries;

internal static class QueryHelper
{
	#region Methods
	
	internal static string GetOperatorTag(MongoOperator mongoOperator)
	{
		return mongoOperator switch
		{
			MongoOperator.Equals => "eq",
			MongoOperator.NotEquals => "ne",
			MongoOperator.GreaterThan => "gt",
			MongoOperator.GreaterThanOrEqual => "gte",
			MongoOperator.LessThan => "lt",
			MongoOperator.LessThanOrEqual => "lte",
			MongoOperator.Contains => "in",
			MongoOperator.NotContains => "nin",
			MongoOperator.And => "and",
			MongoOperator.Or => "or",
			MongoOperator.Nor => "nor",
			MongoOperator.Not => "not",
			MongoOperator.Exists => "exists",
			MongoOperator.TypeOf => "type",
			MongoOperator.Regex => "regex",
			MongoOperator.Text => "text",
			MongoOperator.RegexOptions => "options",
			MongoOperator.TextSearch => "search",
			MongoOperator.TextSearchLanguage => "language",
			MongoOperator.TextSearchCaseSensitive => "caseSensitive",
			MongoOperator.TextSearchDiacriticSensitive => "diacriticSensitive",
			MongoOperator.ElemMatch => "elemMatch",
			MongoOperator.All => "all",
			MongoOperator.Size => "size",
			MongoOperator.Mod => "mod",
			_ => throw new ArgumentOutOfRangeException(nameof(mongoOperator), mongoOperator, null)
		};
	}
	
	internal static string? ConvertRegexOptions(RegexOptions? options)
	{
		if (options == null)
		{
			return null;
		}
		
		var builder = new StringBuilder(4);
		if (options.Value.HasFlag(RegexOptions.AllowDot))
		{
			builder.Append('s');
		}
		
		if (options.Value.HasFlag(RegexOptions.Extended))
		{
			builder.Append('x');
		}
		
		if (options.Value.HasFlag(RegexOptions.Multiline))
		{
			builder.Append('m');
		}
		
		if (options.Value.HasFlag(RegexOptions.CaseInsensitivity))
		{
			builder.Append('i');
		}
		
		return builder.Length > 0 ? builder.ToString() : null;
	}
	
	/// <summary>
	/// Converts the date time to UTC before it is written with a 'Z' suffix (a date time without a kind is UTC)
	/// </summary>
	internal static DateTime ToUniversalTime(DateTime dateTime)
	{
		return dateTime.Kind switch
		{
			DateTimeKind.Utc => dateTime,
			DateTimeKind.Local => dateTime.ToUniversalTime(),
			_ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
		};
	}
	
	/// <summary>
	/// Writes the text as a json string (quoted and escaped), so a value or a field name can not change the structure of the query.
	/// Only the characters that json requires are escaped (quote, backslash and the control characters), the others are kept as they are.
	/// </summary>
	internal static string ToJsonString(string text)
	{
		var builder = new StringBuilder(text.Length + 2);
		builder.Append('"');
		foreach (var character in text)
		{
			switch (character)
			{
				case '"':
					builder.Append("\\\"");
					break;
				case '\\':
					builder.Append("\\\\");
					break;
				case '\n':
					builder.Append("\\n");
					break;
				case '\r':
					builder.Append("\\r");
					break;
				case '\t':
					builder.Append("\\t");
					break;
				case '\b':
					builder.Append("\\b");
					break;
				case '\f':
					builder.Append("\\f");
					break;
				case < ' ':
					builder.Append("\\u").Append(((int) character).ToString("x4", CultureInfo.InvariantCulture));
					break;
				default:
					builder.Append(character);
					break;
			}
		}
		
		builder.Append('"');
		return builder.ToString();
	}
	
	/// <summary>
	/// A date in extended json (a strict json, which MongoDB reads as a date, not as a string)
	/// </summary>
	internal static string ToExtendedJsonDate(DateTime utcDateTime)
	{
		return "{ \"$date\": \"" + utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture) + "\" }";
	}
	
	/// <summary>
	/// NaN and the infinities in extended json (json numbers can't express them)
	/// </summary>
	internal static string ToExtendedJsonDouble(double value)
	{
		var text = double.IsNaN(value) ? "NaN" : double.IsPositiveInfinity(value) ? "Infinity" : "-Infinity";
		return "{ \"$numberDouble\": \"" + text + "\" }";
	}
	
	internal static string GetInnerQuery(IQuery query)
	{
		var expressionJson = query.ToString();
		if (!string.IsNullOrEmpty(expressionJson))
		{
			expressionJson = expressionJson.Trim();
			if (expressionJson.StartsWith('{') && expressionJson.EndsWith('}'))
			{
				expressionJson = expressionJson.TrimStart('{');
				expressionJson = expressionJson.TrimEnd('}');
				expressionJson = expressionJson.Trim();   
			}
		}
		
		return expressionJson;
	}
	
	#endregion
}