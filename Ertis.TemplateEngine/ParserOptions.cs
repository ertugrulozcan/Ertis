namespace Ertis.TemplateEngine
{
    public class ParserOptions
    {
        #region Properties

        public string OpenBrackets { get; init; }
        
        public string CloseBrackets { get; init; }

        public UndefinedStrategy UndefinedStrategy { get; init; } = UndefinedStrategy.Ignore;
        
        public string Fallback { get; init; }

        #endregion
    }
}