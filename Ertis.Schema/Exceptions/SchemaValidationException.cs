namespace Ertis.Schema.Exceptions
{
    public class SchemaValidationException : ErtisSchemaValidationException
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SchemaValidationException(string message) : base(message)
        {
            
        }

        #endregion
    }
}