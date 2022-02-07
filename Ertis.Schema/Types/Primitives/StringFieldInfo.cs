using System;
using System.Text.RegularExpressions;
using Ertis.Schema.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class StringFieldInfo : FieldInfo<string>
    {
        #region Fields

        private readonly int? minLength;
        private readonly int? maxLength;
        
        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override PrimitiveType Type => PrimitiveType.@string;
        
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

        #endregion

        #region Methods
        
        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateMinLength(out exception);
            this.ValidateMaxLength(out exception);

            return exception == null;
        }

        protected override void Validate(object obj)
        {
            base.Validate(obj);
            
            if (obj is string text)
            {
                if (this.MaxLength != null && text.Length > this.MaxLength.Value)
                {
                    throw new FieldValidationException($"String length can not be greater than {this.MaxLength}", this);
                }
                
                if (this.MinLength != null && text.Length < this.MinLength.Value)
                {
                    throw new FieldValidationException($"String length can not be less than {this.MinLength}", this);
                }

                if (!string.IsNullOrEmpty(this.RegexPattern))
                {
                    try
                    {
                        var match = Regex.Match(text, this.RegexPattern);
                        if (!match.Success)
                        {
                            throw new FieldValidationException($"String value is not valid by the regular expression rule. ({this.RegexPattern})", this);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FieldValidationException($"String value is not valid by the regular expression rule. ({ex.Message})", this);
                    }
                }
            }
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
        
        #endregion
    }
}