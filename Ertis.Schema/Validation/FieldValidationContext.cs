using System.Collections.Generic;
using Ertis.Schema.Exceptions;

namespace Ertis.Schema.Validation
{
    public class FieldValidationContext : IValidationContext
    {
        #region Properties

        public IList<FieldValidationException> Errors { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public FieldValidationContext()
        {
            this.Errors = new List<FieldValidationException>();
        }

        #endregion
    }
}