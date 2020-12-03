using Ertis.MongoDB.Repository;
using Ertis.Tests.Ertis.MongoDB.Tests.Models;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Services.Interfaces
{
	public interface IMockDatabaseRepository : IMongoRepository<TestModel>
	{
		
	}
}