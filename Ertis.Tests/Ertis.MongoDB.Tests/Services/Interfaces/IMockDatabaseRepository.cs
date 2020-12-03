using Ertis.Data.Repository;
using Ertis.Tests.Ertis.MongoDB.Tests.Models;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Services.Interfaces
{
	public interface IMockDatabaseRepository : IRepository<TestModel, string>
	{
		
	}
}