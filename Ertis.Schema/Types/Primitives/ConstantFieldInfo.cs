using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class ConstantFieldInfo : FieldInfo<object>
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.@const;
        
        [JsonProperty("value")]
        public object Value { get; set; }
        
        #endregion
        
        #region Methods

        public override object Clone()
        {
            return new ConstantFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                IsVirtual = this.IsVirtual,
                IsHidden = this.IsHidden,
                DefaultValue = this.DefaultValue,
                Value = this.Value,
            };
        }

        #endregion
    }
}