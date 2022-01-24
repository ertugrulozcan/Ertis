using System;

namespace Ertis.MongoDB.Queries
{
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
                MongoOperator.Exist => "exists",
                MongoOperator.TypeOf => "type",
                MongoOperator.Regex => "regex",
                MongoOperator.Text => "text",
                MongoOperator.RegexOptions => "options",
                MongoOperator.TextSearch => "search",
                MongoOperator.TextSearchLanguage => "language",
                MongoOperator.TextSearchCaseSensitive => "caseSensitive",
                MongoOperator.TextSearchDiacriticSensitive => "diacriticSensitive",
                _ => throw new ArgumentOutOfRangeException(nameof(mongoOperator), mongoOperator, null)
            };
        }

        internal static string ConvertRegexOptions(RegexOptions? options)
        {
            string regexOptions = null;
            
            if (options != null)
            {
                var flagValue = (int) options.Value;
                regexOptions = flagValue switch
                {
                    1 => "i",
                    2 => "m",
                    4 => "x",
                    8 => "s",
                    3 => "mi",
                    5 => "xi",
                    9 => "si",
                    6 => "xm",
                    10 => "sm",
                    12 => "sx",
                    _ => null
                };
            }

            return regexOptions;
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
}