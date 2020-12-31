using System.Collections;
using System.Collections.Generic;

namespace Ertis.MongoDB.Queries
{
	public class QueryArray : IEnumerable<IQueryable>, IQueryable
	{
		#region Properties

		private List<IQueryable> QueryList { get; }

		#endregion
		
		#region Constructors
		
		public QueryArray()
		{
			this.QueryList = new List<IQueryable>();
		}

		public QueryArray(IEnumerable<IQueryable> list) : this()
		{
			if (list != null)
			{
				this.QueryList.AddRange(list);
			}
		}
		
		#endregion

		#region Methods
		
		public void Add(IQuery query)
		{
			this.QueryList.Add(query);
		}
		
		public void Add(object value)
		{
			this.QueryList.Add(new QueryValue(value));
		}
		
		public IEnumerator<IQueryable> GetEnumerator()
		{
			return this.QueryList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public override string ToString()
		{
			return "[ " + string.Join(", ", this.QueryList) + " ]";
		}

		#endregion
	}
}