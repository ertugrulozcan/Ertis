using System;
using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Models;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class RichTextFieldInfo : StringFieldInfo
    {
        #region Fields

        private readonly int? minWordCount;
        private readonly int? maxWordCount;

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.richtext;

        [JsonProperty("minWordCount", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("minWordCount")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinWordCount
        {
            get => this.minWordCount;
            init
            {
                this.minWordCount = value;
                
                if (!this.ValidateMinWordCount(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("maxWordCount", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("maxWordCount")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxWordCount
        {
            get => this.maxWordCount;
            init
            {
                this.maxWordCount = value;
                
                if (!this.ValidateMaxWordCount(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("embeddedImageRules", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("embeddedImageRules")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ResolutionRules? EmbeddedImageRules { get; set; }
        
        [JsonProperty("embeddedImageMaxSize", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("embeddedImageMaxSize")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? EmbeddedImageMaxSize { get; set; }
            
        #endregion

        #region Methods
        
        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateMinWordCount(out exception);
            this.ValidateMaxWordCount(out exception);

            return exception == null;
        }

        protected internal override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);

            /*
            if (obj is string richText)
            {
                // TODO: CalculateTotalWordCount Method Implementation
                var wordCount = HtmlAgilityPack.CalculateTotalWordCount(richText);
                
                if (this.MaxWordCount != null && wordCount > this.MaxWordCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Total word count can not be greater than {this.MaxWordCount}", this));
                }
                
                if (this.MinWordCount != null && wordCount < this.MinWordCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Total word count can not be less than {this.MinWordCount}", this));
                }
            }
            */
            
            return isValid;
        }
        
        private bool ValidateMinWordCount(out Exception exception)
        {
            if (this.MinWordCount != null)
            {
                if (this.MinWordCount < 0)
                {
                    exception = new FieldValidationException($"The 'minWordCount' value can not be less than zero ('{this.Name}')", this);
                    return false;
                }

                if (this.MaxWordCount != null && this.MinWordCount != null && this.MaxWordCount < this.MinWordCount)
                {
                    exception = new FieldValidationException($"The 'minWordCount' value can not be greater than the 'maxWordCount' value ('{this.Name}')", this);
                    return false;
                }
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateMaxWordCount(out Exception exception)
        {
            if (this.MaxWordCount != null)
            {
                if (this.MaxWordCount < 0)
                {
                    exception = new FieldValidationException($"The 'maxWordCount' value can not be less than zero ('{this.Name}')", this);
                    return false;
                }

                if (this.MinWordCount != null && this.MaxWordCount != null && this.MinWordCount > this.MaxWordCount)
                {
                    exception = new FieldValidationException($"The 'minWordCount' value can not be greater than the 'maxWordCount' value ('{this.Name}')", this);
                    return false;
                }
            }
            
            exception = null;
            return true;
        }
        
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
                MinWordCount = this.MinWordCount,
                MaxWordCount = this.MaxWordCount,
                MinLength = this.MinLength,
                MaxLength = this.MaxLength,
                FormatPattern = this.FormatPattern,
                RegexPattern = this.RegexPattern,
                RestrictRegexPattern = this.RestrictRegexPattern,
                EmbeddedImageRules = this.EmbeddedImageRules,
                EmbeddedImageMaxSize = this.EmbeddedImageMaxSize
            };
        }

        #endregion
    }
}