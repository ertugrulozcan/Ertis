using System.Collections.Generic;
using System.Linq;

namespace Ertis.MongoDB.Queries
{
    internal class QueryArray : List<IQuery>, IQuery
    {
        #region Properties

        internal MongoOperator? Operator { get; init; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="queries"></param>
        public QueryArray(IEnumerable<IQuery> queries)
        {
            this.AddRange(queries);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (this.Operator != null)
            {
                // Simplify $and operators
                if (this.Operator == MongoOperator.And && this.All(x => x is IQueryExpression))
                {
                    return "{ " + string.Join(", ", this.Select(x => x.ToString()?.Trim().Trim('{').Trim('}').Trim())) + " }";
                }
                
                var operatorTag = QueryHelper.GetOperatorTag(this.Operator.Value);
                return "{ $" + operatorTag + ": [ " + string.Join(", ", this.Select(x => x.ToString())) + " ] }";
            }
            else
            {
                return "[ " + string.Join(", ", this.Select(x => x.ToString())) + " ]";   
            }
        }

        #endregion
    }
}