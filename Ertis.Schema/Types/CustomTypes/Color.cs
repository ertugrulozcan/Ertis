using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class Color : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.color;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Color()
        {
            this.RegexPattern = "(?:#|0x)(?:[a-f0-9]{3}|[a-f0-9]{6})\\b|(?:rgb|hsl)a?\\([^\\)]*\\)";
        }

        #endregion

        #region Methods

        public override bool Validate(object obj, IValidationContext validationContext)
        {
            try
            {
                return base.Validate(obj, validationContext);
            }
            catch (FieldValidationException ex)
            {
                validationContext.Errors.Add(
                    ex.Message == $"String value is not valid by the regular expression rule. ('{this.RegexPattern}')"
                        ? new FieldValidationException("Color code is not valid", this)
                        : ex);

                return false;
            }
        }

        public override object Clone()
        {
            return new Color
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                DefaultValue = this.DefaultValue,
                MinLength = this.MinLength,
                MaxLength = this.MaxLength,
                RegexPattern = this.RegexPattern
            };
        }

        #endregion
    }
}