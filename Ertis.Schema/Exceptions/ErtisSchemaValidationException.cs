using System;

namespace Ertis.Schema.Exceptions
{
    public class ErtisSchemaValidationException : Exception
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public ErtisSchemaValidationException(string message) : base(message)
        {
            
        }

        #endregion
    }
}