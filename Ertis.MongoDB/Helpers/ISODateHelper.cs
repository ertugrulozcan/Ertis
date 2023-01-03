using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.MongoDB.Helpers
{
	public static class ISODateHelper
	{
		#region Methods

		public static string EnsureDatetimeFieldsToISODate(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return json;
			}
			
			try
			{
				var root = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
				if (root is JToken jToken)
				{
					return EnsureDatetimeFieldsToISODate(jToken).ToString();	
				}

				return json;
			}
			catch
			{
				return json;
			}
		}
		
		public static JToken EnsureDatetimeFieldsToISODate(JToken node)
		{
			if (node == null)
			{
				return null;
			}

			try
			{
				if (node is JValue jValue)
				{
					if (node.Type == JTokenType.String || node.Type == JTokenType.Date)
					{
						if (TryParseDateTime(node.Value<string>(), out var dateTime))
						{
							jValue.Replace(new JRaw($"ISODate(\"{dateTime:yyyy-MM-ddTHH:mm:ssZ}\")"));
						}
					}	
				}

				foreach (var child in node)
				{
					EnsureDatetimeFieldsToISODate(child);
				}

				return node;
			}
			catch
			{
				return node;
			}
		}

		public static bool TryParseDateTime(string dateTimeString, out DateTime dateTime)
		{
			if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
			{
				return true;
			}
			else
			{
				var availableFormats = new[]
				{
					"yyyy-MM-ddTHH:mm:ss.fffZ",
					"yyyy-MM-ddTHH:mm:ssZ",
					"dd/MM/yyyy HH:mm:ssZ"
				};

				foreach (var pattern in availableFormats)
				{
					if (DateTime.TryParse(dateTimeString, new DateTimeFormatInfo { LongTimePattern = pattern }, out dateTime))
					{
						return true;
					}
				}

				dateTime = new DateTime();
				return false;
			}
		}

		#endregion
	}
}