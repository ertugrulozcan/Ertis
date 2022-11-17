using System;
using System.Collections.Generic;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
	public sealed class ImageFieldInfo : ObjectFieldInfoBase
	{
        #region Fields

        private readonly int? maxSize;
        private readonly int? minCount;
        private readonly int? maxCount;
        private readonly int? maxWidth;
        private readonly int? maxHeight;
        private readonly int? minWidth;
        private readonly int? minHeight;
        
        #endregion
        
		#region Properties
        
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.image;
        
        [JsonIgnore]
        public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }
        
        [JsonProperty("multiple", NullValueHandling = NullValueHandling.Ignore)]
        public bool Multiple { get; set; }
        
        [JsonProperty("maxSize", NullValueHandling = NullValueHandling.Ignore)]
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
        
        [JsonProperty("minWidth", NullValueHandling = NullValueHandling.Ignore)]
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
        public int? RecommendedWidth { get; set; }
        
        [JsonProperty("recommendedHeight", NullValueHandling = NullValueHandling.Ignore)]
        public int? RecommendedHeight { get; set; }
        
        [JsonProperty("maxSizesRequired")]
        public bool MaxSizesRequired { get; set; }
        
        [JsonProperty("minSizesRequired")]
        public bool MinSizesRequired { get; set; }
        
        [JsonProperty("aspectRatioRequired")]
        public bool AspectRatioRequired { get; set; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageFieldInfo()
        {
            this.Properties = new FieldInfo[]
            {
                new StringFieldInfo
                {
                    Name = "id",
                    DisplayName = "Id",
                    Description = "File Id",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "name",
                    DisplayName = "File Name",
                    Description = "File Name",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "path",
                    DisplayName = "File Path",
                    Description = "File Path",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "fullPath",
                    DisplayName = "Full Path",
                    Description = "File Full Path",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "mimeType",
                    DisplayName = "Mime Type",
                    Description = "File Mime Type",
                    IsRequired = true
                },
                new FloatFieldInfo
                {
                    Name = "size",
                    DisplayName = "File Size",
                    Description = "File Size (bytes)",
                    IsRequired = false
                },
                new StringFieldInfo
                {
                    Name = "url",
                    DisplayName = "Url",
                    Description = "Url",
                    IsRequired = true
                }
            };
        }

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

            if (obj is object[] array)
            {
                if (this.MaxCount != null && array.Length > this.MaxCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"File count can not be greater than {this.MaxCount}", this));
                }
                
                if (this.MinCount != null && array.Length < this.MinCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"File count can not be less than {this.MinCount}", this));
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
            return new ImageFieldInfo
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
                Properties = this.Properties,
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
}