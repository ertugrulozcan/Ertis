using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class ConstantFieldInfo : FieldInfo<object>
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.@const;
        
        [JsonProperty("value")]
        public object Value { get; set; }
        
        [JsonProperty("valueType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConstantType ValueType { get; set; }
        
        #endregion
        
        #region Methods
        
        protected internal override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);
            
            if (obj != null)
            {
                bool incompatibleType;
                var type = obj.GetType();
                switch (this.ValueType)
                {
                    case ConstantType.@string:
                        incompatibleType = type != typeof(string);
                        break;
                    case ConstantType.integer:
                        incompatibleType = !int.TryParse(obj.ToString(), out _);
                        break;
                    case ConstantType.@float:
                        incompatibleType = !double.TryParse(obj.ToString(), out _);
                        break;
                    case ConstantType.boolean:
                        incompatibleType = !bool.TryParse(obj.ToString(), out _);
                        break;
                    case ConstantType.date:
                    case ConstantType.datetime:
                        incompatibleType = !DateTime.TryParse(obj.ToString(), out _);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (incompatibleType)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Constant value is must be {this.ValueType}", this));
                }
            }

            return isValid;
        }

        public override object Clone()
        {
            return new ConstantFieldInfo
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
                Value = this.Value,
                ValueType = this.ValueType
            };
        }

        #endregion

        #region Enums

        public enum ConstantType
        {
            @string,
            integer,
            @float,
            boolean,
            date,
            datetime
        }

        #endregion
    }
}