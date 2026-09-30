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
	
	/// <summary>
	/// The properties by name (built on the first validation; the properties can not change after the initialization)
	/// </summary>
	private Dictionary<string, FieldInfo>? _propertyLookup;
	
	/// <summary>
	/// Whether the object accepts the properties which are not declared in its schema
	/// </summary>
	protected virtual bool AcceptsAdditionalProperties => this.AllowAdditionalProperties;
	
	/// <summary>
	/// Whether the value is an array of objects instead of a single object
	/// </summary>
	protected virtual bool IsMultiple => false;
	
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
			return this.Validate(obj.ToDictionary(), validationContext);
		}
	}
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		var isValid = base.Validate(obj, validationContext);
		
		switch (obj)
		{
			case null:
				break;
			case IDictionary<string, object?> dictionary when !this.IsMultiple:
				isValid &= this.ValidateObject(dictionary, validationContext);
				break;
			case object?[] array when this.IsMultiple:
			{
				for (var i = 0; i < array.Length; i++)
				{
					using (ValidationPath.PushIndex(i))
					{
						if (array[i] is IDictionary<string, object?> item)
						{
							isValid &= this.ValidateObject(item, validationContext);
						}
						else
						{
							isValid = false;
							validationContext.Errors.Add(new FieldValidationException("Type mismatch error. Array items are must be 'object'", this));
						}
					}
				}
				
				break;
			}
			default:
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException($"Type mismatch error. '{this.Name}' is must be '{(this.IsMultiple ? "array" : "object")}'", this));
				break;
		}
		
		return isValid;
	}
	
	private Dictionary<string, FieldInfo> CreatePropertyLookup()
	{
		var propertyLookup = new Dictionary<string, FieldInfo>(this.Properties.Count);
		foreach (var fieldInfo in this.Properties)
		{
			// The first one wins like a lookup in the list would do (the duplicate names are rejected by the schema validation)
			propertyLookup.TryAdd(fieldInfo.Name, (FieldInfo) fieldInfo);
		}
		
		return propertyLookup;
	}
	
	private bool ValidateObject(IDictionary<string, object?> dictionary, IValidationContext validationContext)
	{
		var isValid = true;
		var propertyLookup = this._propertyLookup ??= this.CreatePropertyLookup();
		foreach (var (propertyName, propertyValue) in dictionary)
		{
			if (propertyLookup.TryGetValue(propertyName, out var fieldInfo))
			{
				using (ValidationPath.Push(propertyName))
				{
					isValid &= fieldInfo.Validate(propertyValue, validationContext);
				}
			}
			else if (!this.AcceptsAdditionalProperties)
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException($"Additional properties not allowed in this object schema. ({propertyName})", this));
			}
		}
		
		foreach (var fieldInfo in this.Properties)
		{
			if (!dictionary.ContainsKey(fieldInfo.Name))
			{
				using (ValidationPath.Push(fieldInfo.Name))
				{
					isValid &= ((FieldInfo) fieldInfo).Validate(null, validationContext);
				}
			}
		}
		
		return isValid;
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
		return base.ValidateSchema(out exception) &&
			this.ValidateProperties(out exception);
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
			IsSearchable = this.IsSearchable,
			SearchWeight = this.SearchWeight,
			Appearance = this.Appearance,
			DefaultValue = this.DefaultValue,
			AllowAdditionalProperties = this.AllowAdditionalProperties
		};
	}
	
	#endregion
}