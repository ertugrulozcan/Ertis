using System.Collections.ObjectModel;
using System.Dynamic;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Validation;
using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Types.Primitives;

public abstract class ObjectFieldInfoBase : FieldInfo<object>, ISchema
{
    #region Properties
    
    [JsonIgnore]
    public string Slug => this.Name;
    
    [JsonPropertyName("allowAdditionalProperties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool AllowAdditionalProperties { get; init; }
    
    #endregion
    
    #region Abstract Properties
    
    public abstract IReadOnlyCollection<IFieldInfo> Properties { get; init; }
    
    #endregion
    
    #region Methods
    
    public bool ValidateContent(DynamicObject obj, IValidationContext validationContext)
    {
        return this.Validate(obj.ToDynamic(), validationContext);
    }
    
    #endregion
}

public sealed class ObjectFieldInfo : ObjectFieldInfoBase
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override FieldType Type => FieldType.@object;
    
    [JsonPropertyName("properties")]
    public override IReadOnlyCollection<IFieldInfo> Properties
    {
        get;
        init
        {
            field = value;
            foreach (var fieldInfo in value)
            {
                fieldInfo.Parent = this;
            }
            
            if (!this.ValidateProperties(out var exception))
            {
                throw exception!;
            }
        }
    }
    
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
    
    public override bool ValidateSchema(out Exception? exception)
    {
        base.ValidateSchema(out exception);
        this.Validate(out exception);
        
        return exception == null;
    }
    
    protected internal override bool Validate(object? obj, IValidationContext validationContext)
    {
        var isValid = base.Validate(obj, validationContext);
        
        if (obj != null)
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
        return new ObjectFieldInfo(this.Properties.Select(x => (IFieldInfo) x.Clone()))
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
            AllowAdditionalProperties = this.AllowAdditionalProperties 
        };
    }
    
    #endregion
}