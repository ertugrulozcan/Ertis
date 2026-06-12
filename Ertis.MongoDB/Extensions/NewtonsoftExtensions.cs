using System.Dynamic;
using System.Reflection;
using Ertis.Core.Collections;
using Ertis.MongoDB.Exceptions;
using Newtonsoft.Json;

namespace Ertis.MongoDB.Extensions;

// ReSharper disable once UnusedType.Global
public static class NewtonsoftExtensions
{
	#region Methods
	
	// ReSharper disable once UnusedMember.Global
	public static dynamic ExecuteSelectQuery<T>(this IPaginationCollection<T> paginationCollection, IDictionary<string, bool>? selectFields)
	{
		if (paginationCollection.Items == null || selectFields == null || !selectFields.Any())
		{
			return paginationCollection;
		}
		
		var isInclude = selectFields.Values.Any(x => x);
		var isExclude = selectFields.Values.Any(x => !x);
		if (isInclude && isExclude)
		{
			throw new SelectQueryInclusionException();	
		}
		
		var selectedProperties = new List<PropertyInfo>();
		var jsonFieldNameDictionary = new Dictionary<string, string>();
		
		var properties = typeof(T).GetProperties();
		foreach (var propertyInfo in properties)
		{
			bool? isSelected = null;
			var jsonPropertyAttribute = propertyInfo.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(JsonPropertyAttribute));
			var constructorArguments = jsonPropertyAttribute?.ConstructorArguments.ToList();
			var attributeValue = constructorArguments?.Select(x => x.Value?.ToString()).FirstOrDefault(x => !string.IsNullOrEmpty(x));
			if (attributeValue != null && selectFields.TryGetValue(attributeValue, out var value))
			{
				isSelected = value;
			}
			
			if (attributeValue != null && !string.IsNullOrEmpty(attributeValue))
			{
				/*
					In projections that explicitly include fields, the _id field is the only field that you can explicitly exclude.
					In projections that explicitly excludes fields, the _id field is the only field that you can explicitly include; however, the _id field is included by default.
				*/
				
				if (attributeValue == "_id")
				{
					selectedProperties.Add(propertyInfo);
					jsonFieldNameDictionary.Add(propertyInfo.Name, attributeValue);
					continue;
				}
				
				if (isInclude)
				{
					if (isSelected != null && isSelected.Value)
					{
						selectedProperties.Add(propertyInfo);	
					}
				}
				else
				{
					if (isSelected != null && !isSelected.Value)
					{
						selectedProperties.Add(propertyInfo);	
					}
				}
				
				jsonFieldNameDictionary.TryAdd(propertyInfo.Name, attributeValue);
			}
		}
		
		if (isExclude)
		{
			var excludedProperties = new List<PropertyInfo>();
			excludedProperties.AddRange(selectedProperties);
			selectedProperties.Clear();
			selectedProperties.AddRange(properties.Where(propertyInfo => !excludedProperties.Contains(propertyInfo)));
		}
		
		var projectionList = new List<ExpandoObject>();
		foreach (var item in paginationCollection.Items)
		{
			dynamic expandoObject = new ExpandoObject();
			if (expandoObject is IDictionary<string, object?> expandoObjectDictionary)
			{
				foreach (var propertyInfo in selectedProperties)
				{
					var propertyName = jsonFieldNameDictionary.TryGetValue(propertyInfo.Name, out var value)
						? value
						: propertyInfo.Name;
					
					expandoObjectDictionary.Add(propertyName, propertyInfo.GetValue(item));
				}
				
				projectionList.Add(expandoObject);
			}
		}
		
		return new PaginationCollection<dynamic>
		{
			Count = paginationCollection.Count,
			Items = projectionList
		};
	}
	
	#endregion
}