using System.Collections.Generic;
using Ertis.Schema.Dynamics.Legacy;
using Ertis.Schema.Exceptions;

namespace Ertis.Schema.Validation
{
    public class FieldValidationContext : IValidationContext
    {
        #region Properties

        public DynamicObject Content { get; }
        
        public IList<FieldValidationException> Errors { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content"></param>
        public FieldValidationContext(DynamicObject content)
        {
            this.Content = content;
            this.Errors = new List<FieldValidationException>();
        }

        #endregion
    }
}