using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Types.Primitives
{
    public abstract class ObjectFieldInfoBase : FieldInfo<object>, ISchema
    {
        #region Properties

        [JsonIgnore]
        public string Slug => this.Name;

        #endregion

        #region Abstract Properties

        public abstract IReadOnlyCollection<IFieldInfo> Properties { get; init; }

        #endregion

        #region Abstract Methods

        public bool ValidateContent(object obj, IValidationContext validationContext)
        {
            return this.Validate(obj, validationContext);
        }

        #endregion
    }
    
    public sealed class ObjectFieldInfo : ObjectFieldInfoBase
    {
        #region Fields

        private readonly IReadOnlyCollection<IFieldInfo> properties;

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.@object;

        [JsonProperty("properties")]
        [JsonConverter(typeof(FieldInfoCollectionJsonConverter))]
        public override IReadOnlyCollection<IFieldInfo> Properties
        {
            get => this.properties;
            init
            {
                this.properties = value;
                if (value != null)
                {
                    foreach (var fieldInfo in value)
                    {
                        fieldInfo.Parent = this;
                    }   
                }

                if (!this.ValidateProperties(out var exception))
                {
                    throw exception;
                }
            }
        }

        [JsonProperty("allowAdditionalProperties", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AllowAdditionalProperties { get; init; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="properties"></param>
        public ObjectFieldInfo(IEnumerable<IFieldInfo> properties)
        {
            this.Properties = new ReadOnlyCollection<IFieldInfo>(properties.ToList());
        }

        #endregion

        #region Methods
        
        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateProperties(out exception);
            
            return exception == null;
        }
        
        public override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);

            if (obj != null && this.Properties != null)
            {
                DynamicObject dynamicObject;
                if (obj is ExpandoObject expandoObject)
                {
                    dynamicObject = DynamicObject.Create(expandoObject.ToDictionary());
                }
                else
                {
                    dynamicObject = new DynamicObject(obj);
                }
                
                var validatedProperties = new List<string>();
                foreach (var (propertyName, propertyValue) in dynamicObject.ToDictionary())
                {
                    var fieldInfo = this.Properties.FirstOrDefault(x => x.Name == propertyName);
                    if (fieldInfo != null)
                    {
                        isValid &= ((FieldInfo) fieldInfo).Validate(propertyValue, validationContext);
                    }
                    else if (!this.AllowAdditionalProperties)
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"Additional properties not allowed in this object schema. ({propertyName})", this));
                    }
                
                    validatedProperties.Add(propertyName);
                }

                foreach (var fieldInfo in this.Properties)
                {
                    if (!validatedProperties.Contains(fieldInfo.Name))
                    {
                        isValid &= ((FieldInfo) fieldInfo).Validate(null, validationContext);
                    }
                }   
            }
            
            return isValid;
        }
        
        private bool ValidateProperties(out Exception exception)
        {
            if (this.Properties == null)
            {
                exception = new FieldValidationException($"Schema properties is required for nested objects ('{this.Name}')", this);
                return false;
            }

            foreach (var fieldInfo in this.Properties)
            {
                if (!fieldInfo.ValidateSchema(out exception))
                {
                    return false;
                }
            }
            
            this.CheckPropertiesUniqueness(out exception);
            
            var uniqueProperties = this.GetUniqueProperties();
            foreach (var uniqueProperty in uniqueProperties)
            {
                if (uniqueProperty.IsAnArrayItem(out _))
                {
                    exception = new SchemaValidationException($"The unique constraints could not use in arrays. Use the 'uniqueBy' feature instead of. ('{uniqueProperty.Name}')");
                    
                    return false;
                }
            }
            
            return exception == null;
        }

        public override object Clone()
        {
            return new ObjectFieldInfo(this.Properties.Select(x => x.Clone() as IFieldInfo))
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                DefaultValue = this.DefaultValue,
                AllowAdditionalProperties = this.AllowAdditionalProperties
            };
        }

        #endregion
    }
}