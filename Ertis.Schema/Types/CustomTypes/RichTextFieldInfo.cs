using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class RichTextFieldInfo : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.richtext;

        #endregion

        #region Methods
        
        public override object Clone()
        {
            return new RichTextFieldInfo
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
                MinLength = this.MinLength,
                MaxLength = this.MaxLength,
                FormatPattern = this.FormatPattern,
                RegexPattern = this.RegexPattern,
                RestrictRegexPattern = this.RestrictRegexPattern,
            };
        }

        #endregion
    }
}