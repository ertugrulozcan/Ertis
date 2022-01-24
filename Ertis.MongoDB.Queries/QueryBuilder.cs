using System.Collections.Generic;
using System.Linq;

namespace Ertis.MongoDB.Queries
{
    public static class QueryBuilder
    {
        #region Where Methods

        public static IQuery Where<T>(string key, T value)
        {
            return WhereCore(new []
            {
                Equals(key, value)
            });
        }
        
        public static IQuery WhereOut<T>(string key, T value)
        {
            return WhereCore(new []
            {
                Equals(key, value)
            }, true);
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
                Children = queries.ToList(),
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
                Children = queries.ToList()
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
        /// Exist ($exist)
        /// </summary>
        /// <param name="key">Field Name</param>
        /// <param name="value">Operand Value</param>
        public static IQueryExpression Exist(string key, bool value)
        {
            return new QueryExpression
            {
                Field = key,
                Value = new Query
                {
                    Operator = MongoOperator.Exist,
                    Value = new QueryValue<bool>(value)
                }
            };
        }
        
        /// <summary>
        /// TypeOf ($type)
        /// </summary>
        /// <param name="key">Field Name</param>
        /// <param name="type">Bson Type</param>
        public static IQueryExpression TypeOf(string key, BsonType type)
        {
            var bsonTypeName = type.ToString();
            bsonTypeName = char.ToLower(bsonTypeName[0]) + bsonTypeName.Substring(1);
            
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
            var queryExpression = new QueryExpression
            {
                Field = key,
                Value = new Query
                {
                    Operator = MongoOperator.Regex,
                    Value = new QueryValue<string>(regex.TrimStart('/').TrimEnd('/'))
                }
            };
            
            var regexOptions = QueryHelper.ConvertRegexOptions(options);
            if (!string.IsNullOrEmpty(regexOptions))
            {
                queryExpression.AddQuery(new Query
                {
                    Operator = MongoOperator.RegexOptions,
                    Value = new QueryValue<string>(regexOptions)
                });
            }

            return queryExpression;
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

        #endregion
    }
}