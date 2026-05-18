using MongoDB.Driver;

// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.MongoDB.Client;

public interface IMongoClientProvider
{
	#region Properties
	
	MongoClient Client { get; }
	
	#endregion
}