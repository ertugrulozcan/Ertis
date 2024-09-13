using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics.Legacy;
using Ertis.Schema.Serialization;
using Ertis.Schema.Validation;
using Newtonsoft.Json;

namespace Ertis.Schema.Types
{
    public interface ISchema
    {
        #region Properties

        [JsonProperty("slug")]
        [JsonPropertyName("slug")]
        string Slug { get; }
        
        [JsonProperty("properties")]
        [JsonPropertyName("properties")]
        [Newtonsoft.Json.JsonConverter(typeof(FieldInfoCollectionJsonConverter))]
        IReadOnlyCollection<IFieldInfo> Properties { get; }
        
        [JsonProperty("allowAdditionalProperties")]
        [JsonPropertyName("allowAdditionalProperties")]
        bool AllowAdditionalProperties { get; }
        
        #endregion

        #region Methods

        bool ValidateSchema(out Exception exception);
        
        bool ValidateContent(DynamicObject obj, IValidationContext validationContext);

        #endregion
    }
}