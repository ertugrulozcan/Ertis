using System;
using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types
{
    public interface IFieldInfo : ICloneable
    {
        #region Properties

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        string Name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        string Path { get; }
        
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        IFieldInfo Parent { get; set; }
        
        [JsonProperty("displayName")]
        [JsonPropertyName("displayName")]
        string DisplayName { get; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("description")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string Description { get; }

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        FieldType Type { get; }

        [JsonProperty("isRequired", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isRequired")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        bool IsRequired { get; }

        [JsonProperty("isVirtual", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isVirtual")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        bool IsVirtual { get; init; }
        
        [JsonProperty("isHidden", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isHidden")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        bool IsHidden { get; init; }
        
        [JsonProperty("isReadonly", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isReadonly")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        bool IsReadonly { get; init; }
        
        [JsonProperty("isSearchable", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isSearchable")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        bool IsSearchable { get; init; }
        
        [JsonProperty("searchWeight", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("searchWeight")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        double? SearchWeight { get; init; }

        #endregion
        
        #region Methods

        bool ValidateSchema(out Exception exception);

        bool IsAnArrayItem(out ArrayFieldInfo arrayFieldInfo);

        #endregion
    }
}