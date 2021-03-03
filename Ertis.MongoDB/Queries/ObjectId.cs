namespace Ertis.MongoDB.Queries
{
	public class ObjectId : QueryValue
	{
		#region Properties

		private string Id { get; }

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id"></param>
		public ObjectId(string id) : base(id)
		{
			this.Id = id;
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return $"ObjectId(\"{this.Id}\")";
		}

		#endregion
	}
}