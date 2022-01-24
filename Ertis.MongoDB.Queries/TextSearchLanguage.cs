using System.Collections.Generic;

namespace Ertis.MongoDB.Queries
{
    public readonly struct TextSearchLanguage
    {
        #region Properties

        public string Name { get; }
        
        public string ISO6391Code { get; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Private Constructor
        /// </summary>
        private TextSearchLanguage(string name, string isoCode)
        {
            this.Name = name;
            this.ISO6391Code = isoCode;
        }

        #endregion

        #region Statics

        public static readonly TextSearchLanguage None = new TextSearchLanguage("None", "none");
        
        public static readonly TextSearchLanguage Danish = new TextSearchLanguage("Danish", "da");
        
        public static readonly TextSearchLanguage Dutch = new TextSearchLanguage("Dutch", "nl");
        
        public static readonly TextSearchLanguage English = new TextSearchLanguage("English", "en");
        
        public static readonly TextSearchLanguage Finnish = new TextSearchLanguage("Finnish", "fi");
        
        public static readonly TextSearchLanguage French = new TextSearchLanguage("French", "fr");
        
        public static readonly TextSearchLanguage German = new TextSearchLanguage("German", "de");
        
        public static readonly TextSearchLanguage Hungarian = new TextSearchLanguage("Hungarian", "hu");
        
        public static readonly TextSearchLanguage Italian = new TextSearchLanguage("Italian", "it");
        
        public static readonly TextSearchLanguage Norwegian = new TextSearchLanguage("Norwegian", "nb");
        
        public static readonly TextSearchLanguage Portuguese = new TextSearchLanguage("Portuguese", "pt");
        
        public static readonly TextSearchLanguage Romanian = new TextSearchLanguage("Romanian", "ro");
        
        public static readonly TextSearchLanguage Russian = new TextSearchLanguage("Russian", "ru");
        
        public static readonly TextSearchLanguage Spanish = new TextSearchLanguage("Spanish", "es");
        
        public static readonly TextSearchLanguage Swedish = new TextSearchLanguage("Swedish", "sv");
        
        public static readonly TextSearchLanguage Turkish = new TextSearchLanguage("Turkish", "tr");

        private static IReadOnlyCollection<TextSearchLanguage> _all;
        public static IReadOnlyCollection<TextSearchLanguage> All
        {
            get
            {
                if (_all == null)
                {
                    _all = new[]
                    {
                        None,
                        Danish,
                        Dutch,
                        English,
                        Finnish,
                        French,
                        German,
                        Hungarian,
                        Italian,
                        Norwegian,
                        Portuguese,
                        Romanian,
                        Russian,
                        Spanish,
                        Swedish,
                        Turkish
                    };
                }

                return _all;
            }
        }

        #endregion
    }
}