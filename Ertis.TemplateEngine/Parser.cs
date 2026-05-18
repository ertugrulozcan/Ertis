namespace Ertis.TemplateEngine;

public class Parser
{
    #region Constants
    
    private const string DEFAULT_OPEN_BRACKETS = "{{";
    private const string DEFAULT_CLOSE_BRACKETS = "}}";
    
    #endregion
    
    #region Properties
    
    internal ParserOptions Options { get; }
    
    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options"></param>
    public Parser(ParserOptions? options = null)
    {
        if (options == null)
        {
            this.Options = new ParserOptions
            {
                OpenBrackets = DEFAULT_OPEN_BRACKETS,
                CloseBrackets = DEFAULT_CLOSE_BRACKETS
            };
        }
        else
        {
            if (string.IsNullOrEmpty(options.OpenBrackets) || string.IsNullOrEmpty(options.CloseBrackets))
            {
                throw new ArgumentException("Open&Close brackets could not be null or empty!");
            }
            
            this.Options = options;
        }
    }
    
    #endregion
    
    #region Methods
    
    public IEnumerable<ITemplateSegment> Parse(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield return new RawPart { RawValue = text };
            yield break;
        }
        
        var template = text;
        while (TryFindPlaceHolder(template, out var placeHolder))
        {
            if (placeHolder == null)
            {
                break;
            }
            
            if (placeHolder.StartIndex > 0)
            {
                var rawText = template[..placeHolder.StartIndex];
                yield return new RawPart { RawValue = rawText };
            }
            
            var length = placeHolder.Outer?.Length ?? 0;
            template = template[(placeHolder.StartIndex + length)..];
            
            yield return placeHolder;
        }
        
        if (!string.IsNullOrEmpty(template))
        {
            yield return new RawPart { RawValue = template };
        }
    }
    
    private bool TryFindPlaceHolder(string template, out PlaceHolder? placeHolder)
    {
        placeHolder = this.FindPlaceHolder(template);
        return placeHolder != null;
    }
    
    private PlaceHolder? FindPlaceHolder(string template)
    {
        var openBrackets = this.Options.OpenBrackets ?? DEFAULT_OPEN_BRACKETS;
        var closeBrackets = this.Options.CloseBrackets ?? DEFAULT_CLOSE_BRACKETS;
        
        var startIndex = template.IndexOf(openBrackets, StringComparison.Ordinal);
        var endIndex = template.IndexOf(closeBrackets, StringComparison.Ordinal);
        if (startIndex < 0 || endIndex < 0 || startIndex >= endIndex)
        {
            return null;
        }
        
        var placeholder = template.Substring(startIndex + openBrackets.Length, endIndex - startIndex - openBrackets.Length);
        return new PlaceHolder
        {
            Inner = placeholder,
            Outer = $"{openBrackets}{placeholder}{closeBrackets}",
            Value = placeholder.Trim(),
            StartIndex = startIndex,
            Length = endIndex - startIndex + closeBrackets.Length,
            OpenBrackets = openBrackets,
            CloseBrackets = closeBrackets
        };
    }
    
    #endregion
}