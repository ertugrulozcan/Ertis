using System.Collections.Generic;
using System.Linq;

namespace Ertis.MongoDB.Queries
{
    internal class QueryArray : List<IQuery>, IQuery
    {
        #region Properties

        public MongoOperator? Operator { get; init; }

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

        public string ToQuery(bool addFieldName = true, bool simplifyEqualsQueries = true)
        {
            if (this.Operator != null)
            {
                var operatorTag = OperatorHelper.GetTag(this.Operator.Value);
                return "{ $" + operatorTag + ": [ " + string.Join(", ", this.Select(x => x.ToQuery(simplifyEqualsQueries))) + " ] }";
            }
            else
            {
                return "[ " + string.Join(", ", this.Select(x => x.ToQuery(simplifyEqualsQueries))) + " ]";   
            }
        }
        
        public override string ToString()
        {
            return this.ToQuery();
        }

        #endregion
    }
}