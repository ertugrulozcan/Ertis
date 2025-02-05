using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Ertis.Net.Http;

// ReSharper disable once UnusedType.Global
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class HeaderCollection
{
	#region Statics
	
	public static IHeaderCollection Empty => new RequestHeaders { IsImmutable = true };
	
	#endregion
	
	#region Methods
	
	public static IHeaderCollection Create()
	{
		return new RequestHeaders();
	}
	
	public static IHeaderCollection Add(string key, object value)
	{
		return new RequestHeaders(new Dictionary<string, object> { { key, value } });
	}
	
	public static IHeaderCollection Add(IHeaderCollection collection)
	{
		return new RequestHeaders(collection.ToDictionary());
	}
	
	#endregion
}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RequestHeaders : IHeaderCollection
{
	#region Properties
	
	internal bool IsImmutable { get; init; }
	
	private Dictionary<string, object> HeadersDictionary { get; }
	
	public IEnumerable<string> Keys => this.HeadersDictionary.Keys;
	
	public IEnumerable<object> Values => this.HeadersDictionary.Values;
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor 1
	/// </summary>
	internal RequestHeaders()
	{
		this.HeadersDictionary = new Dictionary<string, object>();
	}
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="dictionary"></param>
	internal RequestHeaders(IDictionary<string, object> dictionary) : this()
	{
		foreach (var pair in dictionary)
		{
			this.HeadersDictionary.Add(pair.Key, pair.Value);
		}
	}
	
	#endregion
	
	#region Methods
	
	public IHeaderCollection Add(string key, object value)
	{
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("Header key is empty!");
		}
		
		if (value == null || string.IsNullOrEmpty(value.ToString()))
		{
			throw new ArgumentException("Header value is null or empty!");
		}
		
		if (this.IsImmutable)
		{
			return new RequestHeaders { { key, value } };
		}
		else
		{
			this.HeadersDictionary[key] = value;
			return this;
		}
	}
	
	public IHeaderCollection Add(KeyValuePair<string, object> pair)
	{
		return this.Add(pair.Key, pair.Value);
	}
	
	public IHeaderCollection Add(IHeaderCollection collection)
	{
		IHeaderCollection headers = this;
		var dictionary = collection.ToDictionary();
		foreach (var pair in dictionary)
		{
			headers = headers.Add(pair.Key, pair.Value);
		}
		
		return headers;
	}
	
	public IHeaderCollection Remove(string key)
	{
        this.HeadersDictionary.Remove(key);
        return this;
	}
	
	public IDictionary<string, object> ToDictionary()
	{
		return this.HeadersDictionary;
	}
	
	public bool ContainsKey(string key)
	{
		return this.HeadersDictionary.ContainsKey(key);
	}
	
	public override string ToString()
	{
		return string.Join("&", 
			this.HeadersDictionary
				.Where(x => !string.IsNullOrEmpty(x.Key))
				.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value.ToString() ?? "")}"));
	}
	
	public IEnumerator GetEnumerator()
	{
		return this.HeadersDictionary.GetEnumerator();
	}
	
	IEnumerator<object> IEnumerable<object>.GetEnumerator()
	{
		return this.HeadersDictionary.Values.GetEnumerator();
	}
	
	#endregion
}