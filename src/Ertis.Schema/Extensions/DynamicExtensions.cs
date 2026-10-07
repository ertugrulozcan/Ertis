using System.Dynamic;
using Ertis.Schema.Dynamics;

namespace Ertis.Schema.Extensions;

public static class DynamicExtensions
{
	#region Methods
	
	public static IDictionary<string, object?> ToDictionary(this object model)
	{
		return DynamicValues.FromObject(model);
	}
	
	public static dynamic ToDynamic(this IDictionary<string, object?> dictionary)
	{
		IDictionary<string, object?> expando = new ExpandoObject();
		foreach (var pair in dictionary)
		{
			if (pair.Value is IDictionary<string, object?> childDictionary)
			{
				expando.Add(new KeyValuePair<string, object?>(pair.Key, childDictionary.ToDynamic()));
			}
			else
			{
				expando.Add(pair);   
			}
		}
		
		return (ExpandoObject) expando;
	}
	
	#endregion
}