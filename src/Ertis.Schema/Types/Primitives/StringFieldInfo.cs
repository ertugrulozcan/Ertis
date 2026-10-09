using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Ertis.Schema.Types.Primitives;

public class StringFieldInfo : FieldInfo<string>, IPrimitiveType
{
	#region Constants
	
	private const string OPEN_FORMAT_BRACKETS = "{";
	private const string CLOSE_FORMAT_BRACKETS = "}";
	
	#endregion
	
	#region Fields
	
	private readonly Regex? _regex;
	private readonly Regex? _restrictRegex;
	
	#endregion
	
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.@string;
	
	[JsonPropertyName("minLength")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public int? MinLength
	{
		get;
		init
		{
			field = value;
			
			if (!this.ValidateMinLength(out var exception) && exception != null)
			{
				throw exception;
			}
		}
	}
	
	[JsonPropertyName("maxLength")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public int? MaxLength
	{
		get;
		init
		{
			field = value;
			
			if (!this.ValidateMaxLength(out var exception) && exception != null)
			{
				throw exception;
			}
		}
	}
	
	[JsonPropertyName("formatPattern")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string? FormatPattern
	{
		get;
		init
		{
			field = value;
			
			if (!this.ValidateFormatPattern(out var exception) && exception != null)
			{
				throw exception;
			}
			
			this.OnPropertyChanged(nameof(this.FormatPattern));
		}
	}
	
	[JsonPropertyName("regexPattern")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string? RegexPattern
	{
		get;
		init
		{
			field = value;
			this._regex = this.CreateRegex(value, "regexPattern");
		}
	}
	
	[JsonPropertyName("restrictRegexPattern")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string? RestrictRegexPattern
	{
		get;
		init
		{
			field = value;
			this._restrictRegex = this.CreateRegex(value, "restrictRegexPattern");
		}
	}
	
	[JsonPropertyName("caseInsensitive")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool CaseInsensitive { get; init; }
	
	[JsonPropertyName("isUnique")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsUnique { get; set; }
	
	#endregion
	
	#region Methods
	
	public override bool ValidateSchema(out Exception? exception)
	{
		return base.ValidateSchema(out exception) &&
			this.ValidateMinLength(out exception) &&
			this.ValidateMaxLength(out exception) &&
			this.ValidateFormatPattern(out exception);
	}
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		if (!string.IsNullOrEmpty(this.FormatPattern))
		{
			if (this.TryFormat(validationContext.Content, out var formattedString))
			{
				obj = formattedString;
			}
		}
		
		var isValid = base.Validate(obj, validationContext);
		
		if (obj is string text)
		{
			if (this.IsRequired && string.IsNullOrEmpty(text.Trim()))
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException($"{this.Name} is required", this));
			}
			
			if (this.MaxLength != null && text.Length > this.MaxLength.Value)
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException($"String length can not be greater than {this.MaxLength}", this));
			}
			
			if (this.MinLength != null && text.Length < this.MinLength.Value)
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException($"String length can not be less than {this.MinLength}", this));
			}
			
			if (!string.IsNullOrEmpty(text) && this._regex != null && this.UsesRegexPattern)
			{
				if (!this._regex.IsMatch(text))
				{
					isValid = false;
					validationContext.Errors.Add(new FieldValidationException($"String value is not valid by the regular expression rule. ('{this.RegexPattern}')", this));
				}
			}
			
			if (!string.IsNullOrEmpty(text) && this._restrictRegex != null)
			{
				if (this._restrictRegex.IsMatch(text))
				{
					isValid = false;
					validationContext.Errors.Add(new FieldValidationException($"String value is not valid by the restrict regular expression rule. ('{this.RestrictRegexPattern}')", this));
				}
			}
		}
		
		return isValid;
	}
	
	/// <summary>
	/// Whether the regexPattern is applied to the values (the types with a built-in format rule ignore it)
	/// </summary>
	protected virtual bool UsesRegexPattern => true;
	
	/// <summary>
	/// Creates the regex once (instead of the static regex cache of each validation), an invalid pattern is rejected when the field is created
	/// </summary>
	private Regex? CreateRegex(string? pattern, string propertyName)
	{
		if (string.IsNullOrEmpty(pattern))
		{
			return null;
		}
		
		try
		{
			return new Regex(pattern);
		}
		catch (ArgumentException ex)
		{
			throw new FieldValidationException($"The '{propertyName}' is not a valid regular expression. ({ex.Message})", this);
		}
	}
	
	private bool ValidateMinLength(out Exception? exception)
	{
		if (this.MinLength < 0)
		{
			exception = new FieldValidationException("MinLength can not be less than zero", this);
			return false;
		}
		
		if (this.MaxLength != null && this.MinLength != null && this.MaxLength < this.MinLength)
		{
			exception = new FieldValidationException("MinLength can not be greater than MaxLength", this);
			return false;
		}
		
		exception = null;
		return true;
	}
	
	private bool ValidateMaxLength(out Exception? exception)
	{
		if (this.MaxLength < 0)
		{
			exception = new FieldValidationException("MaxLength can not be less than zero", this);
			return false;
		}
		
		if (this.MinLength != null && this.MaxLength != null && this.MinLength > this.MaxLength)
		{
			exception = new FieldValidationException("MinLength can not be greater than MaxLength", this);
			return false;
		}
		
		exception = null;
		return true;
	}
	
	private bool ValidateFormatPattern(out Exception? exception)
	{
		if (this.FormatPattern != null)
		{
			var segments = GetFormatSegments();
			if (segments.Any(x => x.Contains(' ')))
			{
				exception = new FieldValidationException("The 'formatPattern' segments can not be contains whitespace", this);
				return false;   
			}
		}
		
		exception = null;
		return true;
	}
	
	public string? Format(DynamicObject? content)
	{
		if (!string.IsNullOrEmpty(this.FormatPattern) && content != null)
		{
			var text = new string(this.FormatPattern.Trim());
			while (text.Contains(OPEN_FORMAT_BRACKETS) && text.Contains(CLOSE_FORMAT_BRACKETS))
			{
				var openIndex = text.IndexOf(OPEN_FORMAT_BRACKETS, StringComparison.Ordinal);
				var closeIndex = text.IndexOf(CLOSE_FORMAT_BRACKETS, StringComparison.Ordinal);
				var segment = text.Substring(openIndex + OPEN_FORMAT_BRACKETS.Length, closeIndex - openIndex - OPEN_FORMAT_BRACKETS.Length);
				if (content.TryGetValue(segment.Trim(), out var value, out _))
				{
					text = text.Replace($"{OPEN_FORMAT_BRACKETS}{segment}{CLOSE_FORMAT_BRACKETS}", value != null ? DynamicValues.ToInvariantString(value) : "null");
				}
				else
				{
					throw new FormatException("Format parameter value could not be retrieved");
				}
			}
			
			return text;
		}
		else
		{
			return null;
		}
	}
	
	public bool TryFormat(DynamicObject? content, out string? value)
	{
		try
		{
			value = this.Format(content);
			return true;
		}
		catch (FormatException)
		{
			value = null;
			return false;
		}
	}
	
	private IEnumerable<string> GetFormatSegments()
	{
		if (!string.IsNullOrEmpty(this.FormatPattern))
		{
			var text = new string(this.FormatPattern.Trim());
			while (text.Contains(OPEN_FORMAT_BRACKETS) && text.Contains(CLOSE_FORMAT_BRACKETS))
			{
				var openIndex = text.IndexOf(OPEN_FORMAT_BRACKETS, StringComparison.Ordinal);
				var closeIndex = text.IndexOf(CLOSE_FORMAT_BRACKETS, StringComparison.Ordinal);
				var segment = text.Substring(openIndex + OPEN_FORMAT_BRACKETS.Length, closeIndex - openIndex - OPEN_FORMAT_BRACKETS.Length);
				text = text.Replace($"{OPEN_FORMAT_BRACKETS}{segment}{CLOSE_FORMAT_BRACKETS}", string.Empty);
				yield return segment;
			}
		}
	}
	
	protected override void OnPropertyChanged(string propertyName)
	{
		base.OnPropertyChanged(propertyName);
		if (propertyName == nameof(this.FormatPattern))
		{
			this.ValidateHiddenRules();
		}
	}
	
	protected override void ValidateHiddenRules()
	{
		if (this.IsHidden && this.IsRequired && this.DefaultValue == null && string.IsNullOrEmpty(this.FormatPattern))
		{
			throw new FieldValidationException("A field with a default value of null cannot be both hidden and required.", this);
		}
	}
	
	#endregion
}