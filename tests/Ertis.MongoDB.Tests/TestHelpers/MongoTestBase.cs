using Ertis.MongoDB.Client;
using Ertis.MongoDB.Configuration;
using MongoDB.Driver;

namespace Ertis.MongoDB.Tests.TestHelpers;

/// <summary>
/// Gives every test its own database in the shared container, dropped after the test
/// </summary>
public abstract class MongoTestBase : IAsyncLifetime
{
	#region Properties
	
	protected IMongoClientProvider ClientProvider { get; }
	
	protected DatabaseSettings Settings { get; }
	
	protected IMongoDatabase Database { get; }
	
	protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;
	
	#endregion
	
	#region Constructors
	
	protected MongoTestBase(MongoDbContainerFixture fixture)
	{
		this.ClientProvider = new MongoClientProvider(MongoClientSettings.FromConnectionString(fixture.ConnectionString));
		this.Settings = new DatabaseSettings
		{
			ConnectionString = fixture.ConnectionString,
			DefaultAuthDatabase = $"test_{Guid.NewGuid():N}"
		};
		
		this.Database = this.ClientProvider.Client.GetDatabase(this.Settings.DefaultAuthDatabase);
	}
	
	#endregion
	
	#region Methods
	
	public ValueTask InitializeAsync()
	{
		return ValueTask.CompletedTask;
	}
	
	public async ValueTask DisposeAsync()
	{
		await this.ClientProvider.Client.DropDatabaseAsync(this.Settings.DefaultAuthDatabase);
		GC.SuppressFinalize(this);
	}
	
	#endregion
}
