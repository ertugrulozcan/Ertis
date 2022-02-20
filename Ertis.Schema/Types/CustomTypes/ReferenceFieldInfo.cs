using System;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class ReferenceFieldInfo : FieldInfo
    {
        #region Enums

        public enum ReferenceTypes
        {
            single,
            multiple,
            // dataset,
            // querydata, // (single)
        }

        #endregion

        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.reference;
        
        [JsonProperty("referenceType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ReferenceTypes ReferenceType { get; set; }
        
        [JsonProperty("contentType", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ContentType { get; set; }

        #endregion

        #region Methods

        protected override bool ValidateSchemaCore(out Exception exception)
        {
            exception = null;
            return true;
        }

        protected override bool ValidateCore(object obj, IValidationContext validationContext)
        {
            var isValid = true;
            
            if (this.ReferenceType == ReferenceTypes.single && obj is string referenceId)
            {
                
            }
            else if (this.ReferenceType == ReferenceTypes.multiple && obj is object[] objectArray && objectArray.All(x => x is string))
            {
                
            }
            else
            {
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"Invalid reference value on '{this.Name}' field", this));
            }
            
            return isValid;
        }
        
        public override object GetDefaultValue()
        {
            return null;
        }

        public override object Clone()
        {
            return new ReferenceFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                ContentType = this.ContentType,
                IsRequired = this.IsRequired,
                IsVirtual = this.IsVirtual,
                ReferenceType = this.ReferenceType,
            };
        }

        #endregion
    }
}