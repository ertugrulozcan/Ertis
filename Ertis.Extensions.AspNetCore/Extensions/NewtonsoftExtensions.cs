using Newtonsoft.Json.Linq;

namespace Ertis.Extensions.AspNetCore.Extensions;

public static class NewtonsoftExtensions
{
	#region Methods
	
	public static bool TryGetValue<T>(this JToken jToken, out T? value)
	{
        ArgumentNullException.ThrowIfNull(jToken);
		
        try
		{
			value = jToken.Value<T>();
			return true;
		}
		catch
		{
			value = default;
			return false;
		}
	}
	
	#endregion
}