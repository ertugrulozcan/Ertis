using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Serialization;
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
	[JsonConverter(typeof(FieldInfoCollectionJsonConverterFactory))]
	IReadOnlyCollection<IFieldInfo> Properties { get; }
	
	[JsonPropertyName("allowAdditionalProperties")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	bool AllowAdditionalProperties { get; }
	
	#endregion
	
	#region Methods
	
	bool ValidateSchema(out Exception? exception);
	
	bool ValidateContent(DynamicObject obj, IValidationContext validationContext);
	
	#endregion
}