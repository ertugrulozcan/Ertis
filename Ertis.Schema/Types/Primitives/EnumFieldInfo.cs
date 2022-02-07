using System;
using System.Linq;
using Ertis.Schema.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class EnumFieldInfo : FieldInfo<object>
    {
        #region Fields

        private object[] items;

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override PrimitiveType Type => PrimitiveType.@enum;
        
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
        
        #endregion

        #region Methods

        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateItems(out exception);
            
            return exception == null;
        }

        protected override void Validate(object obj)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                if (!type.IsPrimitive && type != typeof(string))
                {
                    throw new FieldValidationException($"Enum value is must be primitive type ({this.Name})", this);   
                }
                
                if (this.Items.All(x => !x.Equals(obj)))
                {
                    throw new FieldValidationException($"The value does not exist in the enum items. The '{this.Name}' value must be one of them [{string.Join(", ", this.Items)}]", this);
                }
            }
            else
            {
                if (this.Items.All(x => x != null))
                {
                    throw new FieldValidationException($"The value does not exist in the enum items. The '{this.Name}' value must be one of them [{string.Join(", ", this.Items)}]", this);
                }
            }
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

        #endregion
    }
}