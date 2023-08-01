using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Repository;
using Ertis.Tests.Ertis.MongoDB.Tests.Models;
using Ertis.Tests.Ertis.MongoDB.Tests.Services.Interfaces;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Services
{
	public class MockDatabaseRepository : MongoRepositoryBase<TestModel>, IMockDatabaseRepository
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings"></param>
		public MockDatabaseRepository(IDatabaseSettings settings) : base(null, settings, "test")
		{
			
		}

		#endregion
	}
}