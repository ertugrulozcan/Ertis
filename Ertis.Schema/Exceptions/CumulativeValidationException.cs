using System.Collections.Generic;

namespace Ertis.Schema.Exceptions
{
    public class CumulativeValidationException : ErtisSchemaValidationException
    {
        #region Properties

        public IEnumerable<FieldValidationException> Errors { get; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public CumulativeValidationException(IEnumerable<FieldValidationException> errors) : base("ValidationException")
        {
            this.Errors = errors;
        }

        #endregion
    }
}