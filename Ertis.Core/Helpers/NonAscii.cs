using System.Collections.Generic;
using System.Collections.ObjectModel;
// ReSharper disable MemberCanBePrivate.Global

namespace Ertis.Core.Helpers
{
    public static class NonAscii
    {
	    #region Fields & Properties

	    // International
	    private static ReadOnlyDictionary<char, string> _international;
	    public static ReadOnlyDictionary<char, string> International
	    {
		    get { return _international ??= new ReadOnlyDictionary<char, string>(InternationalChars); }
	    }
	    
	    // Latin
	    private static ReadOnlyDictionary<char, string> _latin;
	    public static ReadOnlyDictionary<char, string> Latin
	    {
		    get { return _latin ??= new ReadOnlyDictionary<char, string>(LatinChars); }
	    }
	    
	    // Turkish
	    private static ReadOnlyDictionary<char, string> _turkish;
	    public static ReadOnlyDictionary<char, string> Turkish
	    {
		    get { return _turkish ??= new ReadOnlyDictionary<char, string>(TurkishChars); }
	    }
	    
	    // Greek
	    private static ReadOnlyDictionary<char, string> _greek;
	    public static ReadOnlyDictionary<char, string> Greek
	    {
		    get { return _greek ??= new ReadOnlyDictionary<char, string>(GreekChars); }
	    }
	    
	    // Czech
	    private static ReadOnlyDictionary<char, string> _czech;
	    public static ReadOnlyDictionary<char, string> Czech
	    {
		    get { return _czech ??= new ReadOnlyDictionary<char, string>(CzechChars); }
	    }
	    
	    // Arabic
	    private static ReadOnlyDictionary<char, string> _arabic;
	    public static ReadOnlyDictionary<char, string> Arabic
	    {
		    get { return _arabic ??= new ReadOnlyDictionary<char, string>(ArabicChars); }
	    }
	    
	    // Vietnamese
	    private static ReadOnlyDictionary<char, string> _vietnamese;
	    public static ReadOnlyDictionary<char, string> Vietnamese
	    {
		    get { return _vietnamese ??= new ReadOnlyDictionary<char, string>(VietnameseChars); }
	    }
	    
	    // Polish
	    private static ReadOnlyDictionary<char, string> _polish;
	    public static ReadOnlyDictionary<char, string> Polish
	    {
		    get { return _polish ??= new ReadOnlyDictionary<char, string>(PolishChars); }
	    }
	    
	    // Latvian
	    private static ReadOnlyDictionary<char, string> _latvian;
	    public static ReadOnlyDictionary<char, string> Latvian
	    {
		    get { return _latvian ??= new ReadOnlyDictionary<char, string>(LatvianChars); }
	    }
	    
	    // German
	    private static ReadOnlyDictionary<char, string> _german;
	    public static ReadOnlyDictionary<char, string> German
	    {
		    get { return _german ??= new ReadOnlyDictionary<char, string>(GermanChars); }
	    }
	    
	    // Ukrainian
	    private static ReadOnlyDictionary<char, string> _ukrainian;
	    public static ReadOnlyDictionary<char, string> Ukrainian
	    {
		    get { return _ukrainian ??= new ReadOnlyDictionary<char, string>(UkrainianChars); }
	    }
	    
	    // Serbian
	    private static ReadOnlyDictionary<char, string> _serbian;
	    public static ReadOnlyDictionary<char, string> Serbian
	    {
		    get { return _serbian ??= new ReadOnlyDictionary<char, string>(SerbianChars); }
	    }
	    
	    // Russian
	    private static ReadOnlyDictionary<char, string> _russian;
	    public static ReadOnlyDictionary<char, string> Russian
	    {
		    get { return _russian ??= new ReadOnlyDictionary<char, string>(RussianChars); }
	    }

	    #endregion
	    
        #region Constants

        private static readonly Dictionary<char, string> InternationalChars = new()
		{
			{ 'À', "a" },
			{ 'Á', "a" },
			{ 'Â', "a" },
			{ 'Ã', "a" },
			{ 'Ä', "a" },
			{ 'Å', "a" },
			{ 'Æ', "ae" },
			{ 'Ç', "c" },
			{ 'È', "e" },
			{ 'É', "e" },
			{ 'Ê', "e" },
			{ 'Ë', "e" },
			{ 'Ì', "i" },
			{ 'Í', "i" },
			{ 'Î', "i" },
			{ 'Ï', "i" },
			{ 'Ð', "d" },
			{ 'Ñ', "n" },
			{ 'Ò', "o" },
			{ 'Ó', "o" },
			{ 'Ô', "o" },
			{ 'Õ', "o" },
			{ 'Ő', "o" },
			{ 'Ø', "o" },
			{ 'Ù', "u" },
			{ 'Ú', "u" },
			{ 'Û', "u" },
			{ 'Ü', "u" },
			{ 'Ű', "u" },
			{ 'Ý', "y" },
			{ 'Þ', "th" },
			{ 'ß', "ss" },
			{ 'à', "a" },
			{ 'á', "a" },
			{ 'â', "a" },
			{ 'ã', "a" },
			{ 'ä', "a" },
			{ 'å', "a" },
			{ 'æ', "ae" },
			{ 'ç', "c" },
			{ 'è', "e" },
			{ 'é', "e" },
			{ 'ê', "e" },
			{ 'ë', "e" },
			{ 'ì', "i" },
			{ 'í', "i" },
			{ 'î', "i" },
			{ 'ï', "i" },
			{ 'ð', "d" },
			{ 'ñ', "n" },
			{ 'ò', "o" },
			{ 'ô', "o" },
			{ 'õ', "o" },
			{ 'ö', "o" },
			{ 'ő', "o" },
			{ 'ø', "o" },
			{ 'ù', "u" },
			{ 'ú', "u" },
			{ 'û', "u" },
			{ 'ü', "u" },
			{ 'ű', "u" },
			{ 'ý', "y" },
			{ 'þ', "th" },
			{ 'ÿ', "y" },
			{ 'ẞ', "ss" },
			{ 'α', "a" },
			{ 'β', "b" },
			{ 'γ', "g" },
			{ 'δ', "d" },
			{ 'ε', "e" },
			{ 'ζ', "z" },
			{ 'η', "h" },
			{ 'θ', "8" },
			{ 'ι', "i" },
			{ 'κ', "k" },
			{ 'λ', "l" },
			{ 'μ', "m" },
			{ 'ν', "n" },
			{ 'ξ', "3" },
			{ 'ο', "o" },
			{ 'π', "p" },
			{ 'ρ', "r" },
			{ 'σ', "s" },
			{ 'τ', "t" },
			{ 'υ', "y" },
			{ 'φ', "f" },
			{ 'χ', "x" },
			{ 'ψ', "ps" },
			{ 'ω', "w" },
			{ 'ά', "a" },
			{ 'έ', "e" },
			{ 'ί', "i" },
			{ 'ό', "o" },
			{ 'ύ', "y" },
			{ 'ώ', "w" },
			{ 'ς', "s" },
			{ 'ϊ', "i" },
			{ 'ΰ', "y" },
			{ 'ϋ', "y" },
			{ 'ΐ', "i" },
			{ 'Α', "a" },
			{ 'Β', "b" },
			{ 'Γ', "g" },
			{ 'Δ', "d" },
			{ 'Ε', "e" },
			{ 'Ζ', "z" },
			{ 'Η', "h" },
			{ 'Θ', "8" },
			{ 'Ι', "i" },
			{ 'Κ', "k" },
			{ 'Λ', "l" },
			{ 'Μ', "m" },
			{ 'Ν', "n" },
			{ 'Ξ', "3" },
			{ 'Ο', "o" },
			{ 'Π', "p" },
			{ 'Ρ', "r" },
			{ 'Σ', "s" },
			{ 'Τ', "t" },
			{ 'Υ', "y" },
			{ 'Φ', "f" },
			{ 'Χ', "x" },
			{ 'Ψ', "ps" },
			{ 'Ω', "w" },
			{ 'Ά', "a" },
			{ 'Έ', "e" },
			{ 'Ί', "i" },
			{ 'Ό', "o" },
			{ 'Ύ', "y" },
			{ 'Ή', "h" },
			{ 'Ώ', "w" },
			{ 'Ϊ', "i" },
			{ 'Ϋ', "y" },
			{ 'ş', "s" },
			{ 'Ş', "s" },
			{ 'ı', "i" },
			{ 'İ', "i" },
			{ 'Ö', "o" },
			{ 'ğ', "g" },
			{ 'Ğ', "g" },
			{ 'а', "a" },
			{ 'б', "b" },
			{ 'в', "v" },
			{ 'г', "g" },
			{ 'д', "d" },
			{ 'е', "e" },
			{ 'ё', "yo" },
			{ 'ж', "zh" },
			{ 'з', "z" },
			{ 'и', "i" },
			{ 'й', "j" },
			{ 'к', "k" },
			{ 'л', "l" },
			{ 'м', "m" },
			{ 'н', "n" },
			{ 'о', "o" },
			{ 'п', "p" },
			{ 'р', "r" },
			{ 'с', "s" },
			{ 'т', "t" },
			{ 'у', "u" },
			{ 'ф', "f" },
			{ 'х', "h" },
			{ 'ц', "c" },
			{ 'ч', "ch" },
			{ 'ш', "sh" },
			{ 'щ', "sh" },
			{ 'ъ', "u" },
			{ 'ы', "y" },
			{ 'ь', "b" },
			{ 'э', "e" },
			{ 'ю', "yu" },
			{ 'я', "ya" },
			{ 'А', "a" },
			{ 'Б', "b" },
			{ 'В', "v" },
			{ 'Г', "g" },
			{ 'Д', "d" },
			{ 'Е', "e" },
			{ 'Ё', "yo" },
			{ 'Ж', "zh" },
			{ 'З', "z" },
			{ 'И', "i" },
			{ 'Й', "j" },
			{ 'К', "k" },
			{ 'Л', "l" },
			{ 'М', "m" },
			{ 'Н', "n" },
			{ 'О', "o" },
			{ 'П', "p" },
			{ 'Р', "r" },
			{ 'С', "s" },
			{ 'Т', "t" },
			{ 'У', "u" },
			{ 'Ф', "f" },
			{ 'Х', "h" },
			{ 'Ц', "c" },
			{ 'Ч', "ch" },
			{ 'Ш', "sh" },
			{ 'Щ', "sh" },
			{ 'Ъ', "u" },
			{ 'Ы', "y" },
			{ 'Ь', "b" },
			{ 'Э', "e" },
			{ 'Ю', "yu" },
			{ 'Я', "ya" },
			{ 'Є', "ye" },
			{ 'І', "i" },
			{ 'Ї', "yi" },
			{ 'Ґ', "g" },
			{ 'є', "ye" },
			{ 'і', "i" },
			{ 'ї', "yi" },
			{ 'ґ', "g" },
			{ 'ď', "d" },
			{ 'ě', "e" },
			{ 'ň', "n" },
			{ 'ř', "r" },
			{ 'š', "s" },
			{ 'ť', "t" },
			{ 'ů', "u" },
			{ 'ž', "z" },
			{ 'Ď', "d" },
			{ 'Ě', "e" },
			{ 'Ň', "n" },
			{ 'Ř', "r" },
			{ 'Š', "s" },
			{ 'Ť', "t" },
			{ 'Ů', "u" },
			{ 'Ž', "z" },
			{ 'ą', "a" },
			{ 'ć', "c" },
			{ 'ę', "e" },
			{ 'ł', "l" },
			{ 'ń', "n" },
			{ 'ó', "o" },
			{ 'ś', "s" },
			{ 'ź', "z" },
			{ 'ż', "z" },
			{ 'Ą', "a" },
			{ 'Ć', "c" },
			{ 'Ę', "e" },
			{ 'Ł', "l" },
			{ 'Ń', "m" },
			{ 'Ś', "s" },
			{ 'Ź', "z" },
			{ 'Ż', "z" },
			{ 'ā', "a" },
			{ 'č', "c" },
			{ 'ē', "e" },
			{ 'ģ', "g" },
			{ 'ī', "i" },
			{ 'ķ', "k" },
			{ 'ļ', "l" },
			{ 'ņ', "n" },
			{ 'ū', "u" },
			{ 'Ā', "a" },
			{ 'Č', "c" },
			{ 'Ē', "e" },
			{ 'Ģ', "g" },
			{ 'Ī', "i" },
			{ 'Ķ', "k" },
			{ 'Ļ', "l" },
			{ 'Ņ', "n" },
			{ 'Ū', "u" },
			{ 'œ', "oe" },
			{ 'Œ', "oe" },
			{ '∂', "d" },
			{ 'ƒ', "f" },
			{ 'º', "o" },
			{ 'ª', "a" },
			{ 'ĉ', "c" },
			{ 'Ĉ', "c" },
			{ '¹', "1" },
			{ '²', "2" },
			{ '³', "3" },
			{ '¶', "P" }
		};
		
		private static readonly Dictionary<char, string> LatinChars = new()
		{
			{ '°', "0" }, 
			{ 'æ', "ae" }, 
			{ 'ǽ', "ae" }, 
			{ 'À', "A" }, 
			{ 'Á', "A" }, 
			{ 'Â', "A" }, 
			{ 'Ã', "A" }, 
			{ 'Å', "A" }, 
			{ 'Ǻ', "A" },
			{ 'Ă', "A" }, 
			{ 'Ǎ', "A" }, 
			{ 'Æ', "AE" }, 
			{ 'Ǽ', "AE" }, 
			{ 'à', "a" }, 
			{ 'á', "a" }, 
			{ 'â', "a" }, 
			{ 'ã', "a" }, 
			{ 'å', "a" },
			{ 'ǻ', "a" }, 
			{ 'ă', "a" }, 
			{ 'ǎ', "a" }, 
			{ 'ª', "a" }, 
			{ '@', "at" }, 
			{ 'Ĉ', "C" }, 
			{ 'Ċ', "C" }, 
			{ 'ĉ', "c" }, 
			{ 'ċ', "c" },
			{ '©', "c" }, 
			{ 'Ð', "Dj" }, 
			{ 'Đ', "D" }, 
			{ 'ð', "dj" }, 
			{ 'đ', "d" }, 
			{ 'È', "E" }, 
			{ 'É', "E" }, 
			{ 'Ê', "E" }, 
			{ 'Ë', "E" },
			{ 'Ĕ', "E" }, 
			{ 'Ė', "E" }, 
			{ 'è', "e" }, 
			{ 'é', "e" }, 
			{ 'ê', "e" }, 
			{ 'ë', "e" }, 
			{ 'ĕ', "e" }, 
			{ 'ė', "e" }, 
			{ 'ƒ', "f" },
			{ 'Ĝ', "G" },
			{ 'Ġ', "G" },
			{ 'ĝ', "g" },
			{ 'ġ', "g" },
			{ 'Ĥ', "H" },
			{ 'Ħ', "H" },
			{ 'ĥ', "h" },
			{ 'ħ', "h" },
			{ 'Ì', "I" },
			{ 'Í', "I" },
			{ 'Î', "I" },
			{ 'Ï', "I" },
			{ 'Ĩ', "I" },
			{ 'Ĭ', "I" },
			{ 'Ǐ', "I" },
			{ 'Į', "I" },
			{ 'Ĳ', "IJ" },
			{ 'ì', "i" },
			{ 'í', "i" },
			{ 'î', "i" },
			{ 'ï', "i" },
			{ 'ĩ', "i" },
			{ 'ĭ', "i" },
			{ 'ǐ', "i" },
			{ 'į', "i" },
			{ 'ĳ', "ij" },
			{ 'Ĵ', "J" },
			{ 'ĵ', "j" },
			{ 'Ĺ', "L" },
			{ 'Ľ', "L" },
			{ 'Ŀ', "L" },
			{ 'ĺ', "l" },
			{ 'ľ', "l" },
			{ 'ŀ', "l" },
			{ 'Ñ', "N" },
			{ 'ñ', "n" },
			{ 'ŉ', "n" },
			{ 'Ò', "O" },
			{ 'Ô', "O" },
			{ 'Õ', "O" },
			{ 'Ō', "O" },
			{ 'Ŏ', "O" },
			{ 'Ǒ', "O" },
			{ 'Ő', "O" },
			{ 'Ơ', "O" },
			{ 'Ø', "O" },
			{ 'Ǿ', "O" },
			{ 'Œ', "OE" },
			{ 'ò', "o" },
			{ 'ô', "o" },
			{ 'õ', "o" },
			{ 'ō', "o" },
			{ 'ŏ', "o" },
			{ 'ǒ', "o" },
			{ 'ő', "o" },
			{ 'ơ', "o" },
			{ 'ø', "o" },
			{ 'ǿ', "o" },
			{ 'º', "o" },
			{ 'œ', "oe" },
			{ 'Ŕ', "R" },
			{ 'Ŗ', "R" },
			{ 'ŕ', "r" },
			{ 'ŗ', "r" },
			{ 'Ŝ', "S" },
			{ 'Ș', "S" },
			{ 'ŝ', "s" },
			{ 'ș', "s" },
			{ 'ſ', "s" },
			{ 'Ţ', "T" },
			{ 'Ț', "T" },
			{ 'Ŧ', "T" },
			{ 'Þ', "TH" },
			{ 'ţ', "t" },
			{ 'ț', "t" },
			{ 'ŧ', "t" },
			{ 'þ', "th" },
			{ 'Ù', "U" },
			{ 'Ú', "U" },
			{ 'Û', "U" },
			{ 'Ũ', "U" },
			{ 'Ŭ', "U" },
			{ 'Ű', "U" },
			{ 'Ų', "U" },
			{ 'Ư', "U" },
			{ 'Ǔ', "U" },
			{ 'Ǖ', "U" },
			{ 'Ǘ', "U" },
			{ 'Ǚ', "U" },
			{ 'Ǜ', "U" },
			{ 'ù', "u" },
			{ 'ú', "u" },
			{ 'û', "u" },
			{ 'ũ', "u" },
			{ 'ŭ', "u" },
			{ 'ű', "u" },
			{ 'ų', "u" },
			{ 'ư', "u" },
			{ 'ǔ', "u" },
			{ 'ǖ', "u" },
			{ 'ǘ', "u" },
			{ 'ǚ', "u" },
			{ 'ǜ', "u" },
			{ 'Ŵ', "W" },
			{ 'ŵ', "w" },
			{ 'Ý', "Y" },
			{ 'Ÿ', "Y" },
			{ 'Ŷ', "Y" },
			{ 'ý', "y" },
			{ 'ÿ', "y" },
			{ 'ŷ', "y" }
		};

		private static readonly Dictionary<char, string> TurkishChars = new()
		{
			{ 'Ç', "C" },
			{ 'Ğ', "G" },
			{ 'İ', "I" },
			{ 'Ş', "S" },
			{ 'ç', "c" },
			{ 'ğ', "g" },
			{ 'ı', "i" },
			{ 'ş', "s" },
			{ 'ö', "o" },
			{ 'Ö', "O" },
			{ 'ü', "u" },
			{ 'Ü', "U" }
		};
		
		private static readonly Dictionary<char, string> GreekChars = new()
		{
			{ 'Α', "A" },
			{ 'Β', "B" },
			{ 'Γ', "G" },
			{ 'Δ', "D" },
			{ 'Ε', "E" },
			{ 'Ζ', "Z" },
			{ 'Η', "I" },
			{ 'Θ', "Th" },
			{ 'Ι', "I" },
			{ 'Κ', "K" },
			{ 'Λ', "L" },
			{ 'Μ', "M" },
			{ 'Ν', "N" },
			{ 'Ξ', "Ks" },
			{ 'Ο', "O" },
			{ 'Π', "P" },
			{ 'Ρ', "R" },
			{ 'Σ', "S" },
			{ 'Τ', "T" },
			{ 'Υ', "Y" },
			{ 'Φ', "Ph" },
			{ 'Χ', "Ch" },
			{ 'Ψ', "Ps" },
			{ 'Ω', "O" },
			{ 'Ϊ', "I" },
			{ 'Ϋ', "Y" },
			{ 'ά', "a" },
			{ 'έ', "e" },
			{ 'ή', "i" },
			{ 'ί', "i" },
			{ 'ΰ', "Y" },
			{ 'α', "a" },
			{ 'β', "b" },
			{ 'γ', "g" },
			{ 'δ', "d" },
			{ 'ε', "e" },
			{ 'ζ', "z" },
			{ 'η', "i" },
			{ 'θ', "th" },
			{ 'ι', "i" },
			{ 'κ', "k" },
			{ 'λ', "l" },
			{ 'μ', "m" },
			{ 'ν', "n" },
			{ 'ξ', "ks" },
			{ 'ο', "o" },
			{ 'π', "p" },
			{ 'ρ', "r" },
			{ 'ς', "s" },
			{ 'σ', "s" },
			{ 'τ', "t" },
			{ 'υ', "y" },
			{ 'φ', "ph" },
			{ 'χ', "x" },
			{ 'ψ', "ps" },
			{ 'ω', "o" },
			{ 'ϊ', "i" },
			{ 'ϋ', "y" },
			{ 'ό', "o" },
			{ 'ύ', "y" },
			{ 'ώ', "o" },
			{ 'ϐ', "b" },
			{ 'ϑ', "th" },
			{ 'ϒ', "Y" }
		};
		
		private static readonly Dictionary<char, string> CzechChars = new()
		{
			{ 'Č', "C" },
			{ 'Ď', "D" },
			{ 'Ě', "E" },
			{ 'Ň', "N" },
			{ 'Ř', "R" },
			{ 'Š', "S" },
			{ 'Ť', "T" },
			{ 'Ů', "U" },
			{ 'Ž', "Z" },
			{ 'č', "c" },
			{ 'ď', "d" },
			{ 'ě', "e" },
			{ 'ň', "n" },
			{ 'ř', "r" },
			{ 'š', "s" },
			{ 'ť', "t" },
			{ 'ů', "u" },
			{ 'ž', "z" }
		};
		
		private static readonly Dictionary<char, string> ArabicChars = new()
		{
			{ 'أ', "a" },
			{ 'ب', "b" },
			{ 'ت', "t" },
			{ 'ث', "th" },
			{ 'ج', "g" },
			{ 'ح', "h" },
			{ 'خ', "kh" },
			{ 'د', "d" },
			{ 'ذ', "th" },
			{ 'ر', "r" },
			{ 'ز', "z" },
			{ 'س', "s" },
			{ 'ش', "sh" },
			{ 'ص', "s" },
			{ 'ض', "d" },
			{ 'ط', "t" },
			{ 'ظ', "th" },
			{ 'ع', "aa" },
			{ 'غ', "gh" },
			{ 'ف', "f" },
			{ 'ق', "k" },
			{ 'ك', "k" },
			{ 'ل', "l" },
			{ 'م', "m" },
			{ 'ن', "n" },
			{ 'ه', "h" },
			{ 'و', "o" },
			{ 'ي', "y" }
		};
		
		private static readonly Dictionary<char, string> VietnameseChars = new()
		{
			{ 'ạ', "a" },
			{ 'ả', "a" },
			{ 'ầ', "a" },
			{ 'ấ', "a" },
			{ 'ậ', "a" },
			{ 'ẩ', "a" },
			{ 'ẫ', "a" },
			{ 'ằ', "a" },
			{ 'ắ', "a" },
			{ 'ặ', "a" },
			{ 'ẳ', "a" },
			{ 'ẵ', "a" },
			{ 'ẹ', "e" },
			{ 'ẻ', "e" },
			{ 'ẽ', "e" },
			{ 'ề', "e" },
			{ 'ế', "e" },
			{ 'ệ', "e" },
			{ 'ể', "e" },
			{ 'ễ', "e" },
			{ 'ị', "i" },
			{ 'ỉ', "i" },
			{ 'ọ', "o" },
			{ 'ỏ', "o" },
			{ 'ồ', "o" },
			{ 'ố', "o" },
			{ 'ộ', "o" },
			{ 'ổ', "o" },
			{ 'ỗ', "o" },
			{ 'ờ', "o" },
			{ 'ớ', "o" },
			{ 'ợ', "o" },
			{ 'ở', "o" },
			{ 'ỡ', "o" },
			{ 'ụ', "u" },
			{ 'ủ', "u" },
			{ 'ừ', "u" },
			{ 'ứ', "u" },
			{ 'ự', "u" },
			{ 'ử', "u" },
			{ 'ữ', "u" },
			{ 'ỳ', "y" },
			{ 'ỵ', "y" },
			{ 'ỷ', "y" },
			{ 'ỹ', "y" },
			{ 'Ạ', "A" },
			{ 'Ả', "A" },
			{ 'Ầ', "A" },
			{ 'Ấ', "A" },
			{ 'Ậ', "A" },
			{ 'Ẩ', "A" },
			{ 'Ẫ', "A" },
			{ 'Ằ', "A" },
			{ 'Ắ', "A" },
			{ 'Ặ', "A" },
			{ 'Ẳ', "A" },
			{ 'Ẵ', "A" },
			{ 'Ẹ', "E" },
			{ 'Ẻ', "E" },
			{ 'Ẽ', "E" },
			{ 'Ề', "E" },
			{ 'Ế', "E" },
			{ 'Ệ', "E" },
			{ 'Ể', "E" },
			{ 'Ễ', "E" },
			{ 'Ị', "I" },
			{ 'Ỉ', "I" },
			{ 'Ọ', "O" },
			{ 'Ỏ', "O" },
			{ 'Ồ', "O" },
			{ 'Ố', "O" },
			{ 'Ộ', "O" },
			{ 'Ổ', "O" },
			{ 'Ỗ', "O" },
			{ 'Ờ', "O" },
			{ 'Ớ', "O" },
			{ 'Ợ', "O" },
			{ 'Ở', "O" },
			{ 'Ỡ', "O" },
			{ 'Ụ', "U" },
			{ 'Ủ', "U" },
			{ 'Ừ', "U" },
			{ 'Ứ', "U" },
			{ 'Ự', "U" },
			{ 'Ử', "U" },
			{ 'Ữ', "U" },
			{ 'Ỳ', "Y" },
			{ 'Ỵ', "Y" },
			{ 'Ỷ', "Y" },
			{ 'Ỹ', "Y" }
		};
		
		private static readonly Dictionary<char, string> PolishChars = new()
		{
			{ 'Ą', "A" },
			{ 'Ć', "C" },
			{ 'Ę', "E" },
			{ 'Ł', "L" },
			{ 'Ń', "N" },
			{ 'Ó', "O" },
			{ 'Ś', "S" },
			{ 'Ź', "Z" },
			{ 'Ż', "Z" },
			{ 'ą', "a" },
			{ 'ć', "c" },
			{ 'ę', "e" },
			{ 'ł', "l" },
			{ 'ń', "n" },
			{ 'ó', "o" },
			{ 'ś', "s" },
			{ 'ź', "z" },
			{ 'ż', "z" }
		};
		
		private static readonly Dictionary<char, string> LatvianChars = new()
		{
			{ 'Ā', "A" },
			{ 'Ē', "E" },
			{ 'Ģ', "G" },
			{ 'Ī', "I" },
			{ 'Ķ', "K" },
			{ 'Ļ', "L" },
			{ 'Ņ', "N" },
			{ 'Ū', "U" },
			{ 'ā', "a" },
			{ 'ē', "e" },
			{ 'ģ', "g" },
			{ 'ī', "i" },
			{ 'ķ', "k" },
			{ 'ļ', "l" },
			{ 'ņ', "n" },
			{ 'ū', "u" }
		};
		
		private static readonly Dictionary<char, string> GermanChars = new()
		{
			{ 'Ä', "AE" },
			{ 'Ö', "OE" },
			{ 'Ü', "UE" },
			{ 'ß', "ss" },
			{ 'ä', "ae" },
			{ 'ö', "oe" },
			{ 'ü', "ue" }
		};
		
		private static readonly Dictionary<char, string> UkrainianChars = new()
		{
			{ 'Ґ', "G" },
			{ 'І', "I" },
			{ 'Ї', "Ji" },
			{ 'Є', "Ye" },
			{ 'ґ', "g" },
			{ 'і', "i" },
			{ 'ї', "ji" },
			{ 'є', "ye" }
		};
		
		private static readonly Dictionary<char, string> SerbianChars = new()
		{
			{ 'ђ', "dj" },
			{ 'ј', "j" },
			{ 'љ', "lj" },
			{ 'њ', "nj" },
			{ 'ћ', "c" },
			{ 'џ', "dz" },
			{ 'Ђ', "Dj" },
			{ 'Ј', "j" },
			{ 'Љ', "Lj" },
			{ 'Њ', "Nj" },
			{ 'Ћ', "C" },
			{ 'Џ', "Dz" }
		};

		private static readonly Dictionary<char, string> RussianChars = new()
		{
			{ 'Ъ', "" },
			{ 'Ь', "" },
			{ 'А', "A" },
			{ 'Б', "B" },
			{ 'Ц', "C" },
			{ 'Ч', "Ch" },
			{ 'Д', "D" },
			{ 'Е', "E" },
			{ 'Ё', "E" },
			{ 'Э', "E" },
			{ 'Ф', "F" },
			{ 'Г', "G" },
			{ 'Х', "H" },
			{ 'И', "I" },
			{ 'Й', "J" },
			{ 'Я', "Ja" },
			{ 'Ю', "Ju" },
			{ 'К', "K" },
			{ 'Л', "L" },
			{ 'М', "M" },
			{ 'Н', "N" },
			{ 'О', "O" },
			{ 'П', "P" },
			{ 'Р', "R" },
			{ 'С', "S" },
			{ 'Ш', "Sh" },
			{ 'Щ', "Shch" },
			{ 'Т', "T" },
			{ 'У', "U" },
			{ 'В', "V" },
			{ 'Ы', "Y" },
			{ 'З', "Z" },
			{ 'Ж', "Zh" },
			{ 'ъ', "" },
			{ 'ь', "" },
			{ 'а', "a" },
			{ 'б', "b" },
			{ 'ц', "c" },
			{ 'ч', "ch" },
			{ 'д', "d" },
			{ 'е', "e" },
			{ 'ё', "e" },
			{ 'э', "e" },
			{ 'ф', "f" },
			{ 'г', "g" },
			{ 'х', "h" },
			{ 'и', "i" },
			{ 'й', "j" },
			{ 'я', "ja" },
			{ 'ю', "ju" },
			{ 'к', "k" },
			{ 'л', "l" },
			{ 'м', "m" },
			{ 'н', "n" },
			{ 'о', "o" },
			{ 'п', "p" },
			{ 'р', "r" },
			{ 'с', "s" },
			{ 'ш', "sh" },
			{ 'щ', "shch" },
			{ 'т', "t" },
			{ 'у', "u" },
			{ 'в', "v" },
			{ 'ы', "y" },
			{ 'з', "z" },
			{ 'ж', "zh" }
		};

        #endregion

        #region Methods

        internal static string RemapToAscii(char c)
        {
	        if (International.ContainsKey(c))
	        {
		        return International[c];
	        }
	        else if (Latin.ContainsKey(c))
	        {
		        return Latin[c];
	        }
	        else if (Turkish.ContainsKey(c))
	        {
		        return Turkish[c];
	        }
	        else if (Greek.ContainsKey(c))
	        {
		        return Greek[c];
	        }
	        else if (Czech.ContainsKey(c))
	        {
		        return Czech[c];
	        }
	        else if (Arabic.ContainsKey(c))
	        {
		        return Arabic[c];
	        }
	        else if (Vietnamese.ContainsKey(c))
	        {
		        return Vietnamese[c];
	        }
	        else if (Polish.ContainsKey(c))
	        {
		        return Polish[c];
	        }
	        else if (Latvian.ContainsKey(c))
	        {
		        return Latvian[c];
	        }
	        else if (German.ContainsKey(c))
	        {
		        return German[c];
	        }
	        else if (Ukrainian.ContainsKey(c))
	        {
		        return Ukrainian[c];
	        }
	        else if (Serbian.ContainsKey(c))
	        {
		        return Serbian[c];
	        }
	        else if (Russian.ContainsKey(c))
	        {
		        return Russian[c];
	        }
	        
	        return string.Empty;
        }

        #endregion
    }
}