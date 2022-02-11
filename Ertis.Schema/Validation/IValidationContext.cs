using System.Collections.Generic;
using Ertis.Schema.Exceptions;

namespace Ertis.Schema.Validation
{
    public interface IValidationContext
    {
        IList<FieldValidationException> Errors { get; }
    }
}