using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Ertis.MongoDB.Helpers
{
	public static class QueryHelper
	{
		#region Methods

		public static string EnsureObjectIdsAndISODates(string json)
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
					return EnsureObjectIdsAndISODates(jToken).ToString();	
				}

				return json;
			}
			catch
			{
				return json;
			}
		}

		private static JToken EnsureObjectIdsAndISODates(JToken node)
		{
			if (node == null)
			{
				return null;
			}

			try
			{
				if (node is JValue jValue)
				{
					if (node.Path == "_id" || node.Path.StartsWith("_id."))
					{
						var nodeValue = node.Value<string>();
						if (node.Type == JTokenType.String && ObjectId.TryParse(nodeValue, out _))
						{
							jValue.Replace(new JRaw($"ObjectId(\"{nodeValue}\")"));
						}	
					}
					else if (node.Type == JTokenType.String || node.Type == JTokenType.Date)
					{
						if (ISODateHelper.TryParseDateTime(node.Value<string>(), out var dateTime))
						{
							jValue.Replace(new JRaw($"ISODate(\"{dateTime:yyyy-MM-ddTHH:mm:ssZ}\")"));
						}
					}
				}

				foreach (var child in node)
				{
					EnsureObjectIdsAndISODates(child);
				}

				return node;
			}
			catch
			{
				return node;
			}
		}

		#endregion
	}
}