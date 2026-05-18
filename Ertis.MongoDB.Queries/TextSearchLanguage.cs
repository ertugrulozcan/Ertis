// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.MongoDB.Queries;

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
    
    public static readonly TextSearchLanguage None = new("None", "none");
    
    public static readonly TextSearchLanguage Danish = new("Danish", "da");
    
    public static readonly TextSearchLanguage Dutch = new("Dutch", "nl");
    
    public static readonly TextSearchLanguage English = new("English", "en");
    
    public static readonly TextSearchLanguage Finnish = new("Finnish", "fi");
    
    public static readonly TextSearchLanguage French = new("French", "fr");
    
    public static readonly TextSearchLanguage German = new("German", "de");
    
    public static readonly TextSearchLanguage Hungarian = new("Hungarian", "hu");
    
    public static readonly TextSearchLanguage Italian = new("Italian", "it");
    
    public static readonly TextSearchLanguage Norwegian = new("Norwegian", "nb");
    
    public static readonly TextSearchLanguage Portuguese = new("Portuguese", "pt");
    
    public static readonly TextSearchLanguage Romanian = new("Romanian", "ro");
    
    public static readonly TextSearchLanguage Russian = new("Russian", "ru");
    
    public static readonly TextSearchLanguage Spanish = new("Spanish", "es");
    
    public static readonly TextSearchLanguage Swedish = new("Swedish", "sv");
    
    public static readonly TextSearchLanguage Turkish = new("Turkish", "tr");
    
    // ReSharper disable once UnusedMember.Global
    public static IReadOnlyCollection<TextSearchLanguage> All
    {
        get
        {
            field ??= new[]
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
            
            return field;
        }
    }
    
    #endregion
}