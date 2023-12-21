using System;
using System.Collections.Generic;
using Ertis.MongoDB.Models;

namespace Ertis.MongoDB.Helpers;

public static class LocaleHelper
{
	#region Properties

	private static readonly Dictionary<string, string> Locales = new()
	{
		{ "Afrikaans", "af" }, 
		{ "Albanian", "sq" }, 
		{ "Amharic", "am" }, 
		{ "Arabic", "ar" }, 
		{ "Armenian", "hy" }, 
		{ "Assamese", "as" }, 
		{ "Azeri", "az" }, 
		{ "Bengali", "bn" }, 
		{ "Belarusian", "be" },
		{ "Bosnian", "bs" }, 
		{ "Bosnian_Cyrillic", "bs_Cyrl" }, 
		{ "Bulgarian", "bg" }, 
		{ "Burmese", "my" }, 
		{ "Catalan", "ca" }, 
		{ "Cherokee", "chr" }, 
		{ "Chinese", "zh" }, 
		{ "Chinese_Traditional", "zh_Hant" }, 
		{ "Croatian", "hr" }, 
		{ "Czech", "cs" }, 
		{ "Danish", "da" }, 
		{ "Dutch", "nl" }, 
		{ "Dzongkha", "dz" }, 
		{ "English", "en" }, 
		{ "English_USA", "en_US" }, 
		{ "English_US_POSIX", "en_US_POSIX" }, 
		{ "Esperanto", "eo" }, 
		{ "Estonian", "et" }, 
		{ "Ewe", "ee" }, 
		{ "Faroese", "fo" }, 
		{ "Filipino", "fil" }, 
		{ "Finnish", "fi" }, 
		{ "French", "fr" }, 
		{ "French_Canada", "fr_CA" }, 
		{ "Galician", "gl" }, 
		{ "Georgian", "ka" }, 
		{ "German", "de" }, 
		{ "German_Austria", "de_AT" }, 
		{ "Greek", "el" }, 
		{ "Gujarati", "gu" }, 
		{ "Hausa", "ha" }, 
		{ "Hawaiian", "haw" }, 
		{ "Hebrew", "he" }, 
		{ "Hindi", "hi" }, 
		{ "Hungarian", "hu" }, 
		{ "Icelandic", "is" }, 
		{ "Igbo", "ig" }, 
		{ "Inari_Sami", "smn" }, 
		{ "Indonesian", "id" }, 
		{ "Irish", "ga" }, 
		{ "Italian", "it" }, 
		{ "Japanese", "ja" }, 
		{ "Kalaallisut", "kl" }, 
		{ "Kannada", "kn" }, 
		{ "Kazakh", "kk" }, 
		{ "Khmer", "km" }, 
		{ "Konkani", "kok" }, 
		{ "Korean", "ko" }, 
		{ "Kyrgyz", "ky" }, 
		{ "Lakota", "lkt" }, 
		{ "Lao", "lo" }, 
		{ "Latvian", "lv" }, 
		{ "Lingala", "ln" }, 
		{ "Lithuanian", "lt" }, 
		{ "Lower_Sorbian", "dsb" }, 
		{ "Luxembourgish", "lb" }, 
		{ "Macedonian", "mk" }, 
		{ "Malay", "ms" }, 
		{ "Malayalam", "ml" }, 
		{ "Maltese", "mt" }, 
		{ "Marathi", "mr" }, 
		{ "Mongolian", "mn" }, 
		{ "Nepali", "ne" }, 
		{ "Northern_Sami", "se" }, 
		{ "Norwegian_Bokmål", "nb" }, 
		{ "Norwegian_Nynorsk", "nn" }, 
		{ "Oriya", "or" }, 
		{ "Oromo", "om" }, 
		{ "Pashto", "ps" }, 
		{ "Persian", "fa" }, 
		{ "Polish", "pl" }, 
		{ "Portuguese", "pt" }, 
		{ "Punjabi", "pa" }, 
		{ "Romanian", "ro" }, 
		{ "Russian", "ru" }, 
		{ "Serbian", "sr" }, 
		{ "Serbian_Latin", "sr_Latn" }, 
		{ "Sinhala", "si" }, 
		{ "Slovak", "sk" }, 
		{ "Slovenian", "sl" }, 
		{ "Spanish", "es" }, 
		{ "Swahili", "sw" }, 
		{ "Swedish", "sv" }, 
		{ "Tamil", "ta" }, 
		{ "Telugu", "te" }, 
		{ "Thai", "th" }, 
		{ "Tibetan", "bo" }, 
		{ "Tongan", "to" }, 
		{ "Turkish", "tr" }, 
		{ "Ukrainian", "uk" }, 
		{ "Upper_Sorbian", "hsb" }, 
		{ "Urdu", "ur" }, 
		{ "Uyghur", "ug" }, 
		{ "Vietnamese", "vi" }, 
		{ "Walser", "wae" }, 
		{ "Welsh", "cy" }, 
		{ "Yiddish", "yi" }, 
		{ "Yoruba", "yo" }, 
		{ "Zulu", "zu" }, 
	};

	#endregion

	#region Methods

	public static string GetLanguageCode(Locale locale)
	{
		if (Locales.TryGetValue(locale.ToString(), out var languageCode))
		{
			return languageCode;
		}
		else
		{
			throw new Exception("Locale not supported");
		}
	}

	#endregion
}