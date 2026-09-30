using System.Dynamic;
using System.Text.Json;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable OutParameterValueIsAlwaysDiscarded.Global
namespace Ertis.Schema.Dynamics;

// ReSharper disable once UnusedType.Global
public class DynamicObject : ICloneable, IDisposable
{
	#region Properties
	
	private IDictionary<string, object?> PropertyDictionary { get; init; }
	
	#endregion
	
	#region Indexer Overloading
	
	public object? this[string path]
	{
		get => this.GetValue(path);
		set => this.SetValue(path, value);
	}
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	private DynamicObject()
	{
		this.PropertyDictionary = new Dictionary<string, object?>();
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	public DynamicObject(object obj)
	{
		if (obj is DynamicObject dynamicObject)
		{
			this.PropertyDictionary = dynamicObject.PropertyDictionary;
		}
		else
		{
			this.PropertyDictionary = DynamicValues.FromObject(obj);
		}
	}
	
	#endregion
	
	#region Methods
	
	public static DynamicObject Create(IDictionary<string, object?> dictionary)
	{
		return new DynamicObject
		{
			PropertyDictionary = DynamicValues.FromObject(dictionary)
		};
	}
	
	public static DynamicObject Parse(string json)
	{
		return new DynamicObject
		{
			PropertyDictionary = DynamicValues.FromJson(json)
		};
	}
	
	internal static DynamicObject FromJsonElement(JsonElement element)
	{
		return new DynamicObject
		{
			PropertyDictionary = DynamicValues.FromJsonObject(element)
		};
	}
	
	public static T? Cast<T>(dynamic obj) where T : class
	{
		if (obj == null)
		{
			return null;
		}
		
		var dynamicObject = new DynamicObject(obj);
		return dynamicObject.Deserialize<T>();
	}
	
	public static object? Cast(dynamic obj, Type type)
	{
		if (obj == null)
		{
			return null;
		}
		
		var dynamicObject = new DynamicObject(obj);
		return dynamicObject.Deserialize(type);
	}
	
	public IDictionary<string, object?> ToDictionary()
	{
		return this.PropertyDictionary;
	}
	
	public string ToJson()
	{
		var dynamicObject = ToDynamic();
		return JsonSerializer.Serialize(dynamicObject);
	}
	
	public string Serialize()
	{
		return this.ToJson();
	}
	
	public T? Deserialize<T>()
	{
		return JsonSerializer.Deserialize<T>(this.ToJson());
	}
	
	public object? Deserialize(Type type)
	{
		return JsonSerializer.Deserialize(this.ToJson(), type);
	}
	
	public dynamic ToDynamic()
	{
		IDictionary<string, object?> expandoDictionary = new ExpandoObject();
		foreach (var pair in this.PropertyDictionary)
		{
			if (pair.Value is IDictionary<string, object?> childDictionary)
			{
				expandoDictionary.Add(new KeyValuePair<string, object?>(pair.Key, childDictionary.ToDynamic()));
			}
			else
			{
				expandoDictionary.Add(pair);   
			}
		}
		
		dynamic dynamicObject = (ExpandoObject) expandoDictionary;
		return dynamicObject;
	}
	
	public object? GetValue(string path)
	{
		return GetValueCore(path, this.PropertyDictionary);
	}
	
	public object? GetValue(string path, object defaultValue)
	{
		try
		{
			return GetValueCore(path, this.PropertyDictionary);
		}
		catch (UndefinedFieldException)
		{
			return defaultValue;
		}
	}
	
	public T? GetValue<T>(string path)
	{
		var value = GetValueCore(path, this.PropertyDictionary);
		return value == null ? default : (T?) ConvertValue(value, typeof(T));
	}
	
	private static object? ConvertValue(object value, Type type)
	{
		if (value is IDictionary<string, object?> dictionary && !type.IsInstanceOfType(value))
		{
			return Create(dictionary).Deserialize(type);
		}
		
		if (type.IsArray && value is Array array)
		{
			var itemType = type.GetElementType()!;
			var typedArray = Array.CreateInstance(itemType, array.Length);
			for (var i = 0; i < array.Length; i++)
			{
				var item = array.GetValue(i);
				typedArray.SetValue(item == null ? null : ConvertValue(item, itemType), i);
			}
			
			return typedArray;
		}
		
		try
		{
			return DynamicValues.ConvertTo(value, type);
		}
		catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException or ArgumentException)
		{
			return value;
		}
	}
	
	public T? GetValue<T>(string path, T defaultValue)
	{
		try
		{
			return this.GetValue<T>(path);
		}
		catch (UndefinedFieldException)
		{
			return defaultValue;
		}
	}
	
	public bool TryGetValue(string path, out object? value)
	{
		try
		{
			value = this.GetValue(path);
			return true;
		}
		catch
		{
			value = null;
			return false;
		}
	}
	
	public bool TryGetValue(string path, out object? value, out Exception? exception)
	{
		try
		{
			value = this.GetValue(path);
			exception = null;
			return true;
		}
		catch (Exception ex)
		{
			value = null;
			exception = ex;
			return false;
		}
	}
	
	public bool TryGetValue<T>(string path, out T? value)
	{
		try
		{
			value = this.GetValue<T>(path);
			return true;
		}
		catch
		{
			value = default;
			return false;
		}
	}
	
	public bool TryGetValue<T>(string path, out T? value, out Exception? exception)
	{
		try
		{
			value = this.GetValue<T>(path);
			exception = null;
			return true;
		}
		catch (Exception ex)
		{
			value = default;
			exception = ex;
			return false;
		}
	}
	
	private static object? GetValueCore(string path, IDictionary<string, object?> dictionary)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("Path can not be null or empty!");
		}
		
		var segments = path.Split('.');
		var key = segments[0];
		if (dictionary.TryGetValue(key, out var value))
		{
			if (segments.Length > 1)
			{
				if (value is IDictionary<string, object?> subDictionary)
				{
					var subPath = string.Join(".", segments.Skip(1));
					return GetValueCore(subPath, subDictionary);
				}
				else
				{
					throw new UndefinedFieldException(path);
				}
			}
			else
			{
				return value;
			}   
		}
		else if (TryGetValueFromArray(key, dictionary, out var foundValue))
		{
			if (segments.Length > 1)
			{
				if (foundValue is IDictionary<string, object?> subDictionary)
				{
					var subPath = string.Join(".", segments.Skip(1));
					return GetValueCore(subPath, subDictionary);
				}
				else
				{
					throw new UndefinedFieldException(path);
				}
			}
			else
			{
				return foundValue;
			}
		}
		else
		{
			throw new UndefinedFieldException(path);
		}
	}
	
	private static bool TryGetValueFromArray(string key, IDictionary<string, object?> dictionary, out object? foundValue)
	{
		if (key.Contains('[') && key.EndsWith(']'))
		{
			var indexerStartIndex = key.IndexOf('[');
			var indexerCloseIndex = key.IndexOf(']');
			
			var originalKey = key[..indexerStartIndex];
			if (dictionary.TryGetValue(originalKey, out var value))
			{
				if (value is Array array)
				{
					var indexStr = key.Substring(indexerStartIndex + 1, indexerCloseIndex - indexerStartIndex - 1);
					if (int.TryParse(indexStr, out var index))
					{
						if (index < array.Length)
						{
							foundValue = array.GetValue(index);
							return true;
						}
						else
						{
							throw new InvalidOperationException($"Out of range (length: {array.Length}, index: {index})");
						}
					}
					else
					{
						throw new InvalidOperationException($"Array index is not valid integer ('{indexStr}')");
					}
				}
				else
				{
					throw new InvalidOperationException("Indexed node is not an array");
				}
			}
			else
			{
				throw new UndefinedFieldException(key);
			}
		}
		
		foundValue = null;
		return false;
	}
	
	public void SetValue(string path, object? value, bool createIfNotExist = false)
	{
		SetValueCore(path, value, this.PropertyDictionary, createIfNotExist);
	}
	
	public bool TrySetValue(string path, object value, out Exception? exception, bool createIfNotExist = false)
	{
		try
		{
			SetValue(path, value, createIfNotExist);
			exception = null;
			return true;
		}
		catch (Exception ex)
		{
			exception = ex;
			return false;
		}
	}
	
	private static void SetValueCore(string path, object? obj, IDictionary<string, object?> dictionary, bool createIfNotExist)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("Path can not be null or empty!");
		}
		
		var segments = path.Split('.');
		var key = segments[0];
		if (dictionary.ContainsKey(key))
		{
			var value = dictionary[key];
			if (segments.Length > 1)
			{
				if (value is IDictionary<string, object?> subDictionary)
				{
					var subPath = string.Join(".", segments.Skip(1));
					SetValueCore(subPath, obj, subDictionary, createIfNotExist);
				}
				else
				{
					throw new UndefinedFieldException(path);
				}
			}
			else
			{
				dictionary[key] = obj;
			}
		}
		else if (TrySetValueOnArray(path, dictionary, obj))
		{
			// NOP
		}
		else if (createIfNotExist)
		{
			if (segments.Length > 1)
			{
				var subDictionary = new Dictionary<string, object?>();
				dictionary.Add(key, subDictionary);
				
				var subPath = string.Join(".", segments.Skip(1));
				SetValueCore(subPath, obj, subDictionary, true);
			}
			else
			{
				dictionary.Add(key, obj);
			}
		}
		else
		{
			throw new UndefinedFieldException(path);
		}
	}
	
	private static bool TrySetValueOnArray(string key, IDictionary<string, object?> dictionary, object? setValue)
	{
		var indexerStartIndex = key.IndexOf('[');
		var indexerCloseIndex = key.IndexOf(']');
		
		if (indexerStartIndex > 0 && indexerCloseIndex > 0 && indexerStartIndex < indexerCloseIndex)
		{
			var originalKey = key[..indexerStartIndex];
			if (dictionary.TryGetValue(originalKey, out var value))
			{
				if (value is Array array)
				{
					var indexStr = key.Substring(indexerStartIndex + 1, indexerCloseIndex - indexerStartIndex - 1);
					if (int.TryParse(indexStr, out var index))
					{
						if (index < array.Length)
						{
							var indexerPath = $"{originalKey}[{index}]";
							var arrayItem = array.GetValue(index);
							if (key.Length > indexerPath.Length && arrayItem is IDictionary<string, object?> subDictionary)
							{
								SetValueCore(key[indexerPath.Length..].TrimStart('.'), setValue, subDictionary, false);
							}
							else
							{
								array.SetValue(setValue, index);
							}
							
							return true;
						}
						else
						{
							throw new InvalidOperationException($"Out of range (length: {array.Length}, index: {index})");
						}
					}
					else
					{
						throw new InvalidOperationException($"Array index is not valid integer ('{indexStr}')");
					}
				}
				else
				{
					throw new InvalidOperationException("Indexed node is not an array");
				}
			}
			else
			{
				throw new UndefinedFieldException(key);
			}
		}
		
		return false;
	}
	
	public void RemoveProperty(string path)
	{
		RemovePropertyCore(path, this.PropertyDictionary);
	}
	
	private static void RemovePropertyCore(string path, IDictionary<string, object?> dictionary)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("Path can not be null or empty!");
		}
		
		var segments = path.Split('.');
		var key = segments[0];
		if (dictionary.ContainsKey(key))
		{
			var value = dictionary[key];
			if (segments.Length > 1)
			{
				if (value is IDictionary<string, object?> subDictionary)
				{
					var subPath = string.Join(".", segments.Skip(1));
					RemovePropertyCore(subPath, subDictionary);
				}
			}
			else
			{
				dictionary.Remove(key);
			}
		}
	}
	
	public bool ContainsProperty(string path)
	{
		if (!this.TryGetValue(path, out _, out var exception))
		{
			if (exception is UndefinedFieldException)
			{
				// ReSharper disable once DuplicatedStatements
				return false;
			}
			else if (exception != null)
			{
				throw exception;
			}
			
			return false;
		}
		
		return true;
	}
	
	#endregion
	
	#region ICloneable
	
	public object Clone()
	{
		return new DynamicObject
		{
			PropertyDictionary = (Dictionary<string, object?>) DynamicValues.DeepCopy(this.PropertyDictionary)!
		};
	}
	
	#endregion
	
	#region Disposing
	
	private bool _disposedValue;
	
	~DynamicObject() => this.Dispose(false);
	
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	
	private void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				this.PropertyDictionary.Clear();
			}
			
			// Free unmanaged resources (unmanaged objects) and override finalizer, set large fields to null
			_disposedValue = true;
		}
	}
	
	#endregion
}