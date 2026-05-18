using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Schema.Types;

public interface IFieldInfo : ICloneable
{
    #region Properties
    
    [JsonIgnore]
    string Name { get; set; }
    
    [JsonIgnore]
    string Path { get; }
    
    [JsonIgnore]
    IFieldInfo? Parent { get; set; }
    
    [JsonPropertyName("displayName")]
    string? DisplayName { get; }
    
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Description { get; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    FieldType Type { get; }
    
    [JsonPropertyName("isRequired")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    bool IsRequired { get; }
    
    [JsonPropertyName("isVirtual")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    bool IsVirtual { get; init; }
    
    [JsonPropertyName("isHidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    bool IsHidden { get; init; }
    
    [JsonPropertyName("isReadonly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    bool IsReadonly { get; init; }
    
    [JsonPropertyName("isSearchable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    bool IsSearchable { get; init; }
    
    [JsonPropertyName("searchWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    double? SearchWeight { get; init; }
    
    #endregion
    
    #region Methods
    
    bool ValidateSchema(out Exception? exception);
    
    bool IsAnArrayItem(out ArrayFieldInfo? arrayFieldInfo);
    
    #endregion
}