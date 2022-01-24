namespace Ertis.MongoDB.Queries
{
    internal class QueryValue<T> : IQuery
    {
        #region Properties

        private T Value { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public QueryValue(T value)
        {
            this.Value = value;
        }

        #endregion
        
        #region Methods
        
        public string ToQuery(bool addFieldName = true, bool simplifyEqualsQueries = true)
        {
            if (this.Value is IQuery query)
            {
                return query.ToQuery(simplifyEqualsQueries);
            }
            else if (typeof(T) == typeof(string))
            {
                return "\"" + this.Value + "\"";
            }
            else if (typeof(T) == typeof(bool))
            {
                return this.Value.ToString()?.ToLower();
            }
            else
            {
                return this.Value.ToString();
            }
        }

        public override string ToString()
        {
            return this.ToQuery();
        }

        #endregion
    }
}