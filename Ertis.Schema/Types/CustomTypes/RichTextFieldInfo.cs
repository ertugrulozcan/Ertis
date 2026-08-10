using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Models;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.CustomTypes;

public class RichTextFieldInfo : StringFieldInfo
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.richtext;
    
    [JsonPropertyName("minWordCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("minWordCount", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? MinWordCount
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateMinWordCount(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    [JsonPropertyName("maxWordCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("maxWordCount", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? MaxWordCount
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateMaxWordCount(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    [JsonPropertyName("embeddedImageRules")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("embeddedImageRules", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ResolutionRules? EmbeddedImageRules { get; set; }
    
    [JsonPropertyName("embeddedImageMaxSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("embeddedImageMaxSize", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? EmbeddedImageMaxSize { get; set; }
    
    #endregion
    
    #region Methods
    
    public override bool ValidateSchema(out Exception? exception)
    {
        base.ValidateSchema(out exception);
        this.ValidateMinWordCount(out exception);
        this.ValidateMaxWordCount(out exception);
        
        return exception == null;
    }
    
    protected internal override bool Validate(object? obj, IValidationContext validationContext)
    {
        var isValid = base.Validate(obj, validationContext);
        
        /*
        if (obj is string richText)
        {
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
    
    private bool ValidateMinWordCount(out Exception? exception)
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
    
    private bool ValidateMaxWordCount(out Exception? exception)
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
            CaseInsensitive = this.CaseInsensitive,
            EmbeddedImageRules = this.EmbeddedImageRules,
            EmbeddedImageMaxSize = this.EmbeddedImageMaxSize
        };
    }
    
    #endregion
}