using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class BooleanFieldInfo : FieldInfo<bool>
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override PrimitiveType Type => PrimitiveType.boolean;
        
        #endregion

        #region Methods

        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            return exception == null;
        }

        #endregion
    }
}