using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Tests.TestHelpers;

public sealed record ValidationResult(bool IsValid, IList<FieldValidationException> Errors, DynamicObject Content)
{
	public IEnumerable<string> Messages => this.Errors.Select(x => x.Message);
}

public static class SchemaValidation
{
	#region Methods
	
	/// <summary>
	/// Validates the json content against the schema the way the consumers do (DynamicObject.Parse + ValidateContent)
	/// </summary>
	public static ValidationResult Validate(ISchema schema, string json)
	{
		return Validate(schema, DynamicObject.Parse(json));
	}
	
	public static ValidationResult Validate(ISchema schema, DynamicObject content)
	{
		var validationContext = new FieldValidationContext(content);
		var isValid = schema.ValidateContent(content, validationContext);
		return new ValidationResult(isValid, validationContext.Errors, content);
	}
	
	public static ValidationResult ValidateField(IFieldInfo fieldInfo, string json)
	{
		return Validate(TestSchema.Of(fieldInfo), json);
	}
	
	#endregion
}
