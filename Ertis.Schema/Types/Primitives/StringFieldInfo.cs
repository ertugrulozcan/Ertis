using System;
using System.Text.RegularExpressions;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class StringFieldInfo : FieldInfo<string>, IPrimitiveType
    {
        #region Fields

        private readonly int? minLength;
        private readonly int? maxLength;
        
        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.@string;
        
        [JsonProperty("minLength", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinLength
        {
            get => this.minLength;
            init
            {
                this.minLength = value;
                
                if (!this.ValidateMinLength(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("maxLength", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxLength
        {
            get => this.maxLength;
            init
            {
                this.maxLength = value;
                
                if (!this.ValidateMaxLength(out var exception))
                {
                    throw exception;
                }
            }
        }

        [JsonProperty("regexPattern", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RegexPattern { get; init; }

        [JsonProperty("isUnique", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsUnique { get; set; }
        
        #endregion

        #region Methods
        
        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateMinLength(out exception);
            this.ValidateMaxLength(out exception);

            return exception == null;
        }

        public override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);
            
            if (obj is string text)
            {
                if (this.MaxLength != null && text.Length > this.MaxLength.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"String length can not be greater than {this.MaxLength}", this));
                }
                
                if (this.MinLength != null && text.Length < this.MinLength.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"String length can not be less than {this.MinLength}", this));
                }

                if (!string.IsNullOrEmpty(this.RegexPattern))
                {
                    var match = Regex.Match(text, this.RegexPattern);
                    if (!match.Success)
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"String value is not valid by the regular expression rule. ('{this.RegexPattern}')", this));
                    }
                }
            }
            
            return isValid;
        }
        
        private bool ValidateMinLength(out Exception exception)
        {
            if (this.MinLength < 0)
            {
                exception = new FieldValidationException("MinLength can not be less than zero", this);
                return false;
            }

            if (this.MaxLength != null && this.MinLength != null && this.MaxLength < this.MinLength)
            {
                exception = new FieldValidationException("MinLength can not be greater than MaxLength", this);
                return false;
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateMaxLength(out Exception exception)
        {
            if (this.MaxLength < 0)
            {
                exception = new FieldValidationException("MaxLength can not be less than zero", this);
                return false;
            }

            if (this.MinLength != null && this.MaxLength != null && this.MinLength > this.MaxLength)
            {
                exception = new FieldValidationException("MinLength can not be greater than MaxLength", this);
                return false;
            }
            
            exception = null;
            return true;
        }

        public override object Clone()
        {
            return new StringFieldInfo
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