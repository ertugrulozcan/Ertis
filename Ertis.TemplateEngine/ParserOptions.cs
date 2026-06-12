// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Ertis.TemplateEngine;

public class ParserOptions
{
	#region Constants
	
	private const string DEFAULT_OPEN_BRACKETS = "{{";
	private const string DEFAULT_CLOSE_BRACKETS = "}}";
	
	#endregion
	
	#region Properties
	
	public string OpenBrackets
	{
		get => string.IsNullOrEmpty(field) ? DEFAULT_OPEN_BRACKETS : field;
		init;
	}
	
	public string CloseBrackets
	{
		get => string.IsNullOrEmpty(field) ? DEFAULT_CLOSE_BRACKETS : field;
		init;
	}
	
	public UndefinedStrategy UndefinedStrategy { get; init; } = UndefinedStrategy.Ignore;
	
	public string? Fallback { get; init; }
	
	#endregion
}