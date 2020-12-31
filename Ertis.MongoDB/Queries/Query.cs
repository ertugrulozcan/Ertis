namespace Ertis.MongoDB.Queries
{
	public class Query : IQuery
	{
		#region Properties

		public string Key { get; }
		
		public IQueryable Value { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public Query(string key, IQueryable value)
		{
			this.Key = key;
			this.Value = value;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public Query(string key, object value)
		{
			this.Key = key;
			this.Value = new QueryValue(value);
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return "{ " + $"\"{this.Key}\": {this.Value}" + " }";
		}

		#endregion
	}
}