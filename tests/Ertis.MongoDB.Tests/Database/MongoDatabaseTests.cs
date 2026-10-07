using Ertis.MongoDB.Tests.TestHelpers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDatabase = Ertis.MongoDB.Database.MongoDatabase;

namespace Ertis.MongoDB.Tests.Database;

public class MongoDatabaseTests(MongoDbContainerFixture fixture) : MongoTestBase(fixture)
{
	#region Methods
	
	private MongoDatabase CreateDatabase()
	{
		return new MongoDatabase(this.ClientProvider, this.Settings);
	}
	
	private async Task<ObjectId> InsertAsync(string collectionName, string name)
	{
		var id = ObjectId.GenerateNewId();
		await this.Database.GetCollection<BsonDocument>(collectionName).InsertOneAsync(new BsonDocument { { "_id", id }, { "name", name } }, cancellationToken: CancellationToken);
		return id;
	}
	
	private async Task<string[]> GetNamesAsync(string collectionName)
	{
		var documents = await this.Database.GetCollection<BsonDocument>(collectionName).Find(FilterDefinition<BsonDocument>.Empty).ToListAsync(CancellationToken);
		return documents.Select(x => x["name"].AsString).Order().ToArray();
	}
	
	#endregion
	
	#region Collection Methods
	
	[Fact]
	public async Task CreateRenameAndDropCollection_ManageTheCollections()
	{
		var database = this.CreateDatabase();
		
		await database.CreateCollectionAsync("a", CancellationToken);
		
		// ReSharper disable once MethodHasAsyncOverload
		database.CreateCollection("b");
		await database.RenameCollectionAsync("a", "c", CancellationToken);
		
		// ReSharper disable once MethodHasAsyncOverload
		database.RenameCollection("b", "d");
		Assert.Equal(["c", "d"], (await database.ListCollectionsAsync(cancellationToken: CancellationToken)).Order());
		Assert.Equal(["c"], await database.ListCollectionsAsync(x => x["name"] == "c", CancellationToken));
		
		await database.DropCollectionAsync("c", CancellationToken);
		
		// ReSharper disable once MethodHasAsyncOverload
		database.DropCollection("d");
		Assert.Empty(await database.ListCollectionsAsync(cancellationToken: CancellationToken));
	}
	
	/// <summary>
	/// Characterization: the synchronous listing does not await ForEachAsync; it works while the names come in the first batch
	/// </summary>
	[Fact]
	public async Task ListCollections_ReturnsTheNames()
	{
		var database = this.CreateDatabase();
		await database.CreateCollectionAsync("a", CancellationToken);
		await database.CreateCollectionAsync("b", CancellationToken);
		
		// ReSharper disable once MethodHasAsyncOverload
		Assert.Equal(["a", "b"], database.ListCollections().Order());
		
		// ReSharper disable once MethodHasAsyncOverload
		Assert.Equal(["b"], database.ListCollections(x => x["name"] == "b"));
	}
	
	#endregion
	
	#region Statistics Methods
	
	[Fact]
	public async Task GetDatabaseStatistics_ReadsTheStatistics()
	{
		await this.InsertAsync("a", "x");
		var database = this.CreateDatabase();
		
		var statistics = await database.GetDatabaseStatisticsAsync(CancellationToken);
		
		// ReSharper disable once MethodHasAsyncOverload
		var syncStatistics = database.GetDatabaseStatistics();
		
		Assert.NotNull(statistics);
		Assert.Equal(this.Settings.DefaultAuthDatabase, statistics.DatabaseName);
		Assert.Equal(1, statistics.CollectionCount);
		Assert.Equal(1, statistics.ObjectCount);
		Assert.True(statistics.DataSize > 0);
		Assert.True(statistics.StorageSize > 0);
		Assert.Equal(1, statistics.IndexCount);
		Assert.Equal(1, statistics.State);
		Assert.Equal(statistics.DatabaseName, syncStatistics.DatabaseName);
	}
	
	#endregion
	
	#region Copy Methods
	
	[Fact]
	public async Task CopyOne_CopiesTheDocument()
	{
		var id = await this.InsertAsync("source", "a");
		await this.InsertAsync("source", "b");
		
		await this.CreateDatabase().CopyOneAsync(id.ToString(), "source", "destination");
		
		Assert.Equal(["a"], await this.GetNamesAsync("destination"));
	}
	
	[Fact]
	public async Task CopyAll_CopiesTheDocuments()
	{
		await this.InsertAsync("source", "a");
		await this.InsertAsync("source", "b");
		
		await this.CreateDatabase().CopyAllAsync("source", "destination");
		
		Assert.Equal(["a", "b"], await this.GetNamesAsync("destination"));
	}
	
	[Fact]
	public async Task ReplaceOne_ReplacesTheDocument()
	{
		var id = await this.InsertAsync("source", "new");
		await this.Database.GetCollection<BsonDocument>("destination").InsertOneAsync(new BsonDocument { { "_id", id }, { "name", "old" } }, cancellationToken: CancellationToken);
		
		await this.CreateDatabase().ReplaceOneAsync(id.ToString(), "source", "destination");
		
		Assert.Equal(["new"], await this.GetNamesAsync("destination"));
	}
	
	#endregion
}
