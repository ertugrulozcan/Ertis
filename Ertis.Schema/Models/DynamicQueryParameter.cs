using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global
namespace Ertis.Schema.Models;

public class DynamicQueryParameter
{
	#region Properties
	
	[JsonPropertyName("name")]
	[Newtonsoft.Json.JsonProperty("name")]
	public string? Name { get; set; }
	
	[JsonPropertyName("slug")]
	[Newtonsoft.Json.JsonProperty("slug")]
	public string? Slug { get; set; }
	
	[JsonPropertyName("description")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("description", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public string? Description { get; set; }
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Newtonsoft.Json.JsonProperty("type")]
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public DynamicQueryParameterType Type { get; set; }
	
	[JsonPropertyName("defaultValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("defaultValue", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public object? DefaultValue { get; set; }
	
	[JsonPropertyName("isRequired")]
	[Newtonsoft.Json.JsonProperty("isRequired")]
	public bool IsRequired { get; set; }
	
	[JsonPropertyName("isNullable")]
	[Newtonsoft.Json.JsonProperty("isNullable")]
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