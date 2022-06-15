using System;
using System.Globalization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;

namespace Ertis.Schema.Types.CustomTypes
{
	public interface IDateTimeFieldInfo
    {
        #region Properties

        [JsonProperty("minValue", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? MinValue { get; init; }

        [JsonProperty("maxValue", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? MaxValue { get; init; }
        
        #endregion
    }
    
    public abstract class DateTimeFieldInfoBase<T> : StringFieldInfo, IDateTimeFieldInfo where T : StringFieldInfo, IDateTimeFieldInfo, new()
    {
        #region Fields

        private readonly DateTime? minValue;
        private readonly DateTime? maxValue;

        #endregion
        
        #region Abstract Properties
        
        [JsonIgnore]
        protected abstract string StringFormat { get; }
        
        #endregion
        
        #region Properties
        
        [JsonProperty("minValue", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? MinValue
        {
            get => this.minValue;
            init
            {
                this.minValue = value;
                
                if (!this.ValidateMinValue(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("maxValue", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? MaxValue
        {
            get => this.maxValue;
            init
            {
                this.maxValue = value;
                
                if (!this.ValidateMaxValue(out var exception))
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
            this.ValidateMinValue(out exception);
            this.ValidateMaxValue(out exception);

            return exception == null;
        }
        
        protected internal override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);

            DateTime? dateTime = null;
            switch (obj)
            {
                case DateTime _dateTime:
                    dateTime = _dateTime;
                    break;
                case string dateStr:
                {
                    if (!IsValidDateTime(dateStr, out dateTime))
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"Datetime is not valid. Datetime values must be '{this.StringFormat}' format.", this));
                    }

                    break;
                }
            }

            if (dateTime != null)
            {
                if (this.MaxValue != null && dateTime.Value > this.MaxValue.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Date can not be greater than {this.MaxValue}", this));
                }
                
                if (this.MinValue != null && dateTime.Value < this.MinValue.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Date can not be less than {this.MinValue}", this));
                }
            }
            
            return isValid;
        }
        
        private bool ValidateMinValue(out Exception exception)
        {
            if (this.MinValue != null)
            {
                if (this.MaxValue != null && this.MinValue != null && this.MaxValue < this.MinValue)
                {
                    exception = new FieldValidationException($"The 'minValue' value can not be greater than the 'maxValue' value ('{this.Name}')", this);
                    return false;
                }
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateMaxValue(out Exception exception)
        {
            if (this.MaxValue != null)
            {
                if (this.MinValue != null && this.MaxValue != null && this.MinValue > this.MaxValue)
                {
                    exception = new FieldValidationException($"The 'minValue' value can not be greater than the 'maxValue' value ('{this.Name}')", this);
                    return false;
                }
            }
            
            exception = null;
            return true;
        }
        
        private bool IsValidDateTime(string dateString, out DateTime? dateTime)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrWhiteSpace(dateString))
            {
                dateTime = null;
                return false;
            }

            var isValid = DateTime.TryParseExact(dateString, this.StringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _dateTime);
            dateTime = isValid ? _dateTime : null;
            
            return isValid;
        }

        public override object Clone()
        {
            return new T
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
                MinValue = this.MinValue,
                MaxValue = this.MaxValue,
                MinLength = this.MinLength,
                MaxLength = this.MaxLength,
                FormatPattern = this.FormatPattern,
                RegexPattern = this.RegexPattern,
                RestrictRegexPattern = this.RestrictRegexPattern,
            };
        }

        #endregion
    }
}