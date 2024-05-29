using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class BooleanFieldInfo : FieldInfo<bool?>, IPrimitiveType
    {
        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.boolean;
        
        [JsonProperty("isUnique", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isUnique")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsUnique { get; set; }
        
        #endregion

        #region Methods

        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            return exception == null;
        }

        public override object Clone()
        {
            return new BooleanFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                IsUnique = this.IsUnique,
                IsVirtual = this.IsVirtual,
                IsHidden = this.IsHidden,
                IsReadonly = this.IsReadonly,
                DefaultValue = this.DefaultValue,
            };
        }
        
        #endregion
    }
}