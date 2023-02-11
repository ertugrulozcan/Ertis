using System;
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

    private readonly int? minCount;
    private readonly int? maxCount;
    private IFieldInfo itemSchema;

    #endregion
    
    #region Properties

    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public override FieldType Type => FieldType.photoGallery;
    
    [JsonProperty("itemSchema")]
    [JsonConverter(typeof(FieldInfoJsonConverter))]
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

    [JsonProperty("minCount", NullValueHandling = NullValueHandling.Ignore)]
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

    #endregion

    #region Methods

    public override bool ValidateSchema(out Exception exception)
    {
        base.ValidateSchema(out exception);
        this.ValidateMinCount(out exception);
        this.ValidateMaxCount(out exception);

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
            MinCount = this.MinCount,
            MaxCount = this.MaxCount
        };
    }

    #endregion
}