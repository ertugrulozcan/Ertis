using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Ertis.Schema.Dynamics.Legacy;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class StringFieldInfo : FieldInfo<string>, IPrimitiveType
    {
        #region Constants

        private const string OPEN_FORMAT_BRACKETS = "{";
        private const string CLOSE_FORMAT_BRACKETS = "}";

        #endregion
        
        #region Fields

        private readonly int? minLength;
        private readonly int? maxLength;
        private readonly string formatPattern;
        
        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.@string;
        
        [JsonProperty("minLength", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("minLength")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
        [JsonPropertyName("maxLength")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
        
        [JsonProperty("formatPattern", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("formatPattern")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FormatPattern
        {
            get => this.formatPattern;
            init
            {
                this.formatPattern = value;
                
                if (!this.ValidateFormatPattern(out var exception))
                {
                    throw exception;
                }
                
                this.OnPropertyChanged(nameof(this.FormatPattern));
            }
        }

        [JsonProperty("regexPattern", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("regexPattern")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string RegexPattern { get; init; }
        
        [JsonProperty("restrictRegexPattern", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("restrictRegexPattern")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string RestrictRegexPattern { get; init; }
        
        [JsonProperty("caseInsensitive", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("caseInsensitive")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool CaseInsensitive { get; init; }

        [JsonProperty("isUnique", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isUnique")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsUnique { get; set; }
        
        #endregion

        #region Methods
        
        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateMinLength(out exception);
            this.ValidateMaxLength(out exception);
            this.ValidateFormatPattern(out exception);

            return exception == null;
        }

        protected internal override bool Validate(object obj, IValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(this.FormatPattern))
            {
                if (this.TryFormat(validationContext.Content, out var formattedString))
                {
                    obj = formattedString;
                }
            }
            
            var isValid = base.Validate(obj, validationContext);
            
            if (obj is string text)
            {
                if (this.IsRequired && string.IsNullOrEmpty(text.Trim()))
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"{this.Name} is required", this));
                }
                
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

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(this.RegexPattern))
                {
                    var match = Regex.Match(text, this.RegexPattern);
                    if (!match.Success)
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"String value is not valid by the regular expression rule. ('{this.RegexPattern}')", this));
                    }
                }

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(this.RestrictRegexPattern))
                {
                    var match = Regex.Match(text, this.RestrictRegexPattern);
                    if (match.Success)
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"String value is not valid by the restrict regular expression rule. ('{this.RestrictRegexPattern}')", this));
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
        
        private bool ValidateFormatPattern(out Exception exception)
        {
            if (this.FormatPattern != null)
            {
                var segments = GetFormatSegments();
                if (segments.Any(x => x.Contains(' ')))
                {
                    exception = new FieldValidationException("The 'formatPattern' segments can not be contains whitespace", this);
                    return false;   
                }
            }
            
            exception = null;
            return true;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string Format(DynamicObject content)
        {
            if (!string.IsNullOrEmpty(this.FormatPattern) && content != null)
            {
                var text = new string(this.FormatPattern.Trim());
                while (text.Contains(OPEN_FORMAT_BRACKETS) && text.Contains(CLOSE_FORMAT_BRACKETS))
                {
                    var openIndex = text.IndexOf(OPEN_FORMAT_BRACKETS, StringComparison.Ordinal);
                    var closeIndex = text.IndexOf(CLOSE_FORMAT_BRACKETS, StringComparison.Ordinal);
                    var segment = text.Substring(openIndex + OPEN_FORMAT_BRACKETS.Length, closeIndex - openIndex - OPEN_FORMAT_BRACKETS.Length);
                    if (content.TryGetValue(segment.Trim(), out var value, out _))
                    {
                        text = text.Replace($"{OPEN_FORMAT_BRACKETS}{segment}{CLOSE_FORMAT_BRACKETS}", value != null ? value.ToString() : "null");
                    }
                    else
                    {
                        throw new FormatException("Format parameter value could not be retrieved");
                    }
                }

                return text;
            }
            else
            {
                return null;
            }
        }

        public bool TryFormat(DynamicObject content, out string value)
        {
            try
            {
                value = this.Format(content);
                return true;
            }
            catch (FormatException)
            {
                value = null;
                return false;
            }
        }
        
        private IEnumerable<string> GetFormatSegments()
        {
            if (!string.IsNullOrEmpty(this.FormatPattern))
            {
                var text = new string(this.FormatPattern.Trim());
                while (text.Contains(OPEN_FORMAT_BRACKETS) && text.Contains(CLOSE_FORMAT_BRACKETS))
                {
                    var openIndex = text.IndexOf(OPEN_FORMAT_BRACKETS, StringComparison.Ordinal);
                    var closeIndex = text.IndexOf(CLOSE_FORMAT_BRACKETS, StringComparison.Ordinal);
                    var segment = text.Substring(openIndex + OPEN_FORMAT_BRACKETS.Length, closeIndex - openIndex - OPEN_FORMAT_BRACKETS.Length);
                    text = text.Replace($"{OPEN_FORMAT_BRACKETS}{segment}{CLOSE_FORMAT_BRACKETS}", string.Empty);
                    yield return segment;
                }
            }
        }
        
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(this.FormatPattern))
            {
                this.ValidateHiddenRules();
            }
        }
        
        protected override void ValidateHiddenRules()
        {
            if (this.IsHidden && this.IsRequired && this.DefaultValue == null && string.IsNullOrEmpty(this.FormatPattern))
            {
                throw new FieldValidationException("A field with a default value of null cannot be both hidden and required.", this);
            }
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
                CaseInsensitive = this.CaseInsensitive
            };
        }
        
        #endregion
    }
}