using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Serialization;
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

        public abstract IReadOnlyCollection<IFieldInfo> Properties { get; init; }

        #endregion

        #region Abstract Methods

        public bool ValidateContent(object obj, out Exception exception)
        {
            return this.IsValid(obj, out exception);
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
        
        protected override void Validate(object obj)
        {
            base.Validate(obj);

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
                        if (!fieldInfo.IsValid(propertyValue, out var ex))
                        {
                            throw ex;
                        }
                    }
                    else if (!this.AllowAdditionalProperties)
                    {
                        throw new FieldValidationException($"Additional properties not allowed in this object schema. ({propertyName})", this);
                    }
                
                    validatedProperties.Add(propertyName);
                }

                foreach (var fieldInfo in this.Properties)
                {
                    if (!validatedProperties.Contains(fieldInfo.Name))
                    {
                        if (!fieldInfo.IsValid(null, out var ex))
                        {
                            throw ex;
                        }
                    }
                }   
            }
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
            
            return exception == null;
        }

        #endregion
    }
}