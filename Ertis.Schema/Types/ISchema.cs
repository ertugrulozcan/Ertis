using System;
using System.Collections.Generic;
using Ertis.Schema.Serialization;
using Newtonsoft.Json;

namespace Ertis.Schema.Types
{
    public interface ISchema
    {
        #region Properties

        [JsonProperty("properties")]
        [JsonConverter(typeof(FieldInfoCollectionJsonConverter))]
        IReadOnlyCollection<IFieldInfo> Properties { get; }

        #endregion

        #region Methods

        bool ValidateSchema(out Exception exception);
        
        bool ValidateContent(object obj, out Exception exception);

        #endregion
    }
}