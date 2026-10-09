using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Schema.Types.Primitives;

public class EnumFieldInfo : FieldInfo<object>, IPrimitiveType
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.@enum;
	
	[JsonPropertyName("items")]
	public EnumItem[] Items
	{
		get => field ?? Array.Empty<EnumItem>();
		set
		{
			field = value;
			
			if (!this.ValidateItems(out var exception) && exception != null)
			{
				throw exception;
			}
		}
	}
	
	[JsonPropertyName("isUnique")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsUnique { get; set; }
	
	[JsonPropertyName("isMultiple")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsMultiple { get; set; }
	
	#endregion
	
	#region Methods
	
	public override bool ValidateSchema(out Exception? exception)
	{
		return base.ValidateSchema(out exception) &&
			this.ValidateItems(out exception);
	}
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		var isValid = base.Validate(obj, validationContext);
		
		var isExistInEnums = false;
		if (obj != null)
		{
			if (this.IsMultiple)
			{
				if (obj is object[] array)
				{
					isExistInEnums = array.All(item => this.Items.Any(x => x.Value.Equals(item)));
					if (array.Distinct().Count() != array.Length)
					{
						isValid = false;
						validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' values must be unique", this));
					}
				}
				else
				{
					isValid = false;
					validationContext.Errors.Add(new FieldValidationException($"Enum value is must be array type ({this.Name})", this));   
				}
			}
			else
			{
				var type = obj.GetType();
				if (type.IsPrimitive || type == typeof(string))
				{
					isExistInEnums = this.Items.Any(x => x.Value.Equals(obj));
				}
				else
				{
					isValid = false;
					validationContext.Errors.Add(new FieldValidationException($"Enum value is must be primitive type ({this.Name})", this));   
				}
			}
		}
		
		if (isValid && obj != null && !isExistInEnums)
		{
			isValid = false;
			var enumValues = string.Join(", ", this.Items.Select(x => $"'{x.Value}'"));
			validationContext.Errors.Add(new FieldValidationException($"The value does not exist in the enum items. The '{this.Name}' value must be one of them [{enumValues}]", this));   
		}
		
		return isValid;
	}
	
	private bool ValidateItems(out Exception? exception)
	{
		if (this.Items.Length == 0)
		{
			exception = new FieldValidationException("Enum items can not be empty", this);
			return false;
		}
		
		var uniqueCount = this.Items.Select(x => x.Value).Distinct().Count();
		if (this.Items.Length != uniqueCount)
		{
			exception = new FieldValidationException("Enum items must be unique", this);
			return false;
		}
		
		exception = null;
		return true;
	}
	
	#endregion
	
	#region Helper Classes
	
	public class EnumItem
	{
		#region Properties
		
		[JsonPropertyName("displayName")]
		public required string DisplayName { get; set; }
		
		[JsonPropertyName("value")]
		public required string Value { get; set; }
		
		#endregion
	}
	
	#endregion
}