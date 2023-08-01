using MongoDB.Driver;

namespace Ertis.MongoDB.Client;

public interface IMongoClientProvider
{
	#region Properties

	MongoClient Client { get; }

	#endregion
}