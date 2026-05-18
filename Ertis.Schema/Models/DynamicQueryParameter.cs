using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

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
	public object? DefaultValue { get; set; }
	
	[JsonPropertyName("isRequired")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsRequired { get; set; }
	
	[JsonPropertyName("isNullable")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsNullable { get; set; }
	
	#endregion
}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum DynamicQueryParameterType
{
	@string,
	number,
	date,
	boolean,
	array
}
