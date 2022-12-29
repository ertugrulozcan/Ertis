using MongoDB.Bson;
using Newtonsoft.Json.Linq;

// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.MongoDB.Helpers
{
	public static class ObjectIdHelper
	{
		#region Methods

		public static string EnsureObjectIds(string json)
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
					return EnsureObjectIds(jToken).ToString();	
				}

				return json;
			}
			catch
			{
				return json;
			}
		}
		
		public static JToken EnsureObjectIds(JToken node)
		{
			if (node == null)
			{
				return null;
			}

			try
			{
				if (node is JValue jValue && (node.Path == "_id" || node.Path.StartsWith("_id.")))
				{
					var nodeValue = node.Value<string>();
					if (node.Type == JTokenType.String && ObjectId.TryParse(nodeValue, out _))
					{
						jValue.Replace(new JRaw($"ObjectId(\"{nodeValue}\")"));
					}	
				}

				foreach (var child in node)
				{
					EnsureObjectIds(child);
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