using System.Collections.Generic;

namespace Ertis.MongoDB.Queries
{
    internal class Query : IQuery, IHasChildren
    {
        #region Fields

        private readonly IQuery _query;

        #endregion
        
        #region Properties

        internal MongoOperator? Operator { get; init; }
        
        internal IQuery Value 
        {
            get => this._query;
            init
            {
                this._query = value;
                this.Children.Insert(0, value);
            }
        }

        public List<IQuery> Children { get; } = new();

        #endregion

        #region Methods

        public void AddQuery(IQuery query)
        {
            this.Children.Add(query);
        }

        public override string ToString()
        {
            if (this.Children.Count == 1)
            {
                if (this.Operator != null)
                {
                    var operatorTag = QueryHelper.GetOperatorTag(this.Operator.Value);
                    var expressionJson = this.Value.ToString();
                    return "{ $" + operatorTag + ": " + expressionJson + " }";
                }
                else
                {
                    return this.Value.ToString();
                }       
            }
            else
            {
                var expressionJsons = new List<string>();
                foreach (var query in this.Children)
                {
                    var expressionJson = QueryHelper.GetInnerQuery(query);
                    if (!string.IsNullOrEmpty(expressionJson))
                    {
                        expressionJsons.Add(expressionJson);
                    }
                }
                
                if (this.Operator != null)
                {
                    var operatorTag = QueryHelper.GetOperatorTag(this.Operator.Value);
                    return "{ $" + operatorTag + ": { " + string.Join(", ", expressionJsons) + " } }";
                }
                else
                {
                    return "{ " + string.Join(", ", expressionJsons) + " }";
                }
            }
        }

        #endregion
    }
}