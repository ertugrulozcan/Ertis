using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ertis.MongoDB.Queries
{
	public class QueryGroup : IEnumerable<IQueryable>, IQueryable
	{
		#region Properties

		private Dictionary<string, IQueryable> QueryDictionary { get; }

		#endregion
		
		#region Constructors
		
		public QueryGroup()
		{
			this.QueryDictionary = new Dictionary<string, IQueryable>();
		}

		public QueryGroup(IDictionary<string, IQueryable> dict) : this()
		{
			if (dict != null)
			{
				foreach (var pair in dict)
				{
					this.Add(pair.Key, pair.Value);
				}
			}
		}
		
		public QueryGroup(IDictionary<string, object> dict) : this()
		{
			if (dict != null)
			{
				foreach (var pair in dict)
				{
					this.Add(pair.Key, new QueryValue(pair.Value));
				}
			}
		}
		
		public QueryGroup(IEnumerable<IQuery> queries) : this()
		{
			if (queries != null)
			{
				foreach (var query in queries)
				{
					this.Add(query);
				}
			}
		}
		
		#endregion

		#region Methods
		
		public void Add(IQuery query)
		{
			this.QueryDictionary.Add(query.Key, query.Value);
		}
		
		public void Add(string key, IQueryable query)
		{
			this.QueryDictionary.Add(key, query);
		}
		
		public void Add(string key, object value)
		{
			this.Add(key, new QueryValue(value));
		}
		
		public IEnumerator<IQueryable> GetEnumerator()
		{
			return this.QueryDictionary.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public override string ToString()
		{
			return "{ " + string.Join(", ", this.QueryDictionary.Select(x => $"\"{x.Key}\": {x.Value}")) + " }";
		}

		#endregion
	}
}