using System.Buffers;
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.MongoDB.Queries;

public static class QueryBuilder
{
	#region Constants
	
	private static readonly SearchValues<char> RegexSpecialCharacters = SearchValues.Create("\\^$.|?*+()[]{}/");
	
	#endregion
	
	#region BuildIn Operators
	
	/// <summary>
	/// ObjectId()
	/// </summary>
	/// <param name="id">Object ID</param>
	public static IQueryExpression ObjectId(string id)
	{
		return new ObjectId(id);
	}
	
	/// <summary>
	/// ISODate()
	/// </summary>
	/// <param name="date">ISODate</param>
	public static IQuery ISODate(DateTime date)
	{
		return new ISODate(date);
	}
	
	#endregion
	
	#region Where Methods
	
	public static IQuery Where(string key, string value)
	{
		return WhereCore(key, value);
	}
	
	public static IQuery Where(string key, char value)
	{
		return WhereCore(key, value);
	}
	
	public static IQuery Where(string key, int value)
	{
		return WhereCore(key, value);
	}
	
	public static IQuery Where(string key, long value)
	{
		return WhereCore(key, value);
	}
	
	public static IQuery Where(string key, double value)
	{
		return WhereCore(key, value);
	}
	
	public static IQuery Where(string key, float value)
	{
		return WhereCore(key, value);
	}
	
	public static IQuery Where(string key, bool value)
	{
		return WhereCore(key, value);
	}
	
	public static IQuery Where(string key, DateTime value)
	{
		return WhereCore(key, value);
	}
	
	private static IQuery WhereCore<T>(string key, T value)
	{
		return WhereCore(new []
		{
			Equals(key, value)
		});
	}
	
	public static IQuery WhereOut(string key, string value)
	{
		return WhereOutCore(key, value);
	}
	
	public static IQuery WhereOut(string key, char value)
	{
		return WhereOutCore(key, value);
	}
	
	public static IQuery WhereOut(string key, int value)
	{
		return WhereOutCore(key, value);
	}
	
	public static IQuery WhereOut(string key, long value)
	{
		return WhereOutCore(key, value);
	}
	
	public static IQuery WhereOut(string key, double value)
	{
		return WhereOutCore(key, value);
	}
	
	public static IQuery WhereOut(string key, float value)
	{
		return WhereOutCore(key, value);
	}
	
	public static IQuery WhereOut(string key, bool value)
	{
		return WhereOutCore(key, value);
	}
	
	public static IQuery WhereOut(string key, DateTime value)
	{
		return WhereOutCore(key, value);
	}
	
	private static IQuery WhereOutCore<T>(string key, T value)
	{
		return WhereCore(new []
		{
			Equals(key, value)
		}, true);
	}
	
	public static IQuery Where(string key, IEnumerable<IQuery> queries)
	{
		return new QueryExpression
		{
			Field = key,
			Value = Combine(queries)
		};
	}
	
	public static IQuery Where(IEnumerable<IQuery> queries)
	{
		return WhereCore(queries);
	}
	
	public static IQuery WhereOut(IEnumerable<IQuery> queries)
	{
		return WhereCore(queries, true);
	}
	
	public static IQuery Where(params IQuery[] queries)
	{
		return WhereCore(queries);
	}
	
	public static IQuery WhereOut(params IQuery[] queries)
	{
		return WhereCore(queries, true);
	}
	
	private static IQuery WhereCore(IEnumerable<IQuery> queries, bool showOperatorTag = false)
	{
		return new CustomQuery
		{
			Operator = "where",
			Children = Compose(queries),
			ShowOperatorTag = showOperatorTag
		};
	}
	
	#endregion
	
	#region Projection Methods
	
	public static IQuery Select(IDictionary<string, bool> selections)
	{
		return new CustomQuery
		{
			Operator = "select",
			Children = selections.Select(x => Equals(x.Key, new QueryValue<int>(x.Value ? 1 : 0))).Cast<IQuery>().ToList()
		};
	}
	
	#endregion
	
	#region Merge Queries
	
	public static IQuery Combine(params IQuery[] queries)
	{
		return CombineCore(queries);
	}
	
	public static IQuery Combine(IEnumerable<IQuery> queries)
	{
		return CombineCore(queries);
	}
	
	private static IQuery CombineCore(IEnumerable<IQuery> queries)
	{
		return new CustomQuery
		{
			Children = Compose(queries)
		};
	}
	
	public static IQueryExpression Combine(string key, params IQuery[] expressions)
	{
		return new QueryExpression
		{
			Field = key,
			Value = Combine(expressions)
		};
	}
	
	#endregion
	
	#region Comparison Queries
	
	/// <summary>
	/// Equals ($eq)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression Equals<T>(string key, T value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new QueryValue<T>(value)
		};
	}
	
	/// <summary>
	/// Equals ($eq)
	/// </summary>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery Equals<T>(T value)
	{
		return new Query
		{
			Operator = MongoOperator.Equals,
			Value = new QueryValue<T>(value)
		};
	}
	
	/// <summary>
	/// NotEquals ($ne)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression NotEquals<T>(string key, T value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.NotEquals,
				Value = new QueryValue<T>(value)   
			}
		};
	}
	
	/// <summary>
	/// NotEquals ($ne)
	/// </summary>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery NotEquals<T>(T value)
	{
		return new Query
		{
			Operator = MongoOperator.NotEquals,
			Value = new QueryValue<T>(value)   
		};
	}
	
	/// <summary>
	/// GreaterThan ($gt)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression GreaterThan<T>(string key, T value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.GreaterThan,
				Value = new QueryValue<T>(value)   
			}
		};
	}
	
	/// <summary>
	/// GreaterThan ($gt)
	/// </summary>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	/// <returns></returns>
	public static IQuery GreaterThan<T>(T value)
	{
		return new Query
		{
			Operator = MongoOperator.GreaterThan,
			Value = new QueryValue<T>(value)   
		};
	}
	
	/// <summary>
	/// GreaterThanOrEqual ($gte)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression GreaterThanOrEqual<T>(string key, T value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.GreaterThanOrEqual,
				Value = new QueryValue<T>(value)   
			}
		};
	}
	
	/// <summary>
	/// GreaterThanOrEqual ($gte)
	/// </summary>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery GreaterThanOrEqual<T>(T value)
	{
		return new Query
		{
			Operator = MongoOperator.GreaterThanOrEqual,
			Value = new QueryValue<T>(value)
		};
	}
	
	/// <summary>
	/// LessThan ($lt)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression LessThan<T>(string key, T value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.LessThan,
				Value = new QueryValue<T>(value)   
			}
		};
	}
	
	/// <summary>
	/// LessThan ($lt)
	/// </summary>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery LessThan<T>(T value)
	{
		return new Query
		{
			Operator = MongoOperator.LessThan,
			Value = new QueryValue<T>(value)   
		};
	}
	
	/// <summary>
	/// LessThanOrEqual ($lte)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression LessThanOrEqual<T>(string key, T value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.LessThanOrEqual,
				Value = new QueryValue<T>(value)   
			}
		};
	}
	
	/// <summary>
	/// LessThanOrEqual ($lte)
	/// </summary>
	/// <param name="value">Operand Value</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery LessThanOrEqual<T>(T value)
	{
		return new Query
		{
			Operator = MongoOperator.LessThanOrEqual,
			Value = new QueryValue<T>(value)   
		};
	}
	
	/// <summary>
	/// In ($in)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression In<T>(string key, IEnumerable<T> values)
	{
		return Contains(key, values);
	}
	
	/// <summary>
	/// Contains ($in)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression Contains<T>(string key, IEnumerable<T> values)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.Contains,
				Value = new QueryArray(values.Select(x => new QueryValue<T>(x)))   
			}
		};
	}
	
	/// <summary>
	/// In ($in)
	/// </summary>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery In<T>(IEnumerable<T> values)
	{
		return Contains(values);
	}
	
	/// <summary>
	/// Contains ($in)
	/// </summary>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery Contains<T>(IEnumerable<T> values)
	{
		return new Query
		{
			Operator = MongoOperator.Contains,
			Value = new QueryArray(values.Select(x => new QueryValue<T>(x)))   
		};
	}
	
	/// <summary>
	/// Nin ($nin)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression Nin<T>(string key, IEnumerable<T> values)
	{
		return NotContains(key, values);
	}
	
	/// <summary>
	/// NotContains ($nin)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQueryExpression NotContains<T>(string key, IEnumerable<T> values)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.NotContains,
				Value = new QueryArray(values.Select(x => new QueryValue<T>(x)))   
			}
		};
	}
	
	/// <summary>
	/// Nin ($nin)
	/// </summary>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery Nin<T>(IEnumerable<T> values)
	{
		return NotContains(values);
	}
	
	/// <summary>
	/// NotContains ($nin)
	/// </summary>
	/// <param name="values">Operand Values</param>
	/// <typeparam name="T">Value Type</typeparam>
	public static IQuery NotContains<T>(IEnumerable<T> values)
	{
		return new Query
		{
			Operator = MongoOperator.NotContains,
			Value = new QueryArray(values.Select(x => new QueryValue<T>(x)))   
		};
	}
	
	/// <summary>
	/// Between ($gte and $lte): the values in the range; a bound is excluded ($gt, $lt) when it is not included
	/// (e.g. a date range including the start and excluding the end)
	/// </summary>
	public static IQueryExpression Between<T>(string key, T from, T to, bool includeFrom = true, bool includeTo = true)
	{
		return new QueryExpression
		{
			Field = key,
			Value = Between(from, to, includeFrom, includeTo)
		};
	}
	
	/// <summary>
	/// Between ($gte and $lte), without a field
	/// </summary>
	public static IQuery Between<T>(T from, T to, bool includeFrom = true, bool includeTo = true)
	{
		return CombineCore(
		[
			includeFrom ? GreaterThanOrEqual(from) : GreaterThan(from),
			includeTo ? LessThanOrEqual(to) : LessThan(to)
		]);
	}
	
	#endregion
	
	#region Logical Queries
	
	/// <summary>
	/// And ($and)
	/// </summary>
	/// <param name="expressions">Expressions</param>
	public static IQuery And(IEnumerable<IQuery> expressions)
	{
		return new QueryArray(expressions)
		{
			Operator = MongoOperator.And
		};
	}
	
	/// <summary>
	/// And ($and)
	/// </summary>
	/// <param name="expressions">Expressions</param>
	public static IQuery And(params IQuery[] expressions)
	{
		return new QueryArray(expressions)
		{
			Operator = MongoOperator.And
		};
	}
	
	/// <summary>
	/// Or ($or)
	/// </summary>
	/// <param name="expressions">Expressions</param>
	public static IQuery Or(IEnumerable<IQuery> expressions)
	{
		return new QueryArray(expressions)
		{
			Operator = MongoOperator.Or
		};
	}
	
	/// <summary>
	/// Or ($or)
	/// </summary>
	/// <param name="expressions">Expressions</param>
	public static IQuery Or(params IQuery[] expressions)
	{
		return new QueryArray(expressions)
		{
			Operator = MongoOperator.Or
		};
	}
	
	/// <summary>
	/// Nor ($nor)
	/// </summary>
	/// <param name="expressions">Expressions</param>
	public static IQuery Nor(IEnumerable<IQuery> expressions)
	{
		return new QueryArray(expressions)
		{
			Operator = MongoOperator.Nor
		};
	}
	
	/// <summary>
	/// Nor ($nor)
	/// </summary>
	/// <param name="expressions">Expressions</param>
	public static IQuery Nor(params IQuery[] expressions)
	{
		return new QueryArray(expressions)
		{
			Operator = MongoOperator.Nor
		};
	}
	
	/// <summary>
	/// Not ($not)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Value</param>
	public static IQueryExpression Not<T>(string key, T value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.Not,
				Value = new Query
				{
					Operator = MongoOperator.Equals,
					Value = new QueryValue<T>(value)
				}
			}
		};
	}
	
	/// <summary>
	/// Not ($not)
	/// </summary>
	/// <param name="value">Value</param>
	public static IQuery Not<T>(T value)
	{
		return new Query
		{
			Operator = MongoOperator.Not,
			Value = new Query
			{
				Operator = MongoOperator.Equals,
				Value = new QueryValue<T>(value)
			}
		};
	}
	
	/// <summary>
	/// Not ($not)
	/// </summary>
	/// <param name="expression">Operator Expression</param>
	public static IQueryExpression Not(IQueryExpression expression)
	{
		if (expression is QueryExpression queryExpression)
		{
			return new QueryExpression
			{
				Field = expression.Field,
				Value = new Query
				{
					Operator = MongoOperator.Not,
					Value = queryExpression.Value
				}
			};
		}
		else
		{
			return new QueryExpression
			{
				Field = expression.Field,
				Value = new Query
				{
					Operator = MongoOperator.Not,
					Value = expression
				}
			};
		}
	}
	
	#endregion
	
	#region Element Queries
	
	/// <summary>
	/// Exists ($exists)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="value">Operand Value</param>
	public static IQueryExpression Exists(string key, bool value)
	{
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.Exists,
				Value = new QueryValue<bool>(value)
			}
		};
	}
	
	/// <summary>
	/// Exists ($exists)
	/// </summary>
	/// <param name="value">Operand Value</param>
	public static IQuery Exists(bool value)
	{
		return new Query
		{
			Operator = MongoOperator.Exists,
			Value = new QueryValue<bool>(value)
		};
	}
	
	/// <summary>
	/// TypeOf ($type)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="type">Bson Type</param>
	public static IQueryExpression TypeOf(string key, BsonType type)
	{
		var bsonTypeName = GetTypeAlias(type);
		
		return new QueryExpression
		{
			Field = key,
			Value = new Query
			{
				Operator = MongoOperator.TypeOf,
				Value = new QueryValue<string>(bsonTypeName)
			}
		};
	}
	
	/// <summary>
	/// TypeOf ($type)
	/// </summary>
	/// <param name="type">Bson Type</param>
	public static IQuery TypeOf(BsonType type)
	{
		var bsonTypeName = GetTypeAlias(type);
		
		return new Query
		{
			Operator = MongoOperator.TypeOf,
			Value = new QueryValue<string>(bsonTypeName)
		};
	}
	
	#endregion
	
	#region Evaluation Queries
	
	/// <summary>
	/// Regex ($regex)
	/// </summary>
	/// <param name="key">Field Name</param>
	/// <param name="regex">Regular Expression</param>
	/// <param name="options">Options</param>
	public static IQueryExpression Regex(string key, string regex, RegexOptions? options = null)
	{
		return new QueryExpression
		{
			Field = key,
			Value = RegexCore(TrimRegexDelimiters(regex), options)
		};
	}
	
	/// <summary>
	/// Text ($text)
	/// </summary>
	/// <param name="keyword">Search Keyword</param>
	/// <param name="language">The language that determines the list of stop words for the search and the rules for the stemmer and tokenizer.</param>
	/// <param name="isCaseSensitive">A boolean flag to enable or disable case sensitive search.</param>
	/// <param name="isDiacriticSensitive">A boolean flag to enable or disable diacritic sensitive search against version 3 text indexes.</param>
	public static IQuery FullTextSearch(string keyword, string language = "none", bool isCaseSensitive = false, bool isDiacriticSensitive = false)
	{
		var query = new Query
		{
			Operator = MongoOperator.Text,
			Value = new Query
			{
				Operator = MongoOperator.TextSearch,
				Value = new QueryValue<string>(keyword)
			}
		};
		
		if (!string.IsNullOrEmpty(language) && language != "none")
		{
			query.AddQuery(new Query
			{
				Operator = MongoOperator.TextSearchLanguage,
				Value = new QueryValue<string>(language)
			});
		}
		
		if (isCaseSensitive)
		{
			query.AddQuery(new Query
			{
				Operator = MongoOperator.TextSearchCaseSensitive,
				Value = new QueryValue<bool>(true)
			});
		}
		
		if (isDiacriticSensitive)
		{
			query.AddQuery(new Query
			{
				Operator = MongoOperator.TextSearchDiacriticSensitive,
				Value = new QueryValue<bool>(true)
			});
		}
		
		return query;
	}
	
	/// <summary>
	/// Mod ($mod): the number field divided by the divisor has the remainder
	/// </summary>
	public static IQueryExpression Mod(string key, long divisor, long remainder)
	{
		return new QueryExpression
		{
			Field = key,
			Value = Mod(divisor, remainder)
		};
	}
	
	/// <summary>
	/// Mod ($mod), without a field
	/// </summary>
	public static IQuery Mod(long divisor, long remainder)
	{
		if (divisor == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(divisor), "The divisor of $mod can not be zero");
		}
		
		return new Query
		{
			Operator = MongoOperator.Mod,
			Value = new QueryArray([new QueryValue<long>(divisor), new QueryValue<long>(remainder)])
		};
	}
	
	#endregion
	
	#region Text Matching Queries
	
	/// <summary>
	/// The values starting with the text ($regex "^text"); the text is matched as it is, not as a pattern
	/// </summary>
	public static IQueryExpression StartsWith(string key, string text, bool ignoreCase = false)
	{
		return Regex(key, "^" + EscapeRegex(text), GetTextMatchingOptions(ignoreCase));
	}
	
	/// <summary>
	/// The values starting with the text ($regex "^text"), without a field (e.g. for the items of an array in ElemMatch)
	/// </summary>
	public static IQuery StartsWith(string text, bool ignoreCase = false)
	{
		return RegexCore("^" + EscapeRegex(text), GetTextMatchingOptions(ignoreCase));
	}
	
	/// <summary>
	/// The values ending with the text ($regex "text$"); the text is matched as it is, not as a pattern
	/// </summary>
	public static IQueryExpression EndsWith(string key, string text, bool ignoreCase = false)
	{
		return Regex(key, EscapeRegex(text) + "$", GetTextMatchingOptions(ignoreCase));
	}
	
	/// <summary>
	/// The values ending with the text ($regex "text$"), without a field
	/// </summary>
	public static IQuery EndsWith(string text, bool ignoreCase = false)
	{
		return RegexCore(EscapeRegex(text) + "$", GetTextMatchingOptions(ignoreCase));
	}
	
	/// <summary>
	/// The values containing the text ($regex "text"); the text is matched as it is, not as a pattern
	/// (Contains is the $in operator)
	/// </summary>
	public static IQueryExpression ContainsText(string key, string text, bool ignoreCase = false)
	{
		return Regex(key, EscapeRegex(text), GetTextMatchingOptions(ignoreCase));
	}
	
	/// <summary>
	/// The values containing the text ($regex "text"), without a field
	/// </summary>
	public static IQuery ContainsText(string text, bool ignoreCase = false)
	{
		return RegexCore(EscapeRegex(text), GetTextMatchingOptions(ignoreCase));
	}
	
	/// <summary>
	/// Escapes the regular expression characters of the text, so it is matched as it is (e.g. a user input in a pattern).
	/// The slash is escaped too: a pattern in slashes ('/pattern/') is read without them by Regex.
	/// </summary>
	public static string EscapeRegex(string text)
	{
		var builder = new StringBuilder(text.Length);
		foreach (var character in text)
		{
			if (RegexSpecialCharacters.Contains(character))
			{
				builder.Append('\\');
			}
			
			builder.Append(character);
		}
		
		return builder.ToString();
	}
	
	private static RegexOptions? GetTextMatchingOptions(bool ignoreCase)
	{
		return ignoreCase ? RegexOptions.CaseInsensitivity : null;
	}
	
	private static IQuery RegexCore(string pattern, RegexOptions? options)
	{
		return new Query
		{
			Operator = MongoOperator.Regex,
			Value = new RegularExpression(pattern, options)
		};
	}
	
	#endregion
	
	#region Array Queries
	
	/// <summary>
	/// Matches the documents whose array field has at least one element that satisfies all the queries,
	/// e.g. ElemMatch("accounts", Equals("provider", "google"), Equals("user_id", "123")) or ElemMatch("scores", GreaterThanOrEqual(80), LessThan(85))
	/// </summary>
	public static IQueryExpression ElemMatch(string key, params IQuery[] queries)
	{
		return ElemMatch(key, (IEnumerable<IQuery>) queries);
	}
	
	/// <summary>
	/// Matches the documents whose array field has at least one element that satisfies all the queries
	/// </summary>
	public static IQueryExpression ElemMatch(string key, IEnumerable<IQuery> queries)
	{
		return new QueryExpression
		{
			Field = key,
			Value = ElemMatchCore(queries)
		};
	}
	
	/// <summary>
	/// The $elemMatch operator without a field (e.g. to combine it with other operators of a field)
	/// </summary>
	public static IQuery ElemMatch(params IQuery[] queries)
	{
		return ElemMatchCore(queries);
	}
	
	/// <summary>
	/// The $elemMatch operator without a field (e.g. to combine it with other operators of a field)
	/// </summary>
	public static IQuery ElemMatch(IEnumerable<IQuery> queries)
	{
		return ElemMatchCore(queries);
	}
	
	private static IQuery ElemMatchCore(IEnumerable<IQuery> queries)
	{
		return new Query
		{
			Operator = MongoOperator.ElemMatch,
			Value = CombineCore(queries)
		};
	}
	
	/// <summary>
	/// All ($all): the array field contains all the values
	/// </summary>
	public static IQueryExpression All<T>(string key, IEnumerable<T> values)
	{
		return new QueryExpression
		{
			Field = key,
			Value = All(values)
		};
	}
	
	/// <summary>
	/// All ($all), without a field
	/// </summary>
	public static IQuery All<T>(IEnumerable<T> values)
	{
		return new Query
		{
			Operator = MongoOperator.All,
			Value = new QueryArray(values.Select(x => new QueryValue<T>(x)))
		};
	}
	
	/// <summary>
	/// Size ($size): the array field has exactly this number of items
	/// </summary>
	public static IQueryExpression Size(string key, int size)
	{
		return new QueryExpression
		{
			Field = key,
			Value = Size(size)
		};
	}
	
	/// <summary>
	/// Size ($size), without a field
	/// </summary>
	public static IQuery Size(int size)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(size);
		return new Query
		{
			Operator = MongoOperator.Size,
			Value = new QueryValue<int>(size)
		};
	}
	
	#endregion
	
	#region Composition Methods
	
	/// <summary>
	/// The queries written into one object must have distinct keys (a repeated key is rejected by MongoDB or drops a condition):
	/// the operators of a repeated field are merged ({ "age": { "$gt": 18, "$lt": 65 } }); the other repeated keys
	/// (e.g. two values of a field, two $or) are combined with $and. The operators without a field are kept as they are.
	/// </summary>
	private static List<IQuery> Compose(IEnumerable<IQuery> queries)
	{
		var composed = MergeFieldOperators(queries.ToList());
		var keys = composed.SelectMany(GetKeys).ToList();
		if (keys.Count == keys.Distinct().Count() || composed.All(x => x is Query))
		{
			return composed;
		}
		
		return [new QueryArray(composed) { Operator = MongoOperator.And }];
	}
	
	/// <summary>
	/// Merges the expressions of a repeated field into one expression, when each of them is a single operator and the operators are distinct
	/// </summary>
	private static List<IQuery> MergeFieldOperators(List<IQuery> queries)
	{
		var composed = new List<IQuery>(queries);
		var repeatedFields = queries.OfType<QueryExpression>().GroupBy(x => x.Field).Where(x => x.Count() > 1);
		foreach (var fieldExpressions in repeatedFields)
		{
			var operators = fieldExpressions.Select(GetSingleOperator).ToList();
			if (operators.Any(x => x == null) || operators.Select(x => x!.Operator).Distinct().Count() != operators.Count)
			{
				continue;
			}
			
			var merged = new QueryExpression
			{
				Field = fieldExpressions.Key,
				Value = new CustomQuery { Children = operators.Cast<IQuery>().ToList() }
			};
			
			// The merged expression takes the place of the first one
			composed[composed.IndexOf(fieldExpressions.First())] = merged;
			foreach (var expression in fieldExpressions.Skip(1))
			{
				composed.Remove(expression);
			}
		}
		
		return composed;
	}
	
	/// <summary>
	/// The operator of an expression like { "age": { "$gt": 18 } }; null when the expression is not a single operator (e.g. a value, a regex with options)
	/// </summary>
	private static Query? GetSingleOperator(QueryExpression expression)
	{
		return expression.Children.Count == 1 && expression.Value is Query { Operator: not null, Children.Count: 1 } query ? query : null;
	}
	
	/// <summary>
	/// The keys the query writes into the object it is combined in
	/// </summary>
	private static IEnumerable<string> GetKeys(IQuery query)
	{
		return query switch
		{
			QueryExpression expression => [expression.Field],
			Query { Operator: not null } operatorQuery => ["$" + QueryHelper.GetOperatorTag(operatorQuery.Operator.Value)],
			QueryArray { IsFlattened: true } andQuery => andQuery.Cast<IQueryExpression>().Select(x => x.Field),
			QueryArray { Operator: not null } arrayQuery => ["$" + QueryHelper.GetOperatorTag(arrayQuery.Operator.Value)],
			CustomQuery { ShowOperatorTag: true, Operator: not null } customQuery => [customQuery.Operator],
			CustomQuery customQuery => customQuery.Children.SelectMany(GetKeys),
			_ => []
		};
	}
	
	#endregion
	
	#region Helper Methods
	
	/// <summary>
	/// The MongoDB alias of the type (e.g. 'objectId', 'timestamp')
	/// </summary>
	private static string GetTypeAlias(BsonType type)
	{
		if (type == BsonType.TimeStamp)
		{
			return "timestamp";
		}
		
		var typeName = type.ToString();
		return char.ToLowerInvariant(typeName[0]) + typeName[1..];
	}
	
	/// <summary>
	/// Removes the delimiters of a '/pattern/' form (the slashes of the pattern itself are kept)
	/// </summary>
	private static string TrimRegexDelimiters(string regex)
	{
		return regex is ['/', _, ..] && regex[^1] == '/' ? regex[1..^1] : regex;
	}
	
	#endregion
}