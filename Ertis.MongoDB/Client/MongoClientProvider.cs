using MongoDB.Driver;

namespace Ertis.MongoDB.Client;

public class MongoClientProvider : IMongoClientProvider
{
	#region Properties
	
	public MongoClient Client { get; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="mongoClientSettings"></param>
	public MongoClientProvider(MongoClientSettings mongoClientSettings)
	{
		this.Client = new MongoClient(mongoClientSettings);
	}
	
	#endregion
}