using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Ertis.Net.Http;

// ReSharper disable once UnusedType.Global
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class QueryString
{
	#region Statics
	
	public static IQueryString Empty => new HttpQueryString { IsImmutable = true };
	
	#endregion
	
	#region Methods
	
	public static IQueryString Create()
	{
		return new HttpQueryString();
	}
	
	public static IQueryString Add(string key, object value)
	{
		return new HttpQueryString(new Dictionary<string, object> { { key, value } });
	}
	
	public static IQueryString Add(IQueryString collection)
	{
		return new HttpQueryString(collection.ToDictionary());
	}
	
	#endregion
}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class HttpQueryString : IQueryString
{
	#region Properties
	
	internal bool IsImmutable { get; init; }
	
	private Dictionary<string, object> QueryDictionary { get; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor 1
	/// </summary>
	internal HttpQueryString()
	{
		this.QueryDictionary = new Dictionary<string, object>();
	}
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="dictionary"></param>
	internal HttpQueryString(IDictionary<string, object> dictionary) : this()
	{
		foreach (var pair in dictionary)
		{
			this.QueryDictionary.Add(pair.Key, pair.Value);
		}
	}
	
	#endregion
	
	#region Methods
	
	public IQueryString Add(string key, object value)
	{
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("QueryString key is empty!");
		}
		
		if (value == null || string.IsNullOrEmpty(value.ToString()))
		{
			throw new ArgumentException("QueryString value is null or empty!");
		}
		
		if (this.IsImmutable)
		{
			return new HttpQueryString { { key, value } };
		}
		else
		{
			this.QueryDictionary[key] = value;
			return this;
		}
	}
	
	public IQueryString Add(KeyValuePair<string, object> pair)
	{
		return this.Add(pair.Key, pair.Value);
	}
	
	public IQueryString Add(IQueryString collection)
	{
		IQueryString queryString = this;
		var dictionary = collection.ToDictionary();
		foreach (var pair in dictionary)
		{
			queryString = queryString.Add(pair.Key, pair.Value);
		}
		
		return queryString;
	}
	
	public IQueryString Remove(string key)
	{
        this.QueryDictionary.Remove(key);
        return this;
	}
	
	public IDictionary<string, object> ToDictionary()
	{
		return this.QueryDictionary;
	}
	
	public bool ContainsKey(string key)
	{
		return this.QueryDictionary.ContainsKey(key);
	}
	
	public override string ToString()
	{
		return string.Join("&", 
			this.QueryDictionary
				.Where(x => !string.IsNullOrEmpty(x.Key))
				.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value.ToString() ?? "")}"));
	}
	
	public IEnumerator GetEnumerator()
	{
		return this.QueryDictionary.GetEnumerator();
	}
	
	IEnumerator<object> IEnumerable<object>.GetEnumerator()
	{
		return this.QueryDictionary.Values.GetEnumerator();
	}
	
	#endregion
}