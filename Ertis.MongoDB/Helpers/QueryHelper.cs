using System.Linq;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Ertis.MongoDB.Helpers;

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
			switch (node)
			{
				case JValue jValue when node.Path == "_id" || node.Path.StartsWith("_id."):
				{
					var nodeValue = node.Value<string>();
					if (node.Type == JTokenType.String && ObjectId.TryParse(nodeValue, out _))
					{
						jValue.Replace(new JRaw($"ObjectId(\"{nodeValue}\")"));
					}
					
					break;
				}
				case JValue jValue:
				{
					if (node.Type is JTokenType.String or JTokenType.Date)
					{
						if (ISODateHelper.TryParseDateTime(node.Value<string>(), out var dateTime))
						{
							jValue.Replace(new JRaw($"ISODate(\"{dateTime:yyyy-MM-ddTHH:mm:ssZ}\")"));
						}
					}
					
					break;
				}
				case JArray jArray:
				{
					var children = jArray.Children().ToArray();
					foreach (var item in children)
					{
						EnsureObjectIdsAndISODates(item);
					}
					
					break;
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