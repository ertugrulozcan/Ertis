namespace Ertis.MongoDB.Queries
{
    public interface IQueryExpression : IQuery
    {
        string Field { get; }
    }
    
    public class QueryExpression : IQueryExpression
    {
        #region Properties

        public string Field { get; init; }
        
        internal MongoOperator Operator { get; init; }
        
        internal IQuery Value { get; init; }

        #endregion

        #region Methods

        public string ToQuery(bool addFieldName = true, bool simplifyEqualsQueries = true)
        {
            if (addFieldName)
            {
                // Simplify equals query
                if (simplifyEqualsQueries && this.Operator == MongoOperator.Equals)
                {
                    var expressionJson = this.Value.ToQuery();
                    return "{ \"" + this.Field + "\": " + expressionJson + " }";
                }
                else if (this.Operator == MongoOperator.Not)
                {
                    var operatorTag = OperatorHelper.GetTag(this.Operator);
                    var expressionJson = this.Value.ToQuery(false, false);
                    return "{ \"" + this.Field + "\": { $" + operatorTag + ": " + expressionJson + " } }";
                }
                else
                {
                    var operatorTag = OperatorHelper.GetTag(this.Operator);
                    var expressionJson = this.Value.ToQuery(simplifyEqualsQueries);
                    return "{ \"" + this.Field + "\": { $" + operatorTag + ": " + expressionJson + " } }";   
                }   
            }
            else
            {
                // Simplify equals query
                if (simplifyEqualsQueries && this.Operator == MongoOperator.Equals)
                {
                    var expressionJson = this.Value.ToQuery();
                    return expressionJson;
                }
                else if (this.Operator == MongoOperator.Not)
                {
                    var operatorTag = OperatorHelper.GetTag(this.Operator);
                    var expressionJson = this.Value.ToQuery(false, false);
                    return "{ $" + operatorTag + ": " + expressionJson + " }";
                }
                else
                {
                    var operatorTag = OperatorHelper.GetTag(this.Operator);
                    var expressionJson = this.Value.ToQuery(simplifyEqualsQueries);
                    return "{ $" + operatorTag + ": " + expressionJson + " }";
                }
            }
        }

        public override string ToString()
        {
            return this.ToQuery();
        }

        #endregion
    }
}