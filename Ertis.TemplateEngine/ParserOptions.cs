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
    
    #endregion
}