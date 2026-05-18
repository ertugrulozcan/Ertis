using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Validation;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Schema.Types;

public interface ISchema
{
	#region Properties
	
	[JsonPropertyName("slug")]
	string Slug { get; }
	
	[JsonPropertyName("properties")]
	IReadOnlyCollection<IFieldInfo> Properties { get; }
	
	[JsonPropertyName("allowAdditionalProperties")]
	bool AllowAdditionalProperties { get; }
	
	#endregion
	
	#region Methods
	
	bool ValidateSchema(out Exception? exception);
	
	bool ValidateContent(DynamicObject obj, IValidationContext validationContext);
	
	#endregion
}