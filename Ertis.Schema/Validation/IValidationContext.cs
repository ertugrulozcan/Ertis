using System.Collections.Generic;
using Ertis.Schema.Dynamics.Legacy;
using Ertis.Schema.Exceptions;

namespace Ertis.Schema.Validation
{
    public interface IValidationContext
    {
        DynamicObject Content { get; }
        
        IList<FieldValidationException> Errors { get; }
    }
}