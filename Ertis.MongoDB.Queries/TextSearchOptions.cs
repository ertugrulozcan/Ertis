namespace Ertis.MongoDB.Queries
{
    public class TextSearchOptions
    {
        #region Properties

        public TextSearchLanguage Language { get; set; }
        
        public bool IsCaseSensitive { get; set; }
        
        public bool IsDiacriticSensitive { get; set; }

        #endregion
    }
}