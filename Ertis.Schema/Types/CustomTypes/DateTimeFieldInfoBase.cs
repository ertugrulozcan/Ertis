using System.Globalization;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.CustomTypes;

public interface IDateTimeFieldInfo
{
	#region Properties
	
	[JsonPropertyName("minValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("minValue", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public DateTime? MinValue { get; init; }
	
	[JsonPropertyName("maxValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("maxValue", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public DateTime? MaxValue { get; init; }
	
	#endregion
}

public abstract class DateTimeFieldInfoBase : StringFieldInfo, IDateTimeFieldInfo
{
	#region Abstract Properties
	
	[JsonIgnore]
	[Newtonsoft.Json.JsonIgnore]
	protected abstract string StringFormat { get; }
	
	/// <summary>
	/// The formats accepted in the string values (StringFormat by default)
	/// </summary>
	[JsonIgnore]
	[Newtonsoft.Json.JsonIgnore]
	protected virtual string[] AcceptedFormats => [this.StringFormat];
	
	#endregion
	
	#region Properties
	
	[JsonPropertyName("minValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("minValue", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public DateTime? MinValue
	{
		get;
		init
		{
			field = value;
			
			if (!this.ValidateMinValue(out var exception) && exception != null)
			{
				throw exception;
			}
		}
	}
	
	[JsonPropertyName("maxValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[Newtonsoft.Json.JsonProperty("maxValue", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
	public DateTime? MaxValue
	{
		get;
		init
		{
			field = value;
			
			if (!this.ValidateMaxValue(out var exception) && exception != null)
			{
				throw exception;
			}
		}
	}
	
	#endregion
	
	#region Methods
	
	public override bool ValidateSchema(out Exception? exception)
	{
		return base.ValidateSchema(out exception) &&
			this.ValidateMinValue(out exception) &&
			this.ValidateMaxValue(out exception);
	}
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		var isValid = base.Validate(obj, validationContext);
		
		if (obj is string && !this.TryGetUtcDateTime(obj, out _))
		{
			isValid = false;
			validationContext.Errors.Add(new FieldValidationException($"Datetime is not valid. Datetime values must be '{this.StringFormat}' format.", this));
		}
		else if (this.TryGetUtcDateTime(obj, out var dateTime))
		{
			if (this.MaxValue != null && dateTime > ToUtc(this.MaxValue.Value))
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException($"Date can not be greater than {FormatBound(this.MaxValue.Value)}", this));
			}
			
			if (this.MinValue != null && dateTime < ToUtc(this.MinValue.Value))
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException($"Date can not be less than {FormatBound(this.MinValue.Value)}", this));
			}
		}
		
		return isValid;
	}
	
	/// <summary>
	/// Gets the value as a UTC date time; the strings are parsed by the accepted formats (a value without an offset is UTC)
	/// </summary>
	internal bool TryGetUtcDateTime(object? value, out DateTime dateTime)
	{
		switch (value)
		{
			case DateTime dateTimeValue:
				dateTime = ToUtc(dateTimeValue);
				return true;
			case DateTimeOffset dateTimeOffset:
				dateTime = dateTimeOffset.UtcDateTime;
				return true;
			case string text when !string.IsNullOrWhiteSpace(text):
				return DateTime.TryParseExact(text, this.AcceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime);
			default:
				dateTime = default;
				return false;
		}
	}
	
	/// <summary>
	/// Normalizes the date time to UTC (a date time without a kind is UTC)
	/// </summary>
	private static DateTime ToUtc(DateTime dateTime)
	{
		return dateTime.Kind switch
		{
			DateTimeKind.Utc => dateTime,
			DateTimeKind.Local => dateTime.ToUniversalTime(),
			_ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
		};
	}
	
	private string FormatBound(DateTime dateTime)
	{
		return ToUtc(dateTime).ToString(this.StringFormat, CultureInfo.InvariantCulture);
	}
	
	private bool ValidateMinValue(out Exception? exception)
	{
		if (this.MinValue != null)
		{
			if (this.MaxValue != null && this.MinValue != null && this.MaxValue < this.MinValue)
			{
				exception = new FieldValidationException($"The 'minValue' value can not be greater than the 'maxValue' value ('{this.Name}')", this);
				return false;
			}
		}
		
		exception = null;
		return true;
	}
	
	private bool ValidateMaxValue(out Exception? exception)
	{
		if (this.MaxValue != null)
		{
			if (this.MinValue != null && this.MaxValue != null && this.MinValue > this.MaxValue)
			{
				exception = new FieldValidationException($"The 'minValue' value can not be greater than the 'maxValue' value ('{this.Name}')", this);
				return false;
			}
		}
		
		exception = null;
		return true;
	}
	
	#endregion
}