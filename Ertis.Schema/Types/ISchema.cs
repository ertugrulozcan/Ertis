using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics.Legacy;
using Ertis.Schema.Serialization;
using Ertis.Schema.Validation;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Schema.Types;

public interface ISchema
{
	#region Properties
	
	[JsonPropertyName("slug")]
	[Newtonsoft.Json.JsonProperty("slug")]
	string Slug { get; }
	
	[JsonPropertyName("properties")]
	[Newtonsoft.Json.JsonProperty("properties")]
	[Newtonsoft.Json.JsonConverter(typeof(FieldInfoCollectionJsonConverter))]
	IReadOnlyCollection<IFieldInfo> Properties { get; }
	
	[JsonPropertyName("allowAdditionalProperties")]
	[Newtonsoft.Json.JsonProperty("allowAdditionalProperties")]
	bool AllowAdditionalProperties { get; }
	
	#endregion
	
	#region Methods
	
	bool ValidateSchema(out Exception? exception);
	
	bool ValidateContent(DynamicObject obj, IValidationContext validationContext);
	
	#endregion
}