using System.Globalization;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable OutParameterValueIsAlwaysDiscarded.Global
namespace Ertis.Schema.Dynamics;

// ReSharper disable once UnusedType.Global
/// <summary>
/// Serialized by its converter with any JsonSerializerOptions (no registration needed): without it, System.Text.Json
/// would write a DynamicObject as {} (it has no public properties)
/// </summary>
[JsonConverter(typeof(DynamicObjectJsonConverter))]
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
	
	/// <summary>
	/// Reads the json value at the current token of the reader (a json which is not an object results in an empty object)
	/// </summary>
	internal static DynamicObject Read(ref Utf8JsonReader reader)
	{
		return new DynamicObject
		{
			PropertyDictionary = DynamicValues.ReadValue(ref reader) as Dictionary<string, object?> ?? new Dictionary<string, object?>()
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
		return JsonSerializer.Serialize(this.PropertyDictionary);
	}
	
	public string Serialize()
	{
		return this.ToJson();
	}
	
	public T? Deserialize<T>()
	{
		return JsonSerializer.Deserialize<T>(JsonSerializer.SerializeToUtf8Bytes(this.PropertyDictionary));
	}
	
	public object? Deserialize(Type type)
	{
		return JsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(this.PropertyDictionary), type);
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
		var exception = TryResolve(path, this.PropertyDictionary, out var value);
		return exception switch
		{
			null => value,
			UndefinedFieldException => defaultValue,
			_ => throw exception
		};
	}
	
	public T? GetValue<T>(string path)
	{
		return ConvertValue<T>(GetValueCore(path, this.PropertyDictionary));
	}
	
	public T? GetValue<T>(string path, T defaultValue)
	{
		var exception = TryResolve(path, this.PropertyDictionary, out var value);
		return exception switch
		{
			null => ConvertValue<T>(value),
			UndefinedFieldException => defaultValue,
			_ => throw exception
		};
	}
	
	public bool TryGetValue(string path, out object? value)
	{
		return TryResolve(path, this.PropertyDictionary, out value) == null;
	}
	
	public bool TryGetValue(string path, out object? value, out Exception? exception)
	{
		exception = TryResolve(path, this.PropertyDictionary, out value);
		return exception == null;
	}
	
	public bool TryGetValue<T>(string path, out T? value)
	{
		return this.TryGetValue(path, out value, out _);
	}
	
	public bool TryGetValue<T>(string path, out T? value, out Exception? exception)
	{
		exception = TryResolve(path, this.PropertyDictionary, out var rawValue);
		if (exception == null)
		{
			try
			{
				value = ConvertValue<T>(rawValue);
				return true;
			}
			catch (Exception ex)
			{
				exception = ex;
			}
		}
		
		value = default;
		return false;
	}
	
	/// <summary>
	/// Gets the value of the path in a dictionary of the object model, without throwing
	/// </summary>
	internal static bool TryGetValue(IDictionary<string, object?> dictionary, string path, out object? value)
	{
		return TryResolve(path, dictionary, out value) == null;
	}
	
	private static T? ConvertValue<T>(object? value)
	{
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
	
	private static object? GetValueCore(string path, IDictionary<string, object?> dictionary)
	{
		var exception = TryResolve(path, dictionary, out var value);
		return exception == null ? value : throw exception;
	}
	
	/// <summary>
	/// Resolves the path; returns the exception (without throwing it) when the path can not be resolved.
	/// The exceptions are created instead of thrown, because the Try methods are used on the hot paths (e.g. the default values of the missing fields).
	/// </summary>
	private static Exception? TryResolve(string path, IDictionary<string, object?> dictionary, out object? value)
	{
		value = null;
		if (string.IsNullOrEmpty(path))
		{
			return new ArgumentException("Path can not be null or empty!");
		}
		
		var remaining = path.AsSpan();
		var current = dictionary;
		while (true)
		{
			var dotIndex = remaining.IndexOf('.');
			var key = dotIndex < 0 ? remaining : remaining[..dotIndex];
			
			if (!TryGetProperty(current, key, out value))
			{
				var indexerException = TryGetIndexedValue(current, key, out var isIndexed, out value);
				if (indexerException != null)
				{
					value = null;
					return indexerException;
				}
				
				if (!isIndexed)
				{
					return new UndefinedFieldException(remaining.ToString());
				}
			}
			
			if (dotIndex < 0)
			{
				return null;
			}
			
			if (value is not IDictionary<string, object?> subDictionary)
			{
				value = null;
				return new UndefinedFieldException(remaining.ToString());
			}
			
			remaining = remaining[(dotIndex + 1)..];
			if (remaining.IsEmpty)
			{
				value = null;
				return new ArgumentException("Path can not be null or empty!");
			}
			
			current = subDictionary;
		}
	}
	
	/// <summary>
	/// Looks up a property without allocating the key (the dictionaries of the object model are Dictionary&lt;string, object?&gt;)
	/// </summary>
	private static bool TryGetProperty(IDictionary<string, object?> dictionary, ReadOnlySpan<char> key, out object? value)
	{
		if (dictionary is Dictionary<string, object?> concreteDictionary && concreteDictionary.Comparer.Equals(EqualityComparer<string>.Default))
		{
			return concreteDictionary.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(key, out value);
		}
		
		return dictionary.TryGetValue(key.ToString(), out value);
	}
	
	/// <summary>
	/// Resolves an indexed key like 'tags[1]' or 'matrix[1][0]' (isIndexed is false when the key has no indexer)
	/// </summary>
	private static Exception? TryGetIndexedValue(IDictionary<string, object?> dictionary, ReadOnlySpan<char> key, out bool isIndexed, out object? value)
	{
		var indexerStart = key.IndexOf('[');
		isIndexed = indexerStart >= 0 && key[^1] == ']';
		if (!isIndexed)
		{
			value = null;
			return null;
		}
		
		if (!TryGetProperty(dictionary, key[..indexerStart], out value))
		{
			return new UndefinedFieldException(key.ToString());
		}
		
		var indexers = key[indexerStart..];
		while (!indexers.IsEmpty)
		{
			var exception = TryGetIndexedArray(value, indexers, out var array, out var index, out var indexerLength);
			if (exception != null)
			{
				return exception;
			}
			
			value = array!.GetValue(index);
			indexers = indexers[indexerLength..];
		}
		
		return null;
	}
	
	/// <summary>
	/// Validates the first indexer of the indexers ('[1]...') against the node
	/// </summary>
	private static Exception? TryGetIndexedArray(object? node, ReadOnlySpan<char> indexers, out Array? array, out int index, out int indexerLength)
	{
		array = node as Array;
		index = 0;
		indexerLength = 0;
		if (array == null)
		{
			return new InvalidOperationException("Indexed node is not an array");
		}
		
		var closeIndex = indexers.IndexOf(']');
		if (indexers[0] != '[' || closeIndex < 0)
		{
			return new InvalidOperationException($"Array index is not valid integer ('{indexers}')");
		}
		
		var indexText = indexers[1..closeIndex];
		if (!int.TryParse(indexText, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
		{
			return new InvalidOperationException($"Array index is not valid integer ('{indexText}')");
		}
		
		if (index < 0 || index >= array.Length)
		{
			return new InvalidOperationException($"Out of range (length: {array.Length}, index: {index})");
		}
		
		indexerLength = closeIndex + 1;
		return null;
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
		
		var remaining = path.AsSpan();
		var current = dictionary;
		while (true)
		{
			var dotIndex = remaining.IndexOf('.');
			var key = dotIndex < 0 ? remaining : remaining[..dotIndex];
			
			if (TryGetProperty(current, key, out var value))
			{
				if (dotIndex < 0)
				{
					current[key.ToString()] = obj;
					return;
				}
				
				if (value is not IDictionary<string, object?> subDictionary)
				{
					throw new UndefinedFieldException(remaining.ToString());
				}
				
				current = subDictionary;
			}
			else if (TrySetIndexedValue(current, key, dotIndex >= 0, obj, out var item))
			{
				if (item == null)
				{
					return;
				}
				
				// The properties of an array item are never created
				current = item;
				createIfNotExist = false;
			}
			else if (createIfNotExist && remaining.IndexOf('[') < 0)
			{
				if (dotIndex < 0)
				{
					current.Add(key.ToString(), obj);
					return;
				}
				
				var subDictionary = new Dictionary<string, object?>();
				current.Add(key.ToString(), subDictionary);
				current = subDictionary;
			}
			else
			{
				throw new UndefinedFieldException(remaining.ToString());
			}
			
			remaining = remaining[(dotIndex + 1)..];
			if (remaining.IsEmpty)
			{
				throw new ArgumentException("Path can not be null or empty!");
			}
		}
	}
	
	/// <summary>
	/// Sets the item of an indexed key like 'tags[1]' or 'matrix[1][0]'; returns false when the key has no indexer.
	/// When the path continues after the key and the item is an object, the item is returned instead to set the value in it.
	/// </summary>
	private static bool TrySetIndexedValue(IDictionary<string, object?> dictionary, ReadOnlySpan<char> key, bool hasMoreSegments, object? obj, out IDictionary<string, object?>? item)
	{
		item = null;
		var indexerStart = key.IndexOf('[');
		if (indexerStart <= 0 || key[^1] != ']')
		{
			return false;
		}
		
		if (!TryGetProperty(dictionary, key[..indexerStart], out var node))
		{
			throw new UndefinedFieldException(key.ToString());
		}
		
		var indexers = key[indexerStart..];
		while (true)
		{
			var exception = TryGetIndexedArray(node, indexers, out var array, out var index, out var indexerLength);
			if (exception != null)
			{
				throw exception;
			}
			
			indexers = indexers[indexerLength..];
			if (!indexers.IsEmpty)
			{
				node = array!.GetValue(index);
				continue;
			}
			
			if (hasMoreSegments && array!.GetValue(index) is IDictionary<string, object?> subDictionary)
			{
				item = subDictionary;
			}
			else
			{
				array!.SetValue(obj, index);
			}
			
			return true;
		}
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
		if (dictionary.TryGetValue(key, out var value))
		{
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
	
	/// <summary>
	/// Clears the properties. The object holds no unmanaged resources, so it has no finalizer
	/// (a finalizer would keep every instance alive for an extra garbage collection).
	/// </summary>
	public void Dispose()
	{
		this.PropertyDictionary.Clear();
	}
	
	#endregion
}