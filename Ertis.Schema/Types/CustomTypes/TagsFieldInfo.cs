using System;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
	public class TagsFieldInfo : FieldInfo<string[]>
	{
		#region Fields

		private readonly int? minCount;
		private readonly int? maxCount;
        private readonly int? minLength;
        private readonly int? maxLength;

		#endregion
		
		#region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.tags;
        
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

        #endregion
        
        #region Methods
        
        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateMinCount(out exception);
            this.ValidateMaxCount(out exception);
            this.ValidateMinLength(out exception);
            this.ValidateMaxLength(out exception);

            return exception == null;
        }

        protected internal override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);

            if (obj is string[] array)
            {
                if (this.MaxCount != null && array.Length > this.MaxCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Tags item count can not be greater than {this.MaxCount}", this));
                }
                
                if (this.MinCount != null && array.Length < this.MinCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Tags item count can not be less than {this.MinCount}", this));
                }

                var uniqueCount = array.Cast<object>().Distinct().Count();
                if (array.Length != uniqueCount)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException("Tags items must be unique", this));
                }

                // Item validations
                if (array.Any(string.IsNullOrEmpty))
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException("Tags items can not be empty", this));
                }
                
                if (this.MaxLength != null && array.Any(x => x.Length > this.MaxLength.Value))
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The length of tag items can not be greater than {this.MaxLength} character", this));
                }
                
                if (this.MinLength != null && array.Any(x => x.Length < this.MinLength.Value))
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The length of tag items can not be less than {this.MinLength} character", this));
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
            return new TagsFieldInfo
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
                MaxCount = this.MaxCount,
                MinLength = this.MinLength,
                MaxLength = this.MaxLength,
            };
        }

        #endregion
	}
}