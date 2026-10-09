using Ertis.Schema.Dynamics;
using Ertis.Schema.Extensions;
using Ertis.Schema.Types;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Tests.TestHelpers;

/// <summary>
/// A minimal ISchema implementation, shaped like the schema types of the consumers (e.g. ErtisAuth UserType)
/// </summary>
public class TestSchema : ISchema
{
	#region Properties
	
	public string Slug { get; init; } = "test-schema";
	
	public required IReadOnlyCollection<IFieldInfo> Properties { get; init; }
	
	public bool AllowAdditionalProperties { get; init; }
	
	#endregion
	
	#region Methods
	
	public static TestSchema Of(params IFieldInfo[] properties)
	{
		return new TestSchema { Properties = properties };
	}
	
	public bool ValidateSchema(out Exception? exception)
	{
		this.Validate(out exception);
		return exception == null;
	}
	
	public bool ValidateContent(DynamicObject obj, IValidationContext validationContext)
	{
		return this.ValidateData(obj, validationContext);
	}
	
	#endregion
}
