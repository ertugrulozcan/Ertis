using System.Collections.Generic;
using Ertis.Extensions.AspNetCore.Extensions;
using Newtonsoft.Json.Linq;

namespace Ertis.Extensions.AspNetCore.Helpers
{
	public static class QueryHelper
	{
		#region Methods

		public static string ExtractWhereQuery(dynamic body)
		{
			if (body == null)
			{
				return null;
			}
			
			var root = Newtonsoft.Json.JsonConvert.DeserializeObject(body);
			if (root is JObject rootNode && rootNode.TryGetValue("where", out var whereNode))
			{
				return whereNode.ToString();
			}

			return null;
		}

		public static Dictionary<string, bool> ExtractSelectFields(dynamic body)
		{
			var fieldDictionary = new Dictionary<string, bool>();
			
			if (body == null)
			{
				return fieldDictionary;
			}
			
			var root = Newtonsoft.Json.JsonConvert.DeserializeObject(body);
			if (root is JObject rootNode)
			{
				if (rootNode.TryGetValue("select", out var jToken))
				{
					if (jToken is JObject selectNode)
					{
						foreach (var (key, value) in selectNode)
						{
							if (value != null)
							{
								var intValue = 0;
								if (value.TryGetValue(out bool boolValue) || value.TryGetValue(out intValue))
								{
									fieldDictionary.Add(key, boolValue || intValue == 1);	
								}
							}
						}	
					}
				}
			}

			return fieldDictionary;
		}

		#endregion
	}
}