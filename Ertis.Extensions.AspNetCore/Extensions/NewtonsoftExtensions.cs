using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Ertis.Core.Collections;
using Ertis.Extensions.AspNetCore.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ertis.Extensions.AspNetCore.Extensions
{
	public static class NewtonsoftExtensions
	{
		#region Methods

		public static bool TryGetValue<T>(this JToken jToken, out T value)
		{
			if (jToken == null) 
				throw new ArgumentNullException(nameof(jToken));

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

		public static dynamic ExecuteSelectQuery<T>(this IPaginationCollection<T> paginationCollection, IDictionary<string, bool> selectFields)
		{
			if (paginationCollection?.Items == null || selectFields == null || !selectFields.Any())
			{
				return paginationCollection;
			}

			bool isInclude = selectFields.Values.Any(x => x);
			bool isExclude = selectFields.Values.Any(x => !x);
			if (isInclude && isExclude)
			{
				throw new SelectQueryProjectionException();	
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
				if (attributeValue != null && selectFields.ContainsKey(attributeValue))
				{
					isSelected = selectFields[attributeValue];
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

					if (!jsonFieldNameDictionary.ContainsKey(propertyInfo.Name))
					{
						jsonFieldNameDictionary.Add(propertyInfo.Name, attributeValue);
					}
				}
			}
			
			if (isExclude)
			{
				var exludedProperties = new List<PropertyInfo>();
				exludedProperties.AddRange(selectedProperties);
				selectedProperties.Clear();
				selectedProperties.AddRange(properties.Where(propertyInfo => !exludedProperties.Contains(propertyInfo)));
			}

			List<ExpandoObject> projectinatedList = new List<ExpandoObject>();
			foreach (var item in paginationCollection.Items)
			{
				dynamic expandoObject = new ExpandoObject();
				IDictionary<string, object> expandoObjectDictionary = expandoObject as IDictionary<string, object>;
				foreach (var propertyInfo in selectedProperties)
				{
					var propertyName = jsonFieldNameDictionary.ContainsKey(propertyInfo.Name)
						? jsonFieldNameDictionary[propertyInfo.Name]
						: propertyInfo.Name;
					
					expandoObjectDictionary.Add(propertyName, propertyInfo.GetValue(item));
				}
				
				projectinatedList.Add(expandoObject);
			}

			return new PaginationCollection<dynamic>
			{
				Count = paginationCollection.Count,
				Items = projectinatedList
			};
		}

		#endregion
	}
}