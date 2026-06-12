// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Schema.Exceptions;

// ReSharper disable once UnusedType.Global
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