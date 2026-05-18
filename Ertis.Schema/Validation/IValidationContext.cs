using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;

// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Schema.Validation;

public interface IValidationContext
{
	DynamicObject? Content { get; }
	
	IList<FieldValidationException> Errors { get; }
}