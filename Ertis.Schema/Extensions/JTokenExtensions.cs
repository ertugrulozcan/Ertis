using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Extensions;

public static class JTokenExtensions
{
	#region Methods
	
	public static IDictionary<string, object?> ToDictionary(this JToken jToken)
	{
		var dictionary = new Dictionary<string, object?>();
		
		foreach (var childToken in jToken.Children())
		{
			if (childToken is JProperty jProperty)
			{
				var propertyName = jProperty.Name;
				dictionary.Add(propertyName, ToDictionaryCore(jProperty.Value));
			}
		}
		
		return dictionary;
	}
	
	private static object? ToDictionaryCore(JToken jToken)
	{
		switch (jToken)
		{
			case JProperty jProperty:
			{
				return jProperty.Value is JValue jValue ? jValue.Value : ToDictionaryCore(jProperty.Value);
			}
			case JValue jValue:
			{
				return jValue.Value;
			}
			case JObject jObject:
			{
				return jObject.Children().ToDictionary(childToken => childToken.GetFullPath(), ToDictionaryCore);
			}
			case JArray jArray:
			{
				return jArray.Select(ToDictionaryCore).ToArray();
			}
			default:
			{
				throw new Exception("Unknown json node in ToDictionaryCore");
			}
		}
	}
	
	public static string GetFullPath(this JToken jToken)
	{
		if (jToken.Path.StartsWith("['") && jToken.Path.EndsWith("']"))
		{
			return string.Join('.', jToken.AncestorsAndSelf()
				.OfType<JProperty>()
				.Select(p => p.Name)
				.Reverse());
		}
		
		return jToken.Path;
	}
	
	#endregion
}