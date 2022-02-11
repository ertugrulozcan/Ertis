using System;

namespace Ertis.Schema.Exceptions
{
    public abstract class ErtisSchemaValidationException : Exception
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        protected ErtisSchemaValidationException(string message) : base(message)
        {
            
        }

        #endregion
    }
}