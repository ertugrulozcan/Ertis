using System.Text.Json.Serialization;
using Ertis.Schema.Types;
using Ertis.Schema.Validation;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Schema.Exceptions;

public class FieldValidationException : ErtisSchemaValidationException
{
	#region Properties
	
	[JsonIgnore]
	private IFieldInfo FieldInfo { get; }
	
	/// <summary>
	/// The path of the invalid value when the exception is created during a data validation (includes the array item indexes)
	/// </summary>
	[JsonIgnore]
	private string? CapturedPath { get; }
	
	[JsonPropertyName("fieldName")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string FieldName => this.FieldInfo.Name;
	
	[JsonPropertyName("fieldPath")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string FieldPath => this.CapturedPath ?? this.FieldInfo.Path;
	
	[JsonPropertyName("throwEvenOnCreate")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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