using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;

// ReSharper disable UnusedMember.Global
namespace Ertis.Schema.Models;

public class DynamicQueryParameter
{
	#region Properties
	
	[JsonPropertyName("name")]
	public string? Name { get; set; }
	
	[JsonPropertyName("slug")]
	public string? Slug { get; set; }
	
	[JsonPropertyName("description")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Description { get; set; }
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public DynamicQueryParameterType Type { get; set; }
	
	[JsonPropertyName("defaultValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public object? DefaultValue
	{
		get;
		set => field = DynamicValues.FromDeserializedValue(value);
	}
	
	[JsonPropertyName("isRequired")]
	public bool IsRequired { get; set; }
	
	[JsonPropertyName("isNullable")]
	public bool IsNullable { get; set; }
	
	#endregion
}

public enum DynamicQueryParameterType
{
	@string,
	number,
	date,
	boolean,
	array
}