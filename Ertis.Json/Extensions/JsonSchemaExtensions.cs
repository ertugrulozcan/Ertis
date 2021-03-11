using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Schema;

namespace Ertis.Json.Extensions
{
	public static class JsonSchemaExtensions
	{
		#region Methods

		public static bool? IsValidDefaults(this JSchema schema, out IList<ValidationError> validationErrors)
		{
			var cumulativeValidationErrors = new List<ValidationError>();
			foreach (var (_, jSchema) in schema.Properties)
			{
				IList<ValidationError> validationErrorsForProperty = null;
				var isValidProperty = jSchema?.Default?.IsValid(jSchema, out validationErrorsForProperty);
				if (isValidProperty != null && !isValidProperty.Value)
				{
					cumulativeValidationErrors.AddRange(validationErrorsForProperty);
				}
			}

			IList<ValidationError> validationErrorsForSchema = null;
			var isValidSchema = schema.Default?.IsValid(schema, out validationErrorsForSchema);
			if (isValidSchema != null && !isValidSchema.Value)
			{
				cumulativeValidationErrors.AddRange(validationErrorsForSchema);
			}

			validationErrors = cumulativeValidationErrors;
			return !cumulativeValidationErrors.Any();
		}

		public static bool IsContainsAdditionalProperties(this JSchema schema, out IList<string> additionalPropertyErrors)
		{
			var additionalPropertyErrors_ = new List<string>();
			
			var isContains = schema.ExtensionData.Any();
			if (isContains)
			{
				additionalPropertyErrors_.AddRange(schema.ExtensionData.Select(x => $"Unexpected token [{x.Value.GetPathFromAnnotation()}]"));
				additionalPropertyErrors = additionalPropertyErrors_;
				return true;
			}

			foreach (var (_, jSchema) in schema.Properties)
			{
				isContains |= IsContainsAdditionalProperties(jSchema, out var additionalPropertyErrors__);
				if (isContains)
				{
					additionalPropertyErrors_.AddRange(additionalPropertyErrors__);
					additionalPropertyErrors = additionalPropertyErrors_;
					return true;
				}	
			}

			additionalPropertyErrors = additionalPropertyErrors_;
			return false;
		}
		
		#endregion
	}
}