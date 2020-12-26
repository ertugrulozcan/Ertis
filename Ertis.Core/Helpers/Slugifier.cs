using System.Globalization;
using System.Linq;

namespace Ertis.Core.Helpers
{
	public static class Slugifier
	{
		public static string Slugify(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			var words = text.Split(" ").ToList();
			words.ForEach(x => x = x.Replace('&', '-'));
			words.RemoveAll(x => x == "&");
			return string.Join("-", words.Select(x => ConvertTurkishCharacters(x.ToLower(CultureInfo.GetCultureInfo(0x0409)))));
		}

		private static string ConvertTurkishCharacters(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			
			return text
				.Replace("ç", "c")
				.Replace("Ç", "C")
				.Replace("ğ", "g")
				.Replace("Ğ", "G")
				.Replace("ı", "i")
				.Replace("İ", "I")
				.Replace("ö", "o")
				.Replace("Ö", "O")
				.Replace("ş", "s")
				.Replace("Ş", "S")
				.Replace("ü", "u")
				.Replace("Ü", "U");
		}

		public static string TrimSlug(string slug)
		{
			if (string.IsNullOrEmpty(slug))
				return slug;

			return slug.TrimStart('/').TrimEnd('/');
		}
	}
}