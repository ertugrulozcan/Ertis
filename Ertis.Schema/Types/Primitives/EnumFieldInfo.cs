using System;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class EnumFieldInfo : FieldInfo<object>, IPrimitiveType
    {
        #region Fields

        private object[] items;

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.@enum;
        
        [JsonProperty("items")]
        public object[] Items
        {
            get => this.items;
            set
            {
                this.items = value;
                
                if (!this.ValidateItems(out var exception))
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
            this.ValidateItems(out exception);
            
            return exception == null;
        }

        protected internal override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);
            
            bool isExistInEnums;
            if (obj != null)
            {
                var type = obj.GetType();
                if (!type.IsPrimitive && type != typeof(string))
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Enum value is must be primitive type ({this.Name})", this));   
                }

                isExistInEnums = this.Items.Any(x => x != null && x.Equals(obj));
            }
            else
            {
                isExistInEnums = this.Items.Any(x => x == null);
            }

            if (!isExistInEnums)
            {
                isValid = false;
                var enumValues = string.Join(", ", this.Items.Select(x => x == null ? "null" : (x is string ? $"'{x}'" : x.ToString())));
                validationContext.Errors.Add(new FieldValidationException($"The value does not exist in the enum items. The '{this.Name}' value must be one of them [{enumValues}]", this));   
            }
            
            return isValid;
        }

        private bool ValidateItems(out Exception exception)
        {
            if (this.Items == null)
            {
                exception = new FieldValidationException($"Enum items is required ('{this.Name}')", this);
                return false;
            }
            
            if (this.Items.Length == 0)
            {
                throw new FieldValidationException("Enum items can not be empty", this);
            }
            
            if (this.Items.Any(x => x != null && !x.GetType().IsPrimitive && x.GetType() != typeof(string)))
            {
                throw new FieldValidationException("Enum items must be primitive type", this);
            }
            
            var uniqueCount = this.Items.Distinct().Count();
            if (this.Items.Length != uniqueCount)
            {
                throw new FieldValidationException("Enum items must be unique", this);
            }

            exception = null;
            return true;
        }

        public override object Clone()
        {
            return new EnumFieldInfo
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
                Items = this.Items,
            };
        }

        #endregion
    }
}