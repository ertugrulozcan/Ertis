using System;

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

        public override string ToString()
        {
            if (this.Value is IQuery query)
            {
                return query.ToString();
            }
            else if (typeof(T) == typeof(string) || this.Value is string)
            {
                return "\"" + this.Value + "\"";
            }
            else if (typeof(T) == typeof(bool) || this.Value is bool)
            {
                return this.Value.ToString()?.ToLower();
            }
            else if (typeof(T) == typeof(DateTime) || this.Value is DateTime)
            {
                var dateTime = this.Value is DateTime time ? time : default;
                return "\"" + dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\"";
            }
            else if (this.Value == null)
            {
                return "null";
            }
            else
            {
                return this.Value.ToString();
            }
        }

        #endregion
    }
}