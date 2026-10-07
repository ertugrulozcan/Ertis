// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Ertis.TemplateEngine;

public class ParserOptions
{
	#region Properties
	
	public required string OpenBrackets { get; init; }
	
	public required string CloseBrackets { get; init; }
	
	public UndefinedStrategy UndefinedStrategy { get; init; } = UndefinedStrategy.Ignore;
	
	public string? Fallback { get; init; }
	
	/// <summary>
	/// The culture of the formatted values (numbers, dates); the current culture when it is null
	/// </summary>
	public IFormatProvider? FormatProvider { get; init; }
	
	/// <summary>
	/// Encodes the resolved values before they are written into the template (e.g. WebUtility.HtmlEncode for html templates).
	/// The fallback value and the unresolved placeholders are written as they are.
	/// </summary>
	public Func<string, string>? ValueEncoder { get; init; }
	
	#endregion
}