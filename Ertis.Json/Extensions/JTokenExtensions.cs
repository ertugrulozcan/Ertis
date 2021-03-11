using System.Linq;
using Newtonsoft.Json.Linq;

namespace Ertis.Json.Extensions
{
	public static class JTokenExtensions
	{
		#region Methods

		public static string GetPathFromAnnotation(this JToken jToken)
		{
			var annotations = jToken.Annotations<object>();
			var annotation = annotations.FirstOrDefault(x => x.GetType().Name == "JTokenPathAnnotation");
			var type = annotation?.GetType();
			var fields = type?.GetFields();
			var basePathProperty = fields?.FirstOrDefault(x => x.Name == "BasePath");
			var basePath = basePathProperty?.GetValue(annotation);
			return basePath?.ToString();
		}

		#endregion
	}
}