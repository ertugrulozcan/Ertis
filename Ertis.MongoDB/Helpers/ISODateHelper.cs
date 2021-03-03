using System;
using Newtonsoft.Json.Linq;

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
			catch (Exception ex)
			{
				Console.WriteLine(ex);
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
						if (DateTime.TryParse(node.Value<string>(), out var dateTime))
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
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return node;
			}
		}

		#endregion
	}
}