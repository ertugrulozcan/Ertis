using System.Text;

namespace Ertis.Core.Helpers
{
	public static class Slugifier
	{
		public static string Slugify(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}

			var inputLength = input.Length;
			var previousDash = false;
			var slugBuilder = new StringBuilder(inputLength * 2);

			for (var i = 0; i < inputLength; i++)
			{
				var c = input[i];
				switch (c)
				{
					case >= 'a' and <= 'z':
					case >= '0' and <= '9':
						slugBuilder.Append(c);
						previousDash = false;
						break;
					case >= 'A' and <= 'Z':
						slugBuilder.Append((char)(c | 32));
						previousDash = false;
						break;
					case ' ':
					case ',':
					case '.':
					case '/':
					case '\\':
					case '-':
					case '_':
					case '=':
					{
						if (!previousDash && slugBuilder.Length > 0)
						{
							slugBuilder.Append('-');
							previousDash = true;
						}

						break;
					}
					default:
					{
						if (c >= 128)
						{
							var previousLength = slugBuilder.Length;
							slugBuilder.Append(NonAscii.RemapToAscii(c));
							if (previousLength != slugBuilder.Length)
							{
								previousDash = false;
							}
						}

						break;
					}
				}
			}

			var str = slugBuilder.ToString();
			if (previousDash)
			{
				// ReSharper disable once ReplaceSubstringWithRangeIndexer
				str = str.Substring(0, slugBuilder.Length - 1);
			}

			return str;
		}
	}
}