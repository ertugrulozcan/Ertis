using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class FloatFieldInfo : FieldInfo<double?>, IPrimitiveType
    {
        #region Fields

        private readonly double? minimum;
        private readonly double? maximum;
        private readonly double? exclusiveMinimum;
        private readonly double? exclusiveMaximum;
        
        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.@float;
        
        /// <summary>
        /// Greater than or equal
        /// </summary>
        [JsonProperty("minimum", NullValueHandling = NullValueHandling.Ignore)]
        public double? Minimum
        {
            get => this.minimum;
            init
            {
                this.minimum = value;

                if (!this.ValidateMinimum(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        /// <summary>
        /// Less than or equal
        /// </summary>
        [JsonProperty("maximum", NullValueHandling = NullValueHandling.Ignore)]
        public double? Maximum
        {
            get => this.maximum;
            init
            {
                this.maximum = value;
                
                if (!this.ValidateMaximum(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        /// <summary>
        /// Greater than
        /// </summary>
        [JsonProperty("exclusiveMinimum", NullValueHandling = NullValueHandling.Ignore)]
        public double? ExclusiveMinimum
        {
            get => this.exclusiveMinimum;
            init
            {
                this.exclusiveMinimum = value;
                
                if (!this.ValidateExclusiveMinimum(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        /// <summary>
        /// Less than or equal
        /// </summary>
        [JsonProperty("exclusiveMaximum", NullValueHandling = NullValueHandling.Ignore)]
        public double? ExclusiveMaximum
        {
            get => this.exclusiveMaximum;
            init
            {
                this.exclusiveMaximum = value;
                
                if (!this.ValidateExclusiveMaximum(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("isUnique", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsUnique { get; set; }
        
        #endregion

        #region Methods

        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateMinimum(out exception);
            this.ValidateMaximum(out exception);
            this.ValidateExclusiveMinimum(out exception);
            this.ValidateExclusiveMaximum(out exception);

            return exception == null;
        }
        
        public override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);
            
            if (obj is double doubleValue)
            {
                if (this.Maximum != null && doubleValue > this.Maximum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be greater than {this.Maximum}", this));
                }
                
                if (this.Minimum != null && doubleValue < this.Minimum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be less than {this.Minimum}", this));
                }
                
                if (this.ExclusiveMaximum != null && doubleValue >= this.ExclusiveMaximum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be greater than or equal {this.ExclusiveMaximum}", this));
                }
                
                if (this.ExclusiveMinimum != null && doubleValue < this.ExclusiveMinimum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be less than or equal {this.ExclusiveMinimum}", this));
                }
            }
            
            return isValid;
        }
        
        private bool ValidateMinimum(out Exception exception)
        {
            if (this.Maximum != null && this.Minimum != null && this.Maximum < this.Minimum)
            {
                exception = new FieldValidationException($"The 'minimum' value can not be greater than the 'maximum' value ({this.Name})", this);
                return false;
            }

            if (this.ExclusiveMaximum != null && this.Minimum != null && this.ExclusiveMaximum < this.Minimum)
            {
                exception = new FieldValidationException($"The 'minimum' value can not be greater than the 'exclusiveMaximum' value ({this.Name})", this);
                return false;
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateMaximum(out Exception exception)
        {
            if (this.Minimum != null && this.Maximum != null && this.Minimum > this.Maximum)
            {
                exception = new FieldValidationException($"The 'maximum' value can not be less than the 'minimum' value ({this.Name})", this);
                return false;
            }

            if (this.ExclusiveMinimum != null && this.Maximum != null && this.ExclusiveMinimum > this.Maximum)
            {
                exception = new FieldValidationException($"The 'maximum' value can not be less than the 'exclusiveMinimum' value ({this.Name})", this);
                return false;
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateExclusiveMinimum(out Exception exception)
        {
            if (this.Maximum != null && this.ExclusiveMinimum != null && this.Maximum < this.ExclusiveMinimum)
            {
                exception = new FieldValidationException($"The 'exclusiveMinimum' value can not be greater than the 'maximum' value ({this.Name})", this);
                return false;
            }

            if (this.ExclusiveMaximum != null && this.ExclusiveMinimum != null && this.ExclusiveMaximum < this.ExclusiveMinimum)
            {
                exception = new FieldValidationException($"The 'exclusiveMinimum' value can not be greater than the 'exclusiveMaximum' value ({this.Name})", this);
                return false;
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateExclusiveMaximum(out Exception exception)
        {
            if (this.Minimum != null && this.ExclusiveMaximum != null && this.Minimum > this.ExclusiveMaximum)
            {
                exception = new FieldValidationException($"The 'exclusiveMaximum' value can not be less than the 'minimum' value ({this.Name})", this);
                return false;
            }

            if (this.ExclusiveMinimum != null && this.ExclusiveMaximum != null && this.ExclusiveMinimum > this.ExclusiveMaximum)
            {
                exception = new FieldValidationException($"The 'exclusiveMaximum' value can not be less than the 'exclusiveMinimum' value ({this.Name})", this);
                return false;
            }
            
            exception = null;
            return true;
        }

        public override object Clone()
        {
            return new FloatFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                IsUnique = this.IsUnique,
                IsVirtual = this.IsVirtual,
                IsHidden = this.IsHidden,
                DefaultValue = this.DefaultValue,
                Minimum = this.Minimum,
                Maximum = this.Maximum,
                ExclusiveMinimum = this.ExclusiveMinimum,
                ExclusiveMaximum = this.ExclusiveMaximum,
            };
        }
        
        #endregion
    }
}