using System;
using System.Collections.Generic;
using System.Linq;

namespace Ertis.Net.Http
{
	public static class QueryString
	{
		#region Statics

		public static readonly IQueryString Empty = new HttpQueryString();

		#endregion
		
		#region Methods

		public static IQueryString Add(string key, object value)
		{
			return new HttpQueryString(new Dictionary<string, object> { { key, value } });
		}
		
		public static IQueryString Add(IQueryString collection)
		{
			return new HttpQueryString(collection.ToDictionary(x => x.Key, y => y.Value));
		}

		#endregion
	}
	
	public class HttpQueryString : Dictionary<string, object>, IQueryString
	{
		#region Constructors

		/// <summary>
		/// Constructor 1
		/// </summary>
		internal HttpQueryString() : this(new Dictionary<string, object>())
		{ }
		
		/// <summary>
		/// Constructor 2
		/// </summary>
		/// <param name="dictionary"></param>
		internal HttpQueryString(IDictionary<string, object> dictionary) : base(dictionary)
		{
			
		}

		#endregion

		#region Methods

		public new IQueryString Add(string key, object value)
		{
			if (!this.ContainsKey(key))
				this.Add(key, value);
			else
				this[key] = value;
			
			return this;
		}

		public IQueryString Add(KeyValuePair<string, object> pair)
		{
			return this.Add(pair.Key, pair.Value);
		}
		
		public IQueryString Add(IQueryString collection)
		{
			IQueryString queryString = this;
			foreach (var pair in collection)
			{
				queryString = this.Add(pair.Key, pair.Value);
			}

			return queryString;
		}

		public new IQueryString Remove(string key)
		{
			if (this.ContainsKey(key))
			{
				this.Remove(key);
			}

			return this;
		}

		public override string ToString()
		{
			return string.Join("&", 
				this.Where(x => !string.IsNullOrEmpty(x.Key) && x.Value != null)
					.Select(x => $"{x.Key}={Uri.EscapeUriString(x.Value?.ToString() ?? "")}"));
		}

		#endregion
	}
}