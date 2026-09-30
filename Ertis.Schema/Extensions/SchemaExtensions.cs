using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;
using Ertis.Schema.Types.CustomTypes;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Schema.Extensions;

public static class SchemaExtensions
{
	#region Schema & Field Methods
	
	/// <summary>
	/// Return field path without schema segment
	/// </summary>
	/// <param name="fieldInfo"></param>
	/// <param name="schema"></param>
	/// <returns></returns>
	public static string GetSelfPath(this IFieldInfo fieldInfo, ISchema schema)
	{
		var path = fieldInfo.Path;
		
		// The path starts with the schema segment only when the schema itself is the root of the field (e.g. an ObjectFieldInfo schema)
		var root = fieldInfo;
		while (root.Parent != null)
		{
			root = root.Parent;
		}
		
		if (ReferenceEquals(root, schema) && path.StartsWith($"{schema.Slug}.", StringComparison.Ordinal))
		{
			path = path[(schema.Slug.Length + 1)..];
		}
		
		return path;
	}
	
	#endregion
	
	#region Validation Methods
	
	public static void Validate(this ISchema schema, out Exception? exception)
	{
		schema.ValidateProperties(out exception);
	}
	
	internal static bool ValidateProperties(this ISchema schema, out Exception? exception)
	{
		foreach (var fieldInfo in schema.Properties)
		{
			if (!fieldInfo.ValidateSchema(out exception))
			{
				return false;
			}
		}
		
		schema.CheckPropertiesUniqueness(out exception);
		
		var uniqueProperties = schema.GetUniqueProperties();
		foreach (var uniqueProperty in uniqueProperties)
		{
			if (uniqueProperty.IsAnArrayItem(out _))
			{
				exception = new SchemaValidationException($"The unique constraints could not use in arrays. Use the 'uniqueBy' feature instead of. ('{uniqueProperty.Name}')");
				return false;
			}
		}
		
		return exception == null;
	}
	
	public static bool ValidateData(this ISchema schema, DynamicObject model, IValidationContext validationContext)
	{
		try
		{
			var rootObjectFieldInfo = schema as ObjectFieldInfo ?? ObjectFieldInfo.CreateDetachedRoot(schema);
			
			var isValidContent = rootObjectFieldInfo.ValidateContent(model, validationContext);
			
			schema.SetDefaultValues(model);
			schema.SetConstants(model);
			schema.SetFormatPatterns(model);
			schema.SetDateTimes(model);
			
			if (!isValidContent && !validationContext.Errors.Any())
			{
				validationContext.Errors.Add(new FieldValidationException("Unknown validation error", rootObjectFieldInfo));
			}
			
			return isValidContent;
		}
		catch (FieldValidationException ex)
		{
			validationContext.Errors.Add(ex);
			return false;
		}
	}
	
	#endregion
	
	#region Merge Methods
	
	public static IEnumerable<IFieldInfo> MergeTypeProperties(this ISchema contentType1, ISchema contentType2, bool allowDuplicateFieldWithBaseType = false)
	{
		var properties = new List<IFieldInfo>();
		properties.AddRange(contentType1.Properties);
		
		foreach (var fieldInfo in contentType2.Properties)
		{
			var currentFieldInfo = properties.FirstOrDefault(x => x.Name == fieldInfo.Name);
			if (currentFieldInfo != null)
			{
				if (fieldInfo.IsVirtual)
				{
					if (currentFieldInfo.Type != fieldInfo.Type)
					{
						throw new SchemaValidationException($"The field type cannot be overwritten on virtual fields. ({fieldInfo.Name})");
					}
				}
				else if (!allowDuplicateFieldWithBaseType)
				{
					throw new SchemaValidationException($"'{fieldInfo.Name}' field is already exist in base type.");   
				}
				else
				{
					var index = properties.IndexOf(currentFieldInfo);
					properties.RemoveAt(index);
					properties.Insert(index, fieldInfo);
				}
			}
			else
			{
				properties.Add(fieldInfo);
			}
		}
		
		return properties;
	}
	
	#endregion
	
	#region Schema Tree Methods
	
	public static IFieldInfo? FindField(this ISchema schema, string path)
	{
		return FindFieldCore(schema.Properties, path);
	}
	
	private static IFieldInfo? FindFieldCore(IEnumerable<IFieldInfo> properties, string path)
	{
		foreach (var property in properties)
		{
			var found = FindFieldCore(property, path);
			if (found != null)
			{
				return found;
			}
		}
		
		return null;
	}
	
	private static IFieldInfo? FindFieldCore(IFieldInfo property, string path)
	{
		if (property.Type == FieldType.@object && property is ObjectFieldInfo objectFieldInfo)
		{
			return FindFieldCore(objectFieldInfo.Properties, path);
		}
		else if (property.Type == FieldType.array && property is ArrayFieldInfo arrayFieldInfo)
		{
			return arrayFieldInfo.ItemSchema == null ? null : FindFieldCore(arrayFieldInfo.ItemSchema, path);
		}
		else
		{
			return property.Path == path ? property : null;
		}
	}
	
	#endregion
	
	#region Uniqueness Methods
	
	private static bool CheckPropertiesUniqueness(this ISchema schema, out Exception? exception)
	{
		var fieldInfos = schema.Properties;
		var distinctCount = fieldInfos.Select(x => x.Name).Distinct().Count();
		if (fieldInfos.Count != distinctCount)
		{
			if (schema is IFieldInfo fieldInfo)
			{
				exception = new FieldValidationException("Duplicate property declaration in a field info. Property names are must be unique.", fieldInfo);    
			}
			else
			{
				exception = new SchemaValidationException("Duplicate property declaration in schema. Property names are must be unique.");
			}
			
			return false;
		}
		
		foreach (var fieldInfo in fieldInfos)
		{
			if (fieldInfo is ISchema subObjectSchema)
			{
				var isValid = CheckPropertiesUniqueness(subObjectSchema, out exception);
				if (!isValid)
				{
					return false;
				}
			}
		}
		
		exception = null;
		return true;
	}
	
	#endregion
	
	#region Unique Property Methods
	
	public static IEnumerable<IFieldInfo> GetUniqueProperties(this ISchema schema)
	{
		var uniqueProperties = new List<IFieldInfo>();
		var fieldInfos = schema.Properties;
		foreach (var fieldInfo in fieldInfos)
		{
			uniqueProperties.AddRange(GetUniqueProperties(fieldInfo));
		}
		
		return uniqueProperties;
	}
	
	private static IEnumerable<IFieldInfo> GetUniqueProperties(IFieldInfo fieldInfo)
	{
		var uniqueProperties = new List<IFieldInfo>();
		switch (fieldInfo)
		{
			case IPrimitiveType { IsUnique: true }:
				uniqueProperties.Add(fieldInfo);
				break;
			case ObjectFieldInfo objectFieldInfo:
			{
				foreach (var property in objectFieldInfo.Properties)
				{
					uniqueProperties.AddRange(GetUniqueProperties(property));
				}
				break;
			}
			case ArrayFieldInfo { ItemSchema: not null } arrayFieldInfo:
				uniqueProperties.AddRange(GetUniqueProperties(arrayFieldInfo.ItemSchema));
				break;
		}
		
		return uniqueProperties;
	}
	
	#endregion
	
	#region Reference Content Methods
	
	public static IEnumerable<ReferenceFieldInfo> GetReferenceProperties(this ISchema schema)
	{
		var referenceProperties = new List<ReferenceFieldInfo>();
		var fieldInfos = schema.Properties;
		foreach (var fieldInfo in fieldInfos)
		{
			referenceProperties.AddRange(GetReferenceProperties(fieldInfo));
		}
		
		return referenceProperties;
	}
	
	private static IEnumerable<ReferenceFieldInfo> GetReferenceProperties(IFieldInfo fieldInfo)
	{
		var referenceProperties = new List<ReferenceFieldInfo>();
		switch (fieldInfo.Type)
		{
			case FieldType.reference when fieldInfo is ReferenceFieldInfo referenceFieldInfo:
				referenceProperties.Add(referenceFieldInfo);
				break;
			case FieldType.@object when fieldInfo is ObjectFieldInfo objectFieldInfo:
			{
				foreach (var property in objectFieldInfo.Properties)
				{
					referenceProperties.AddRange(GetReferenceProperties(property));
				}
				break;
			}
			case FieldType.array when fieldInfo is ArrayFieldInfo { ItemSchema: not null } arrayFieldInfo:
				referenceProperties.AddRange(GetReferenceProperties(arrayFieldInfo.ItemSchema));
				break;
		}
		
		return referenceProperties;
	}
	
	#endregion
	
	#region Post Validation Methods
	
	/// <summary>
	/// Returns the fields that the post validation steps (default values, constants, format patterns, dates) apply to:
	/// the top-level fields and the fields of the nested objects that exist in the model
	/// </summary>
	private static IEnumerable<IFieldInfo> GetApplicableFields(ISchema schema, IEnumerable<IFieldInfo> properties, DynamicObject model)
	{
		foreach (var fieldInfo in properties)
		{
			yield return fieldInfo;
			
			if (fieldInfo is ObjectFieldInfo objectFieldInfo && model.TryGetValue(fieldInfo.GetSelfPath(schema), out var value) && value is IDictionary<string, object?>)
			{
				foreach (var childFieldInfo in GetApplicableFields(schema, objectFieldInfo.Properties, model))
				{
					yield return childFieldInfo;
				}
			}
		}
	}
	
	#endregion
	
	#region DefaultValue Methods
	
	private static void SetDefaultValues(this ISchema schema, DynamicObject model)
	{
		foreach (var fieldInfo in GetApplicableFields(schema, schema.Properties, model))
		{
			if (fieldInfo is IHasDefault hasDefault)
			{
				var defaultValue = hasDefault.GetDefaultValue();
				if (defaultValue != null)
				{
					var path = fieldInfo.GetSelfPath(schema);
					if (!model.TryGetValue(path, out var currentValue, out _) || currentValue == null)
					{
						model.TrySetValue(path, defaultValue, out _, true);
					}
				}
			}
		}
	}
	
	#endregion
	
	#region Constant Methods
	
	private static void SetConstants(this ISchema schema, DynamicObject model)
	{
		foreach (var fieldInfo in GetApplicableFields(schema, schema.Properties, model))
		{
			if (fieldInfo is ConstantFieldInfo { Value: not null } constantFieldInfo)
			{
				var path = fieldInfo.GetSelfPath(schema);
				model.TrySetValue(path, constantFieldInfo.Value, out _, true);
			}
		}
	}
	
	#endregion
	
	#region DateTime Methods
	
	private static void SetDateTimes(this ISchema schema, DynamicObject model)
	{
		foreach (var fieldInfo in GetApplicableFields(schema, schema.Properties, model))
		{
			if (fieldInfo is IDateTimeFieldInfo)
			{
				var path = fieldInfo.GetSelfPath(schema);
				if (model.TryGetValue<string>(path, out var stringValue, out _) && DateTime.TryParse(stringValue, out var dateValue))
				{
					model.TrySetValue(path, dateValue, out _, true);
				}
			}
		}
	}
	
	#endregion
	
	#region FormatPattern Methods
	
	private static void SetFormatPatterns(this ISchema schema, DynamicObject model)
	{
		foreach (var fieldInfo in GetApplicableFields(schema, schema.Properties, model))
		{
			if (fieldInfo is StringFieldInfo stringFieldInfo && !string.IsNullOrEmpty(stringFieldInfo.FormatPattern))
			{
				if (stringFieldInfo.TryFormat(model, out var formattedString) && !string.IsNullOrEmpty(formattedString))
				{
					var path = fieldInfo.GetSelfPath(schema);
					model.TrySetValue(path, formattedString, out _, true);
				}
			}
		}
	}
	
	#endregion
}