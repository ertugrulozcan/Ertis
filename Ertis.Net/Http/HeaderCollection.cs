using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ertis.Net.Http
{
	public static class HeaderCollection
	{
		#region Statics

		public static readonly IHeaderCollection Empty = new RequestHeaders();

		#endregion

		#region Methods

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
	
	public class RequestHeaders : IHeaderCollection
	{
		#region Properties

		private Dictionary<string, object> HeadersDictionary { get; }

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
			if (!this.HeadersDictionary.ContainsKey(key))
				this.HeadersDictionary.Add(key, value);
			else
				this.HeadersDictionary[key] = value;
			
			return this;
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
				headers = this.Add(pair.Key, pair.Value);
			}

			return headers;
		}

		public IHeaderCollection Remove(string key)
		{
			if (this.HeadersDictionary.ContainsKey(key))
			{
				this.HeadersDictionary.Remove(key);
			}

			return this;
		}

		public IDictionary<string, object> ToDictionary()
		{
			return this.HeadersDictionary;
		}

		public override string ToString()
		{
			return string.Join("&", 
				this.HeadersDictionary
					.Where(x => !string.IsNullOrEmpty(x.Key) && x.Value != null)
					.Select(x => $"{x.Key}={Uri.EscapeUriString(x.Value?.ToString() ?? "")}"));
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
}