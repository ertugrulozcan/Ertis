using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class Uri : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.uri;

        #endregion

        #region Methods

        public override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);
            
            if (obj is string uri)
            {
                if (!IsValidUri(uri))
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Uri is not valid", this));
                }
            }

            return isValid;
        }
        
        private static bool IsValidUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return false;

            return System.Uri.IsWellFormedUriString(uri, UriKind.Absolute) &&
                   System.Uri.TryCreate(uri, UriKind.Absolute, out _);
        }
        
        public override object Clone()
        {
            return new Uri
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