using System;

namespace Ertis.MongoDB.Queries
{
    public class QueryValue<T> : IQuery
    {
        #region Properties

        protected T Value { get; }

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
            else if (this.Value == null)
            {
                return "null";
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
            else if (IsNumericType(typeof(T)))
            {
                return this.Value.ToString()?.Replace(',', '.');
            }
            else
            {
                return this.Value.ToString();
            }
        }

        private static bool IsNumericType(Type type)
        {
            return
                IsIntegralNumericType(type) ||
                IsFloatingPointNumericType(type);
        }
        
        private static bool IsIntegralNumericType(Type type)
        {
            return
                type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(nint) ||
                type == typeof(nuint) ||
                type == typeof(long) ||
                type == typeof(ulong);
        }
        
        private static bool IsFloatingPointNumericType(Type type)
        {
            return
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal);
        }
        
        #endregion
    }
}