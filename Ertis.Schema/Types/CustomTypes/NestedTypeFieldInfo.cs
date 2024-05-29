using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Types.CustomTypes;

public sealed class NestedTypeFieldInfo : ObjectFieldInfoBase
{
    #region Fields

    private readonly IReadOnlyCollection<IFieldInfo> properties;

    #endregion
    
    #region Properties

    [JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    [JsonPropertyName("type")]
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public override FieldType Type => FieldType.nestedType;

    [JsonProperty("properties")]
    [JsonPropertyName("properties")]
    [Newtonsoft.Json.JsonConverter(typeof(FieldInfoCollectionJsonConverter))]
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
    
    [JsonProperty("nestedTypeId")]
    [JsonPropertyName("nestedTypeId")]
    public string NestedTypeId { get; set; }

    #endregion
    
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="properties"></param>
    public NestedTypeFieldInfo(IEnumerable<IFieldInfo> properties)
    {
        this.Properties = new ReadOnlyCollection<IFieldInfo>(properties.ToList());
    }

    #endregion

    #region Methods
    
    public override bool ValidateSchema(out Exception exception)
    {
        base.ValidateSchema(out exception);
        this.Validate(out exception);
        
        return exception == null;
    }
    
    protected internal override bool Validate(object obj, IValidationContext validationContext)
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
    
    public override object Clone()
    {
        return new NestedTypeFieldInfo(this.Properties.Select(x => x.Clone() as IFieldInfo))
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
            AllowAdditionalProperties = this.AllowAdditionalProperties,
            NestedTypeId = this.NestedTypeId, 
            // Properties ** From Constructor
        };
    }

    #endregion
}