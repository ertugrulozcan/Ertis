using System;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes;

public class PhotoGalleryFieldInfo : FieldInfo<Array>
{
	#region Fields

    private readonly int? maxSize;
    private readonly int? minCount;
    private readonly int? maxCount;
    private readonly int? maxWidth;
    private readonly int? maxHeight;
    private readonly int? minWidth;
    private readonly int? minHeight;
    private IFieldInfo itemSchema;

    #endregion
    
    #region Properties

    [JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    [JsonPropertyName("type")]
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public override FieldType Type => FieldType.photoGallery;
    
    [JsonProperty("itemSchema")]
    [JsonPropertyName("itemSchema")]
    [Newtonsoft.Json.JsonConverter(typeof(FieldInfoJsonConverter))]
    public IFieldInfo ItemSchema
    {
        get
        {
            if (this.itemSchema == null)
            {
                var properties = new IFieldInfo[]
                {
                    new ImageFieldInfo
                    {
                        DisplayName = "Image",
                        Name = "image",
                        IsRequired = true,
                        IsVirtual = false
                    },
                    new StringFieldInfo
                    {
                        DisplayName = "Title",
                        Name = "title",
                        IsRequired = false,
                        IsVirtual = false
                    },
                    new StringFieldInfo
                    {
                        DisplayName = "Description",
                        Name = "description",
                        IsRequired = false,
                        IsVirtual = false
                    },
                    new RichTextFieldInfo
                    {
                        DisplayName = "Body",
                        Name = "body",
                        IsRequired = false,
                        IsVirtual = false
                    }
                };
                
                this.itemSchema = new ObjectFieldInfo(properties)
                {
                    Parent = this
                };
            }

            return this.itemSchema;
        }
    }
    
    [JsonProperty("maxSize", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("maxSize")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxSize
    {
        get => this.maxSize;
        init
        {
            this.maxSize = value;
                
            if (!this.ValidateMaxSize(out var exception))
            {
                throw exception;
            }
        }
    }

    [JsonProperty("minCount", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("minCount")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinCount
    {
        get => this.minCount;
        init
        {
            this.minCount = value;
            
            if (!this.ValidateMinCount(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("maxCount", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("maxCount")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxCount
    {
        get => this.maxCount;
        init
        {
            this.maxCount = value;
            
            if (!this.ValidateMaxCount(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("minWidth", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("minWidth")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinWidth
    {
        get => this.minWidth;
        init
        {
            this.minWidth = value;
            
            if (!this.ValidateMinWidth(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("minHeight", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("minHeight")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinHeight
    {
        get => this.minHeight;
        init
        {
            this.minHeight = value;
            
            if (!this.ValidateMinHeight(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("maxWidth", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("maxWidth")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxWidth
    {
        get => this.maxWidth;
        init
        {
            this.maxWidth = value;
            
            if (!this.ValidateMaxWidth(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("maxHeight", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("maxHeight")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxHeight
    {
        get => this.maxHeight;
        init
        {
            this.maxHeight = value;
            
            if (!this.ValidateMaxHeight(out var exception))
            {
                throw exception;
            }
        }
    }
    
    [JsonProperty("recommendedWidth", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("recommendedWidth")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RecommendedWidth { get; set; }
    
    [JsonProperty("recommendedHeight", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("recommendedHeight")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RecommendedHeight { get; set; }
    
    [JsonProperty("maxSizesRequired")]
    [JsonPropertyName("maxSizesRequired")]
    public bool MaxSizesRequired { get; set; }
    
    [JsonProperty("minSizesRequired")]
    [JsonPropertyName("minSizesRequired")]
    public bool MinSizesRequired { get; set; }
    
    [JsonProperty("aspectRatioRequired")]
    [JsonPropertyName("aspectRatioRequired")]
    public bool AspectRatioRequired { get; set; }

    #endregion

    #region Methods

    public override bool ValidateSchema(out Exception exception)
    {
        base.ValidateSchema(out exception);
        this.ValidateMinCount(out exception);
        this.ValidateMaxCount(out exception);
        this.ValidateMaxSize(out exception);

        return exception == null;
    }

    protected internal override bool Validate(object obj, IValidationContext validationContext)
    {
        var isValid = base.Validate(obj, validationContext);

        if (obj is Array array)
        {
            if (this.MaxCount != null && array.Length > this.MaxCount.Value)
            {
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"Photo gallery array length can not be greater than {this.MaxCount}", this));
            }
            
            if (this.MinCount != null && array.Length < this.MinCount.Value)
            {
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"Photo gallery array length can not be less than {this.MinCount}", this));
            }

            // Item validations
            foreach (var item in array)
            {
                var itemFieldInfo = (FieldInfo) this.ItemSchema.Clone();
                isValid &= itemFieldInfo.Validate(item, validationContext);
            }
        }
        
        return isValid;
    }
    
    private bool ValidateMaxSize(out Exception exception)
    {
        if (this.MaxSize < 0)
        {
            exception = new FieldValidationException("MaxSize can not be less than zero", this);
            return false;
        }
            
        exception = null;
        return true;
    }

    private bool ValidateMinCount(out Exception exception)
    {
        if (this.MinCount != null)
        {
            if (this.MinCount < 0)
            {
                exception = new FieldValidationException($"The 'minCount' value can not be less than zero ('{this.Name}')", this);
                return false;
            }

            if (this.MaxCount != null && this.MinCount != null && this.MaxCount < this.MinCount)
            {
                exception = new FieldValidationException($"The 'minCount' value can not be greater than the 'maxCount' value ('{this.Name}')", this);
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMaxCount(out Exception exception)
    {
        if (this.MaxCount != null)
        {
            if (this.MaxCount < 0)
            {
                exception = new FieldValidationException($"The 'maxCount' value can not be less than zero ('{this.Name}')", this);
                return false;
            }

            if (this.MinCount != null && this.MaxCount != null && this.MinCount > this.MaxCount)
            {
                exception = new FieldValidationException($"The 'minCount' value can not be greater than the 'maxCount' value ('{this.Name}')", this);
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMinWidth(out Exception exception)
    {
        if (this.MinWidth != null)
        {
            if (this.MinWidth < 0)
            {
                exception = new FieldValidationException($"The 'minWidth' value can not be less than zero ('{this.Name}')", this);
                return false;
            }

            if (this.MaxWidth != null && this.MinWidth != null && this.MaxWidth < this.MinWidth)
            {
                exception = new FieldValidationException($"The 'minWidth' value can not be greater than the 'maxWidth' value ('{this.Name}')", this);
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMinHeight(out Exception exception)
    {
        if (this.MinHeight != null)
        {
            if (this.MinHeight < 0)
            {
                exception = new FieldValidationException($"The 'minHeight' value can not be less than zero ('{this.Name}')", this);
                return false;
            }

            if (this.MaxHeight != null && this.MinHeight != null && this.MaxHeight < this.MinHeight)
            {
                exception = new FieldValidationException($"The 'minHeight' value can not be greater than the 'maxHeight' value ('{this.Name}')", this);
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMaxWidth(out Exception exception)
    {
        if (this.MaxWidth != null)
        {
            if (this.MaxWidth < 0)
            {
                exception = new FieldValidationException($"The 'maxWidth' value can not be less than zero ('{this.Name}')", this);
                return false;
            }

            if (this.MinWidth != null && this.MaxWidth != null && this.MinWidth > this.MaxWidth)
            {
                exception = new FieldValidationException($"The 'minWidth' value can not be greater than the 'maxWidth' value ('{this.Name}')", this);
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMaxHeight(out Exception exception)
    {
        if (this.MaxHeight != null)
        {
            if (this.MaxHeight < 0)
            {
                exception = new FieldValidationException($"The 'maxHeight' value can not be less than zero ('{this.Name}')", this);
                return false;
            }

            if (this.MinHeight != null && this.MaxHeight != null && this.MinHeight > this.MaxHeight)
            {
                exception = new FieldValidationException($"The 'minHeight' value can not be greater than the 'maxHeight' value ('{this.Name}')", this);
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    public override object Clone()
    {
        return new PhotoGalleryFieldInfo
        {
            Name = this.Name,
            Description = this.Description,
            DisplayName = this.DisplayName,
            Parent = this.Parent,
            IsRequired = this.IsRequired,
            IsVirtual = this.IsVirtual,
            IsHidden = this.IsHidden,
            IsReadonly = this.IsReadonly,
            DefaultValue = this.DefaultValue,
            MaxSize = this.MaxSize,
            MinCount = this.MinCount,
            MaxCount = this.MaxCount,
            MinWidth = this.MinWidth,
            MinHeight = this.MinHeight,
            MaxWidth = this.MaxWidth,
            MaxHeight = this.MaxHeight,
            RecommendedWidth = this.RecommendedWidth,
            RecommendedHeight = this.RecommendedHeight,
            MinSizesRequired = this.MinSizesRequired,
            MaxSizesRequired = this.MaxSizesRequired,
            AspectRatioRequired = this.AspectRatioRequired
        };
    }

    #endregion
}