using System;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types
{
    public interface IFieldInfo : ICloneable
    {
        #region Properties

        [JsonIgnore]
        string Name { get; set; }

        [JsonIgnore]
        string Path { get; }
        
        [JsonIgnore]
        IFieldInfo Parent { get; set; }
        
        [JsonProperty("displayName")]
        string DisplayName { get; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        string Description { get; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        FieldType Type { get; }

        [JsonProperty("isRequired", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        bool IsRequired { get; }

        #endregion
        
        #region Methods

        bool ValidateSchema(out Exception exception);

        bool IsAnArrayItem(out ArrayFieldInfo arrayFieldInfo);

        #endregion
    }
}