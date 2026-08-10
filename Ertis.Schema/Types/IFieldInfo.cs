using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Schema.Types;

public interface IFieldInfo : ICloneable
{
    #region Properties
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    string Name { get; set; }
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    string Path { get; }
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    IFieldInfo? Parent { get; set; }
    
    [JsonPropertyName("displayName")]
    [Newtonsoft.Json.JsonProperty("displayName")]
    string DisplayName { get; }
    
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("description", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    string? Description { get; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    FieldType Type { get; }
    
    [JsonPropertyName("isRequired")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isRequired", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsRequired { get; }
    
    [JsonPropertyName("isVirtual")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isVirtual", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsVirtual { get; init; }
    
    [JsonPropertyName("isHidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isHidden", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsHidden { get; init; }
    
    [JsonPropertyName("isReadonly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isReadonly", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsReadonly { get; init; }
    
    [JsonPropertyName("isSearchable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isSearchable", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsSearchable { get; init; }
    
    [JsonPropertyName("searchWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("searchWeight", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    double? SearchWeight { get; init; }
    
    #endregion
    
    #region Methods
    
    bool ValidateSchema(out Exception? exception);
    
    bool IsAnArrayItem(out ArrayFieldInfo? arrayFieldInfo);
    
    #endregion
}