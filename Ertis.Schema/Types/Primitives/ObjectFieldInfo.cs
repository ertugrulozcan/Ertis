using System.Collections.ObjectModel;
using System.Dynamic;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Serialization.Legacy;
using Ertis.Schema.Validation;

using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;
using NewtonsoftJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;
using NewtonsoftJsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NewtonsoftJsonConverter = Newtonsoft.Json.JsonConverterAttribute;

namespace Ertis.Schema.Types.Primitives;

public abstract class ObjectFieldInfoBase : FieldInfo<object>, ISchema
{
	#region Properties
	
	[JsonIgnore]
	[NewtonsoftJsonIgnore]
	public string Slug => this.Name;
	
	[JsonPropertyName("allowAdditionalProperties")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[NewtonsoftJsonProperty("allowAdditionalProperties", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
	public bool AllowAdditionalProperties { get; init; }
	
	#endregion
	
	#region Abstract Properties
	
	[JsonPropertyName("properties")]
	[JsonConverter(typeof(FieldInfoCollectionJsonConverterFactory))]
	[NewtonsoftJsonProperty("properties")]
	[NewtonsoftJsonConverter(typeof(FieldInfoCollectionJsonConverter))]
	public abstract IReadOnlyCollection<IFieldInfo> Properties { get; init; }
	
	#endregion
	
	#region Methods
	
	public bool ValidateContent(DynamicObject obj, IValidationContext validationContext)
	{
		using (ValidationPath.Begin(this.Path))
		{
			return this.Validate(obj.ToDynamic(), validationContext);
		}
	}
	
	#endregion
}

public sealed class ObjectFieldInfo : ObjectFieldInfoBase
{
	#region Fields
	
	/// <summary>
	/// A root object created to validate the data of a schema; it doesn't own (re-parent) the shared properties of the schema
	/// </summary>
	private readonly bool _isDetachedRoot;
	
	#endregion
	
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[NewtonsoftJsonProperty("type")]
	[NewtonsoftJsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public override FieldType Type => FieldType.@object;
	
	[JsonPropertyName("properties")]
	[JsonConverter(typeof(FieldInfoCollectionJsonConverterFactory))]
	[NewtonsoftJsonProperty("properties")]
	[NewtonsoftJsonConverter(typeof(FieldInfoCollectionJsonConverter))]
	public override IReadOnlyCollection<IFieldInfo> Properties
	{
		get;
		init
		{
			field = value;
			if (this._isDetachedRoot)
			{
				return;
			}
			
			foreach (var fieldInfo in value)
			{
				fieldInfo.Parent = this;
			}
			
			if (!this.ValidateProperties(out var exception) && exception != null)
			{
				throw exception;
			}
		}
	}

	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	public ObjectFieldInfo()
	{
		this.Properties = new ReadOnlyCollection<IFieldInfo>(new List<IFieldInfo>());
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="properties"></param>
	public ObjectFieldInfo(IEnumerable<IFieldInfo> properties)
	{
		this.Properties = new ReadOnlyCollection<IFieldInfo>(properties.ToList());
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="schema"></param>
	private ObjectFieldInfo(ISchema schema)
	{
		this._isDetachedRoot = true;
		this.Properties = schema.Properties;
	}
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Creates a root object to validate the data of the schema, without changing the parents of the schema properties
	/// </summary>
	/// <param name="schema"></param>
	/// <returns></returns>
	internal static ObjectFieldInfo CreateDetachedRoot(ISchema schema)
	{
		return new ObjectFieldInfo(schema)
		{
			Name = schema.Slug,
			AllowAdditionalProperties = schema.AllowAdditionalProperties
		};
	}
	
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
					using (ValidationPath.Push(propertyName))
					{
						isValid &= ((FieldInfo) fieldInfo).Validate(propertyValue, validationContext);
					}
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
					using (ValidationPath.Push(fieldInfo.Name))
					{
						isValid &= ((FieldInfo) fieldInfo).Validate(null, validationContext);
					}
				}
			}   
		}
		
		return isValid;
	}
	
	public override object Clone()
	{
		return new ObjectFieldInfo(this.Properties.Select(x => (IFieldInfo)x.Clone()))
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