using MongoDB.Bson;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.MongoDB.Helpers;

// ReSharper disable once UnusedType.Global
public static class ObjectIdHelper
{
	#region Methods
	
	public static string? EnsureObjectIds(string json)
	{
		return QueryHelper.Ensure(json, convertObjectIds: true, convertDates: false);
	}
	
	public static JToken? EnsureObjectIds(JToken? node)
	{
		if (node == null)
		{
			return null;
		}
		
		try
		{
			if (node is JValue jValue && (node.Path == "_id" || node.Path.StartsWith("_id.") || node.Path.EndsWith("._id")))
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