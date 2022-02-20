using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class JsonFieldInfo : FieldInfo<object>
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.json;

        #endregion

        #region Methods

        public override object Clone()
        {
            return new JsonFieldInfo()
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                DefaultValue = this.DefaultValue,
                IsVirtual = this.IsVirtual
            };
        }

        #endregion
    }
}