using System.Globalization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class DateTimeFieldInfo : StringFieldInfo
    {
        #region Constants

        private const string StringFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.datetime;

        #endregion

        #region Methods

        public override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);

            if (obj is not System.DateTime)
            {
                if (obj is string dateStr)
                {
                    if (!IsValidDateTime(dateStr))
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"Datetime is not valid. Datetime values must be '{StringFormat}' format.", this));
                    }
                }
            }

            return isValid;
        }
        
        private static bool IsValidDateTime(string dateString)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrWhiteSpace(dateString))
                return false;

            return System.DateTime.TryParseExact(dateString, StringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        public override object Clone()
        {
            return new DateTimeFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                DefaultValue = this.DefaultValue,
                MinLength = this.MinLength,
                MaxLength = this.MaxLength,
                RegexPattern = this.RegexPattern,
                RestrictRegexPattern = this.RestrictRegexPattern
            };
        }

        #endregion
    }
}