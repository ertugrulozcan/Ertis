using System;
using System.Collections.Generic;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Serialization;
using Ertis.Schema.Validation;
using Newtonsoft.Json;

namespace Ertis.Schema.Types
{
    public interface ISchema
    {
        #region Properties

        [JsonProperty("slug")]
        string Slug { get; }
        
        [JsonProperty("properties")]
        [JsonConverter(typeof(FieldInfoCollectionJsonConverter))]
        IReadOnlyCollection<IFieldInfo> Properties { get; }
        
        #endregion

        #region Methods

        bool ValidateSchema(out Exception exception);
        
        bool ValidateContent(DynamicObject obj, IValidationContext validationContext);

        #endregion
    }
}