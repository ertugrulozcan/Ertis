using System.Collections.Generic;
using System.Linq;

namespace Ertis.MongoDB.Queries
{
	public static class QueryBuilder
	{
		#region Collection Queries

		public static IQuery Where(IQueryable value)
		{
			return new Query("where", value);
		}
		
		public static IQuery Where(string key, IQueryable value)
		{
			return new Query("where", new QueryGroup
			{
				{ key, value }
			});
		}
		
		public static IQuery Where(string key, object value)
		{
			return new Query("where", new QueryGroup
			{
				{ key, new QueryValue(value) }
			});
		}

		#endregion

		#region Projection Queires

		public static IQuery Select(IDictionary<string, bool> selections)
		{
			return new Query("select", new QueryGroup(selections.ToDictionary(x => x.Key, y => new QueryValue(y.Value) as IQueryable)));
		}

		#endregion
		
		#region Comparison Queires

		public static IQuery Equals(string key, object value)
		{
			return new Query(key, new QueryGroup
			{
				{ "$eq", new QueryValue(value) }
			});
		}
		
		public static IQuery NotEquals(string key, object value)
		{
			return new Query(key, new QueryGroup
			{
				{ "$ne", new QueryValue(value) }
			});
		}
		
		public static IQuery GreaterThan(string key, object value)
		{
			return new Query(key, new QueryGroup
			{
				{ "$gt", new QueryValue(value) }
			});
		}
		
		public static IQuery GreaterThanOrEqual(string key, object value)
		{
			return new Query(key, new QueryGroup
			{
				{ "$gte", new QueryValue(value) }
			});
		}
		
		public static IQuery LessThan(string key, object value)
		{
			return new Query(key, new QueryGroup
			{
				{ "$lt", new QueryValue(value) }
			});
		}
		
		public static IQuery LessThanOrEqual(string key, object value)
		{
			return new Query(key, new QueryGroup
			{
				{ "$lte", new QueryValue(value) }
			});
		}
		
		public static IQuery Contains(string key, QueryArray queryArray)
		{
			return new Query(key, new Query("$in", queryArray));
		}
		
		public static IQuery NotContains(string key, QueryArray queryArray)
		{
			return new Query(key, new Query("$nin", queryArray));
		}

		#endregion
		
		#region Logical Queries
		
		public static IQuery And(QueryArray queryArray)
		{
			return new Query("$and", queryArray);
		}
		
		public static IQuery Or(QueryArray queryArray)
		{
			return new Query("$or", queryArray);
		}
		
		public static IQuery Nor(QueryArray queryArray)
		{
			return new Query("$nor", queryArray);
		}
		
		public static IQuery Not(IQuery query)
		{
			return new Query(query.Key, new QueryGroup
			{
				{ "$not", query.Value }
			});
		}
		
		#endregion
		
		#region Regular Expression Queries
		
		public static IQuery Regex(string key, string expression, RegexOptions options = null)
		{
			if (options == null)
			{
				return new Query(key, new QueryGroup
				{
					{ "$regex", new QueryValue(expression) }
				});
			}
			else
			{
				return new Query(key, new QueryGroup
				{
					{ "$regex", new QueryValue(expression) },
					{ "$options", new QueryValue(options.ToString()) }
				});	
			}
		}
		
		#endregion
		
		#region Other Queries
		
		public static IQuery Search(string keyword, string language = null, bool? isCaseSensitive = null, bool? isDiacriticSensitive = null)
		{
			var queryGroup = new QueryGroup
			{
				{ "$search", new QueryValue(keyword) }
			};

			if (!string.IsNullOrEmpty(language))
			{
				queryGroup.Add("$language", language);
			}

			if (isCaseSensitive != null)
			{
				queryGroup.Add("$caseSensitive", isCaseSensitive.Value);
			}

			if (isDiacriticSensitive != null)
			{
				queryGroup.Add("$diacriticSensitive", isDiacriticSensitive.Value);
			}
			
			return new Query("$text", queryGroup);
		}
		
		public static IQuery TypeOf(string key, object value)
		{
			return new Query(key, new QueryGroup
			{
				{ "$type", new QueryValue(value) }
			});
		}
		
		#endregion
		
		#region Merge Queries

		public static IQueryable Combine(params IQuery[] queries)
		{
			var queryGroup = new QueryGroup();
			if (queries != null)
			{
				foreach (var query in queries)
				{
					queryGroup.Add(query);
				}
			}

			return queryGroup;
		}

		#endregion
	}
}