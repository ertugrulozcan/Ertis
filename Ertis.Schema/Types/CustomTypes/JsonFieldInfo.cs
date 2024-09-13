using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics.Legacy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Types.CustomTypes
{
    public class JsonFieldInfo : FieldInfo<object>
    {
        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.json;

        #endregion

        #region Methods
        
        public override object GetDefaultValue()
        {
            var defaultValue = base.GetDefaultValue();
            if (defaultValue is JObject jObject)
            {
                return DynamicObject.Load(jObject).ToDynamic();
            }
            else
            {
                return defaultValue;
            }
        }

        public override object Clone()
        {
            return new JsonFieldInfo()
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                IsVirtual = this.IsVirtual,
                IsHidden = this.IsHidden,
                IsReadonly = this.IsReadonly,
                DefaultValue = this.DefaultValue,
            };
        }

        #endregion
    }
}