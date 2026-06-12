using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Extensions;

public static class JTokenExtensions
{
	#region Methods
	
	extension(JToken jToken)
	{
		public IDictionary<string, object?> ToDictionary()
		{
			var dictionary = new Dictionary<string, object?>();
			foreach (var childToken in jToken.Children())
			{
				if (childToken is JProperty jProperty)
				{
					var propertyName = jProperty.Name;
					dynamic dynamicObject = jProperty.Value;
					dictionary.Add(propertyName, DynamicExtensions.ToDictionaryCore(dynamicObject));
				}
			}
			
			return dictionary;
		}
		
		public string GetFullPath()
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
	}
	
	#endregion
}