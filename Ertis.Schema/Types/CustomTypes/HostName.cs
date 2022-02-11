using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class HostName : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.hostname;

        #endregion

        #region Methods

        public override bool Validate(object obj, IValidationContext validationContext)
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

            try
            {
                var hostNameType = System.Uri.CheckHostName(hostName);
                return hostNameType != UriHostNameType.Unknown;
            }
            catch
            {
                return false;
            }
        }

        public override object Clone()
        {
            return new HostName
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