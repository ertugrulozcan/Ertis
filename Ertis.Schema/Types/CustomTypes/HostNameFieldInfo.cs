using System;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class HostNameFieldInfo : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.hostname;

        #endregion

        #region Methods

        protected internal override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);
            
            if (obj is string hostName)
            {
                if (!IsValidHostName(hostName))
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Hostname is not valid", this));
                }
            }

            return isValid;
        }
        
        private static bool IsValidHostName(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                return false;
            
            if (!hostName.Contains('.'))
                return false;

            try
            {
                var testScheme = "http";
                if (Uri.TryCreate(hostName, UriKind.Absolute, out var uriResult))
                {
                    if (HasScheme(uriResult))
                    {
                        testScheme = uriResult.Scheme;
                    }
                    
                    return Uri.IsWellFormedUriString($"{testScheme}://{uriResult}", UriKind.Absolute);
                }
                else if (!hostName.Contains("://")) 
                {
                    return IsValidHostName($"{testScheme}://{hostName}");
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool HasScheme(Uri uri)
        {
            try
            {
                return !string.IsNullOrEmpty(uri.Scheme);
            }
            catch
            {
                return false;
            }
        }

        public override object Clone()
        {
            return new HostNameFieldInfo
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