using System.Text.Json.Serialization;
using Ertis.Schema.Types;
using Ertis.Schema.Validation;
using NewtonsoftJsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NewtonsoftJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Schema.Exceptions;

public class FieldValidationException : ErtisSchemaValidationException
{
	#region Properties
	
	[JsonIgnore]
	[NewtonsoftJsonIgnore]
	private IFieldInfo FieldInfo { get; }
	
	/// <summary>
	/// The path of the invalid value when the exception is created during a data validation (includes the array item indexes)
	/// </summary>
	[JsonIgnore]
	[NewtonsoftJsonIgnore]
	private string? CapturedPath { get; }
	
	[JsonPropertyName("fieldName")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[NewtonsoftJsonProperty("fieldName", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string FieldName => this.FieldInfo.Name;
	
	[JsonPropertyName("fieldPath")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[NewtonsoftJsonProperty("fieldPath", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string FieldPath => this.CapturedPath ?? this.FieldInfo.Path;
	
	[JsonPropertyName("throwEvenOnCreate")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[NewtonsoftJsonProperty("throwEvenOnCreate", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
	public bool ThrowEvenOnCreate { get; init; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message"></param>
	/// <param name="fieldInfo"></param>
	public FieldValidationException(string message, IFieldInfo fieldInfo) : base(message)
	{
		this.FieldInfo = fieldInfo;
		this.CapturedPath = ValidationPath.Current;
	}
	
	#endregion
}