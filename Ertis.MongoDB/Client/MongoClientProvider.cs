using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

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
	/// <param name="eventSubscriber"></param>
	public MongoClientProvider(MongoClientSettings mongoClientSettings, IEventSubscriber eventSubscriber = null)
	{
		if (eventSubscriber != null)
		{
			mongoClientSettings.ClusterConfigurator = builder =>
			{
				builder.Subscribe(eventSubscriber);
			};
		}

		this.Client = new MongoClient(mongoClientSettings);
	}

	#endregion
}