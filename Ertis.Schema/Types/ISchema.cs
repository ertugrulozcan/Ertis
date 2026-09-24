using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Serialization;
using Ertis.Schema.Serialization.Legacy;
using Ertis.Schema.Validation;

using NewtonsoftJsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NewtonsoftJsonConverter = Newtonsoft.Json.JsonConverterAttribute;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Schema.Types;

public interface ISchema
{
	#region Properties
	
	[JsonPropertyName("slug")]
	[NewtonsoftJsonProperty("slug")]
	string Slug { get; }
	
	[JsonPropertyName("properties")]
	[JsonConverter(typeof(FieldInfoCollectionJsonConverterFactory))]
	[NewtonsoftJsonProperty("properties")]
	[NewtonsoftJsonConverter(typeof(FieldInfoCollectionJsonConverter))]
	IReadOnlyCollection<IFieldInfo> Properties { get; }
	
	[JsonPropertyName("allowAdditionalProperties")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[NewtonsoftJsonProperty("allowAdditionalProperties", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
	bool AllowAdditionalProperties { get; }
	
	#endregion
	
	#region Methods
	
	bool ValidateSchema(out Exception? exception);
	
	bool ValidateContent(DynamicObject obj, IValidationContext validationContext);
	
	#endregion
}