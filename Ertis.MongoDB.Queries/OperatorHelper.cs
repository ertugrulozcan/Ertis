using System;

namespace Ertis.MongoDB.Queries
{
    internal static class OperatorHelper
    {
        #region Methods

        internal static string GetTag(MongoOperator mongoOperator)
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
                _ => throw new ArgumentOutOfRangeException(nameof(mongoOperator), mongoOperator, null)
            };
        }

        #endregion
    }
}