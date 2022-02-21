using System.Globalization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class DateFieldInfo : StringFieldInfo
    {
        #region Constants

        private const string StringFormat = "yyyy-MM-dd";

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.date;

        #endregion

        #region Methods

        public override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);

            if (obj is not System.DateTime)
            {
                if (obj is string dateStr)
                {
                    if (!IsValidDate(dateStr))
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"Date is not valid. Datetime values must be '{StringFormat}' format.", this));
                    }
                }
            }

            return isValid;
        }
        
        private static bool IsValidDate(string dateString)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrWhiteSpace(dateString))
                return false;
            
            return System.DateTime.TryParseExact(dateString, StringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        public override object Clone()
        {
            return new DateFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                IsUnique = this.IsUnique,
                IsVirtual = this.IsVirtual,
                IsHidden = this.IsHidden,
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