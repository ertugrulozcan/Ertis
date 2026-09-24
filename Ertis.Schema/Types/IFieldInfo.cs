using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

using NewtonsoftJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;
using NewtonsoftJsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NewtonsoftJsonConverter = Newtonsoft.Json.JsonConverterAttribute;
using NewtonsoftStringEnumConverter = Newtonsoft.Json.Converters.StringEnumConverter;

namespace Ertis.Schema.Types;

public interface IFieldInfo : ICloneable
{
    #region Properties
    
    [JsonIgnore]
    [NewtonsoftJsonIgnore]
    string Name { get; set; }
    
    [JsonIgnore]
    [NewtonsoftJsonIgnore]
    string Path { get; }
    
    [JsonIgnore]
    [NewtonsoftJsonIgnore]
    IFieldInfo? Parent { get; set; }
    
    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [NewtonsoftJsonProperty("displayName", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    string? DisplayName { get; }
    
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [NewtonsoftJsonProperty("description", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    string? Description { get; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [NewtonsoftJsonProperty("type")]
    [NewtonsoftJsonConverter(typeof(NewtonsoftStringEnumConverter))]
    FieldType Type { get; }
    
    [JsonPropertyName("isRequired")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [NewtonsoftJsonProperty("isRequired", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsRequired { get; }
    
    [JsonPropertyName("isVirtual")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [NewtonsoftJsonProperty("isVirtual", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsVirtual { get; init; }
    
    [JsonPropertyName("isHidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [NewtonsoftJsonProperty("isHidden", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsHidden { get; init; }
    
    [JsonPropertyName("isReadonly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [NewtonsoftJsonProperty("isReadonly", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsReadonly { get; init; }
    
    [JsonPropertyName("isSearchable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [NewtonsoftJsonProperty("isSearchable", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsSearchable { get; init; }
    
    [JsonPropertyName("searchWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [NewtonsoftJsonProperty("searchWeight", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    double? SearchWeight { get; init; }
    
    #endregion
    
    #region Methods
    
    bool ValidateSchema(out Exception? exception);
    
    bool IsAnArrayItem(out ArrayFieldInfo? arrayFieldInfo);
    
    #endregion
}